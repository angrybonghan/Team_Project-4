using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController instance { get; private set; }

    [Header("물리 설정")]
    public float forceMultiplier = 5f; // 공 파워 배율
    public float maxForce = 4f; // 최대 공 파워
    public float minLaunchDragMagnitude = 0.25f; // 최소 마우스 드래그 길이

    [Header("시각적 요소 설정")]
    public GameObject VisualUI; // 드래그할때만 나올 오브젝트들의 엄마 UI
    public GameObject arrowIndicator; // 화살표
    public GameObject dotLineIndicator; // 화살표 꼬리에 나올 점선
    public GameObject stick; // 당구 막대기
    public GameObject stickPower; // 당구 막대기에 표시될 파워 게이지
    public float minStickPowerX = -0.5f; // 최고, 최소 막대기 파워 위치
    public float maxStickPowerX = -2.1f;
    public float arrowDistance = 0.75f; // 화살표가 최대로 늘어날 수 있는 거리
    public float arrowSensitivity = 1.0f; // 화살표 늘어나는 감도
    public float dotLineDistance = 0.3f; // 점 스프라이트 길이 감소 (안하면 너무 길어짐)

    [Header("플레이어 공 잔상")]
    public GameObject playerBallAfterimage;

    // 쉴드 적용 작동시간 (Inspector에서 설정할 수 있도록 public으로 유지)
    // 이 값을 static 변수에 할당하여 static 함수에서 접근 가능하게 합니다.
    [Header("쉴드 적용 작동시간")]
    public float runTime_ = 0.2f;
    public float operatingFrequency_ = 20;

    // static 변수들 (static 함수에서 접근 가능)
    private static float staticRunTime; // runTime_ 값을 받을 static 변수
    private static float staticOperatingFrequency; // operatingFrequency_ 값을 받을 static 변수
    private static Transform shield;
    private static Coroutine staticCurrentGetShieldCoroutine; // Coroutine 변수도 static으로 변경
    public static bool isShieldExistence = false;

    // Raycast를 위한 레이어 마스크
    [Header("Raycast 설정")]
    public LayerMask obstacleLayer; // 레이캐스트 레이어

    [Header("힘에 따른 색상 변화")]
    public Color minForceColor = Color.green;
    public Color maxForceColor = Color.red;

    private Rigidbody2D rb;

    private Vector2 startMousePos;
    private Vector2 endMousePos;

    private bool isDragging = false;
    private bool hasReachedMinDrag = false;

    private void Awake()
    {
        if (instance == null) // 싱글톤
        {
            instance = this;

            // Awake에서 Inspector에서 설정된 값을 static 변수에 할당
            staticRunTime = runTime_;
            staticOperatingFrequency = operatingFrequency_;
        }
        else
        {
            Destroy(gameObject);
            return; // Destroy 후에는 더 이상 진행하지 않도록 추가
        }

        rb = GetComponent<Rigidbody2D>();

        // shield가 null일 경우에만 GetChild(0)로 찾도록 방어 코드 추가
        if (shield == null && transform.childCount > 0)
        {
            shield = transform.GetChild(0);
        }
        // 이 경우 쉴드 관련 기능이 작동하지 않을 수 있음을 알림
    }

    void Start()
    {
        rb.gravityScale = 0f;
        rb.drag = 0f;

        if (VisualUI != null)
        {
            VisualUI.SetActive(false);
        }

        unShield();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!GameManager.canPlay || !DataManager.isGameActionable)
            {
                return;
            }

            isDragging = true;
            hasReachedMinDrag = false;
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (VisualUI != null)
            {
                VisualUI.SetActive(true);
            }
        }

        if (isDragging)
        {
            // 마우스 위치를 구하고, 시작 위치에서 빼서 드래그 거리를 구함
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = startMousePos - currentMousePos;
            float rawDragMagnitude = dragVector.magnitude;

            if (rawDragMagnitude >= minLaunchDragMagnitude) // 한번 최소 길이를 넘었는지 확인
            {
                hasReachedMinDrag = true;
            }
            else if (hasReachedMinDrag && rawDragMagnitude < minLaunchDragMagnitude) // 최소 길이를 넘고 다시 돌아오면 취소
            {
                // UI 끄고 어쩌고저쩌고 다함
                isDragging = false;
                hasReachedMinDrag = false;

                if (VisualUI != null)
                {
                    VisualUI.SetActive(false);
                }
                return;
            }

            float currentDragMagnitude = Mathf.Min(rawDragMagnitude, maxForce); // 마우스 드래그 길이
            float forceRatio = currentDragMagnitude / maxForce;

            Vector2 clampedDirection = dragVector.normalized;
            float angle = Mathf.Atan2(clampedDirection.y, clampedDirection.x) * Mathf.Rad2Deg;
            Color lerpedColor = Color.Lerp(minForceColor, maxForceColor, forceRatio);

            // 레이캐스트
            Vector2 raycastOrigin = transform.position; // 레이 위치 결정
            float raycastDistance = Mathf.Min(currentDragMagnitude * forceMultiplier * arrowSensitivity, arrowDistance * forceMultiplier);
            // 레이 길이에 리미트 걸음

            // 레이 발싸
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, clampedDirection, raycastDistance, obstacleLayer);

            Vector3 arrowTargetPosition; // 화살표가 박힌 위치, 안 박혔으면 최대 길이 위치
            float actualLineLength; // 실제 점선 길이
            // 여기서 변수 선언하는 이유 모르겠으면 이시현 DM으로 물어보셈 ㅇㅇ
            // 아니다 그냥 여기서 말하겠음
            // 밑에 if 안에서 두개의 독립적인 작동을 해야하잖아 이 빵빵탱아~~

            if (hit.collider != null)
            {
                // 충돌 지점에서 멈춥니다.
                arrowTargetPosition = hit.point;
                actualLineLength = (hit.point - raycastOrigin).magnitude;
            }
            else
            {
                // 충돌하지 않으면 최대치로 설정
                arrowTargetPosition = raycastOrigin + (Vector2)clampedDirection * raycastDistance;
                actualLineLength = raycastDistance;
            }

            // 화살표 제어
            if (arrowIndicator != null)
            {
                arrowIndicator.SetActive(true);
                arrowIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
                arrowIndicator.transform.position = arrowTargetPosition; // 레이캐스트 최종위치

                SpriteRenderer arrowRenderer = arrowIndicator.GetComponent<SpriteRenderer>();
                if (arrowRenderer != null)
                {
                    arrowRenderer.color = lerpedColor;
                }
            }

            // 점선 제어
            if (dotLineIndicator != null)
            {
                dotLineIndicator.SetActive(true);
                dotLineIndicator.transform.position = transform.position;
                dotLineIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);

                SpriteRenderer dotRenderer = dotLineIndicator.GetComponent<SpriteRenderer>();
                if (dotRenderer != null)
                {
                    // 점선 길이는 실제 계산된 길이를 사용합니다.
                    dotRenderer.size = new Vector2(actualLineLength * dotLineDistance, dotRenderer.size.y);
                    dotRenderer.color = lerpedColor;
                }
            }

            // 당구막대 제어
            if (stick != null)
            {
                stick.transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            // 당구막대 파워 게이지 제어
            if (stickPower != null)
            {
                stickPower.transform.localPosition = new Vector3(Mathf.Lerp(minStickPowerX, maxStickPowerX, forceRatio), stickPower.transform.localPosition.y, stickPower.transform.localPosition.z);
            }
        }

        // 드래그 종료
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            endMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = startMousePos - endMousePos;

            if (dragVector.magnitude < minLaunchDragMagnitude)
            {
                if (VisualUI != null)
                {
                    VisualUI.SetActive(false);
                }
                isDragging = false;
                hasReachedMinDrag = false;
                return;
            }

            if (dragVector.magnitude > maxForce)
            {
                dragVector = dragVector.normalized * maxForce;
            }

            if (GameManager.SpeedBallChance >= 1)
            {
                rb.AddForce(dragVector * forceMultiplier * 1.5f, ForceMode2D.Impulse);
                StartCoroutine(afterImage());
            }
            else
            {
                rb.AddForce(dragVector * forceMultiplier, ForceMode2D.Impulse);
            }

            if (VisualUI != null)
            {
                VisualUI.SetActive(false);
            }

            isDragging = false;
            hasReachedMinDrag = false;
            GameManager.canPlay = false;


            BlackHoleBall[] blackHoleBalls = FindObjectsOfType<BlackHoleBall>();

            if (blackHoleBalls.Length == 0)
            {
                return; // 더 이상 진행할 필요가 없으므로 함수를 종료합니다.
            }

            // 찾은 각 BlackHoleBall에 대해 BlackHoleStart() 함수를 호출합니다.
            foreach (BlackHoleBall blackHole in blackHoleBalls)
            {
                blackHole.BlackHoleStart();
            }
        }
    }

    IEnumerator afterImage()
    {
        yield return null;
        Vector3 lastAfterImagePosition = transform.position;

        while (!GameManager.canPlay)
        {
            Vector3 currentPosition = transform.position;
            if (Vector3.Distance(currentPosition, lastAfterImagePosition) > 0.175)
            {
                GameObject AfterImage = Instantiate(playerBallAfterimage, transform.position, transform.rotation);
                lastAfterImagePosition = transform.position;
            }
            yield return null;
        }
    }

    public static void unShield()
    {
        if (staticCurrentGetShieldCoroutine != null)
        {
            instance.StopCoroutine(staticCurrentGetShieldCoroutine);
            staticCurrentGetShieldCoroutine = null;
        }

        shield.localScale = new Vector3(4f, 4f, 4f);
        shield.localEulerAngles = new Vector3(shield.localEulerAngles.x, shield.localEulerAngles.y, 225f);
        shield.gameObject.SetActive(false);

        isShieldExistence = false;
    }

    public static void GetShield()
    {
        // 중단하고 새로 시작 (코루틴 있으면)
        if (staticCurrentGetShieldCoroutine != null)
        {
            unShield();
        }

        shield.gameObject.SetActive(true);
        staticCurrentGetShieldCoroutine = instance.StartCoroutine(SmoothShieldTransform(staticRunTime));
        isShieldExistence = true;
    }

    private static IEnumerator SmoothShieldTransform(float duration)
    {
        Vector3 startScale = shield.localScale;
        Vector3 targetScale = new Vector3(1f, 1f, 1f);

        Quaternion startRotation = shield.localRotation;
        Quaternion targetRotation = Quaternion.Euler(shield.localEulerAngles.x, shield.localEulerAngles.y, 45f);

        float timeElapsed = 0f;
        float stepDuration = 1f / staticOperatingFrequency; // 각 단계의 지속 시간

        while (timeElapsed < duration)
        {
            timeElapsed += stepDuration;
            float normalizedTime = Mathf.Min(timeElapsed / duration, 1f); // 0에서 1까지의 진행
            shield.localScale = Vector3.Lerp(startScale, targetScale, normalizedTime);
            shield.localRotation = Quaternion.Slerp(startRotation, targetRotation, normalizedTime); // (Lerp도 가능)

            yield return new WaitForSeconds(stepDuration);
        }

        // 부동 소수점 오차
        shield.localScale = targetScale;
        shield.localRotation = targetRotation;

        staticCurrentGetShieldCoroutine = null;
    }
}