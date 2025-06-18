using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance { get; private set; }

    private static Camera mainCamera;
    private static Coroutine currentCameraLerpCoroutine = null;
    private static Coroutine currentCameraShakeCoroutine = null;

    public float lerpOperatingFrequency = 30f;

    private static bool isTransitioning = false;
    private static bool isShaking = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            enabled = false;
            return;
        }

        if (!mainCamera.orthographic)
        {
        }
    }

    public static void Goto(Vector3 targetPosition, float size)
    {
        if (mainCamera == null)
        {
            return;
        }

        if (isTransitioning && currentCameraLerpCoroutine != null)
        {
            Instance.StopCoroutine(currentCameraLerpCoroutine);
            currentCameraLerpCoroutine = null;
            isTransitioning = false;
        }
        if (isShaking && currentCameraShakeCoroutine != null)
        {
            Instance.StopCoroutine(currentCameraShakeCoroutine);
            currentCameraShakeCoroutine = null;
            isShaking = false;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = size;
    }

    public static Coroutine LerpGoto(Vector3 targetPosition, float size, float duration)
    {
        if (mainCamera == null)
        {
            return null;
        }

        if (duration <= 0)
        {
            Goto(targetPosition, size);
            return null;
        }

        if (isTransitioning && currentCameraLerpCoroutine != null)
        {
            Instance.StopCoroutine(currentCameraLerpCoroutine);
            currentCameraLerpCoroutine = null;
        }
        if (isShaking && currentCameraShakeCoroutine != null)
        {
            Instance.StopCoroutine(currentCameraShakeCoroutine);
            currentCameraShakeCoroutine = null;
            isShaking = false;
        }

        currentCameraLerpCoroutine = Instance.StartCoroutine(LerpCameraCoroutine(targetPosition, size, duration));
        return currentCameraLerpCoroutine;
    }

    private static IEnumerator LerpCameraCoroutine(Vector3 targetPosition, float targetSize, float duration)
    {
        isTransitioning = true;
        Vector3 startPosition = mainCamera.transform.position;
        float startSize = mainCamera.orthographicSize;

        float stepDuration = duration / Instance.lerpOperatingFrequency;
        int totalSteps = Mathf.CeilToInt(Instance.lerpOperatingFrequency * duration);

        for (int i = 0; i <= totalSteps; i++)
        {
            float normalizedTime = (float)i / totalSteps;

            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, normalizedTime);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, normalizedTime);

            yield return new WaitForSeconds(stepDuration);
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = targetSize;

        isTransitioning = false;
        currentCameraLerpCoroutine = null;
    }

    /// <summary>
    /// 흔들림 크기, 시간, 흔들림 주기
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="duration"></param>
    /// <param name="frequency"></param>
    public static Coroutine Shake(float intensity, float duration, float frequency)
    {
        if (mainCamera == null)
        {
            return null;
        }

        if (intensity < 0 || duration <= 0) // intensity는 0도 허용 가능 (흔들림 없음)
        {
            return null;
        }

        // frequency는 0보다 커야 합니다. (WaitForSeconds에 0이하 값 넣으면 오류)
        if (frequency <= 0) frequency = 0.001f;

        if (isShaking && currentCameraShakeCoroutine != null)
        {
            Instance.StopCoroutine(currentCameraShakeCoroutine);
            currentCameraShakeCoroutine = null;
        }
        if (isTransitioning && currentCameraLerpCoroutine != null)
        {
            Instance.StopCoroutine(currentCameraLerpCoroutine);
            currentCameraLerpCoroutine = null;
            isTransitioning = false;
        }

        currentCameraShakeCoroutine = Instance.StartCoroutine(ShakeCoroutine(intensity, duration, frequency));
        return currentCameraShakeCoroutine;
    }

    private static IEnumerator ShakeCoroutine(float intensity, float duration, float frequency)
    {
        isShaking = true;
        Vector3 initialPosition = mainCamera.transform.position; // 흔들림 시작 전 카메라의 원래 위치

        float elapsedTotalTime = 0.0f; // 흔들림 지속 시간 추적

        // PerlinNoise 대신 Random.insideUnitCircle을 사용하므로, 더 이상 offsetX, offsetY는 필요 없습니다.

        while (elapsedTotalTime < duration)
        {
            // 다음 흔들림이 발생할 때까지 frequency 시간만큼 기다립니다.
            yield return new WaitForSeconds(frequency);

            // 경과 시간 업데이트 (WaitForSeconds 이후에)
            elapsedTotalTime += frequency;

            // 흔들림 지속 시간이 끝나면 루프를 종료합니다.
            // 마지막 흔들림이 duration을 초과하는 것을 방지
            if (elapsedTotalTime > duration) break;

            // 랜덤한 방향으로 이동할 벡터를 생성합니다.
            // Random.insideUnitCircle은 반지름이 1인 원 내부의 랜덤한 2D 포인트를 반환합니다.
            // 여기에 intensity를 곱하여 최대 이동 거리를 조절합니다.
            Vector2 randomShakeOffset2D = Random.insideUnitCircle * intensity;

            // Z축은 변경하지 않고, X, Y만 흔들리도록 합니다.
            Vector3 targetShakePosition = initialPosition + new Vector3(randomShakeOffset2D.x, randomShakeOffset2D.y, 0f);

            // 카메라 위치를 즉시 새로운 랜덤 위치로 설정합니다.
            mainCamera.transform.position = targetShakePosition;
        }

        // 흔들림이 끝나면 카메라를 원래 시작 위치로 되돌립니다.
        mainCamera.transform.position = initialPosition;
        isShaking = false;
        currentCameraShakeCoroutine = null;
    }
}