using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance { get; private set; }

    private static Camera mainCamera;
    private static Coroutine currentCameraLerpCoroutine = null;
    private static Coroutine currentCameraShakeCoroutine = null;

    [Header("보간 주기")]
    public float lerpOperatingFrequency = 60f;
    [Header("불규칙성 강도")]
    public float shakeNoiseScale = 0.5f;

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

    public static void Goto(float x, float y, float z, float size)
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

        mainCamera.transform.position = new Vector3(x, y, z);
        mainCamera.orthographicSize = size;
    }

    public static void LerpGoto(float x, float y, float z, float size, float duration)
    {
        if (mainCamera == null)
        {
            return;
        }

        if (duration <= 0)
        {
            Goto(x, y, z, size);
            return;
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

        currentCameraLerpCoroutine = Instance.StartCoroutine(LerpCameraCoroutine(new Vector3(x, y, z), size, duration));
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
    /// 강도-시간-주기
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="duration"></param>
    /// <param name="frequency"></param>
    public static void Shake(float intensity, float duration, float frequency)
    {
        if (mainCamera == null)
        {
            return;
        }

        if (intensity <= 0 || duration <= 0)
        {
            return;
        }

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
    }

    private static IEnumerator ShakeCoroutine(float intensity, float duration, float frequency)
    {
        isShaking = true;
        Vector3 originalPosition = mainCamera.transform.position;

        float elapsed = 0.0f;

        float offsetX = Random.Range(-1000f, 1000f);
        float offsetY = Random.Range(-1000f, 1000f);

        while (elapsed < duration)
        {
            float x = originalPosition.x + (Mathf.PerlinNoise(offsetX + elapsed * frequency * Instance.shakeNoiseScale, 0f) * 2 - 1) * intensity;
            float y = originalPosition.y + (Mathf.PerlinNoise(0f, offsetY + elapsed * frequency * Instance.shakeNoiseScale) * 2 - 1) * intensity;

            mainCamera.transform.position = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalPosition;
        isShaking = false;
        currentCameraShakeCoroutine = null;
    }
}