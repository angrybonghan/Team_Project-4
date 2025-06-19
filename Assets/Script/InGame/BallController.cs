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

    [Header("크로스 마커")]
    public GameObject CrossMarker;

    // 쉴드 적용 작동시간 (Inspector에서 설정할 수 있도록 public으로 유지)
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
    private bool isUIVisible = false; // VisualUI가 현재 보이는지 여부를 추적하는 변수 추가

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
    }

    void Start()
    {
        rb.gravityScale = 0f;
        rb.drag = 0f;

        if (VisualUI != null)
        {
            VisualUI.SetActive(false);
            isUIVisible = false; // 초기 상태는 보이지 않음
        }

        unShield();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }

            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            transform.position = mouseWorldPosition;

            Debug.Log($"Z 키가 눌렸습니다. 오브젝트 '{gameObject.name}'의 속도를 0으로 설정하고, 마우스 위치 ({mouseWorldPosition})로 이동했습니다.");
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (!GameManager.canPlay || !DataManager.isGameActionable)
            {
                return;
            }

            isDragging = true;
            hasReachedMinDrag = false;
            isUIVisible = false; // 드래그 시작 시 UI는 아직 안 보임
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // 여기에 VisualUI.SetActive(true)는 더 이상 놓지 않습니다.
        }

        if (isDragging)
        {
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = startMousePos - currentMousePos;
            float rawDragMagnitude = dragVector.magnitude;

            // VisualUI를 띄울 최소 드래그 거리를 계산
            float minUIVisibleDragMagnitude = minLaunchDragMagnitude / 5f;

            // 드래그가 minUIVisibleDragMagnitude를 넘었고, UI가 아직 보이지 않는 경우 활성화
            if (rawDragMagnitude >= minUIVisibleDragMagnitude && !isUIVisible)
            {
                if (VisualUI != null)
                {
                    VisualUI.SetActive(true);
                    Instantiate(CrossMarker, startMousePos, transform.rotation);
                    isUIVisible = true; // UI가 보인다고 상태 업데이트
                }
            }


            if (rawDragMagnitude >= minLaunchDragMagnitude)
            {
                hasReachedMinDrag = true;
            }
            else if (hasReachedMinDrag && rawDragMagnitude < minLaunchDragMagnitude)
            {
                // UI 끄고 어쩌고저쩌고 다함
                isDragging = false;
                hasReachedMinDrag = false;

                if (VisualUI != null)
                {
                    VisualUI.SetActive(false);
                    isUIVisible = false; // UI가 보이지 않는다고 상태 업데이트
                }
                return;
            }

            // UI가 활성화된 경우에만 나머지 시각적 업데이트 로직 실행
            if (isUIVisible)
            {
                float currentDragMagnitude = Mathf.Min(rawDragMagnitude, maxForce);
                float forceRatio = currentDragMagnitude / maxForce;

                Vector2 clampedDirection = dragVector.normalized;
                float angle = Mathf.Atan2(clampedDirection.y, clampedDirection.x) * Mathf.Rad2Deg;
                Color lerpedColor = Color.Lerp(minForceColor, maxForceColor, forceRatio);

                Vector2 raycastOrigin = transform.position;
                float raycastDistance = Mathf.Min(currentDragMagnitude * forceMultiplier * arrowSensitivity, arrowDistance * forceMultiplier);

                RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, clampedDirection, raycastDistance, obstacleLayer);

                Vector3 arrowTargetPosition;
                float actualLineLength;

                if (hit.collider != null)
                {
                    arrowTargetPosition = hit.point;
                    actualLineLength = (hit.point - raycastOrigin).magnitude;
                }
                else
                {
                    arrowTargetPosition = raycastOrigin + (Vector2)clampedDirection * raycastDistance;
                    actualLineLength = raycastDistance;
                }

                if (arrowIndicator != null)
                {
                    arrowIndicator.SetActive(true);
                    arrowIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
                    arrowIndicator.transform.position = arrowTargetPosition;

                    SpriteRenderer arrowRenderer = arrowIndicator.GetComponent<SpriteRenderer>();
                    if (arrowRenderer != null)
                    {
                        arrowRenderer.color = lerpedColor;
                    }
                }

                if (dotLineIndicator != null)
                {
                    dotLineIndicator.SetActive(true);
                    dotLineIndicator.transform.position = transform.position;
                    dotLineIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);

                    SpriteRenderer dotRenderer = dotLineIndicator.GetComponent<SpriteRenderer>();
                    if (dotRenderer != null)
                    {
                        dotRenderer.size = new Vector2(actualLineLength * dotLineDistance, dotRenderer.size.y);
                        dotRenderer.color = lerpedColor;
                    }
                }

                if (stick != null)
                {
                    stick.transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                if (stickPower != null)
                {
                    stickPower.transform.localPosition = new Vector3(Mathf.Lerp(minStickPowerX, maxStickPowerX, forceRatio), stickPower.transform.localPosition.y, stickPower.transform.localPosition.z);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            endMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = startMousePos - endMousePos;

            if (VisualUI != null)
            {
                VisualUI.SetActive(false); // 드래그 종료 시 UI 비활성화
                isUIVisible = false; // 상태 업데이트
            }

            if (dragVector.magnitude < minLaunchDragMagnitude)
            {
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

            isDragging = false;
            hasReachedMinDrag = false;
            GameManager.canPlay = false;

            BlackHoleBall[] blackHoleBalls = FindObjectsOfType<BlackHoleBall>();

            if (blackHoleBalls.Length == 0)
            {
                return;
            }

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
        float stepDuration = 1f / staticOperatingFrequency;

        while (timeElapsed < duration)
        {
            timeElapsed += stepDuration;
            float normalizedTime = Mathf.Min(timeElapsed / duration, 1f);
            shield.localScale = Vector3.Lerp(startScale, targetScale, normalizedTime);
            shield.localRotation = Quaternion.Slerp(startRotation, targetRotation, normalizedTime);

            yield return new WaitForSeconds(stepDuration);
        }

        shield.localScale = targetScale;
        shield.localRotation = targetRotation;

        staticCurrentGetShieldCoroutine = null;
    }
}