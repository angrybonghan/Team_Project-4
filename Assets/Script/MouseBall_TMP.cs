using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class MouseBall : MonoBehaviour
{
    // public 변수
    [Header("발사 설정")]
    public float forceMultiplier = 5f;        // 발사 힘 계수
    public float maxForce = 4f;                // 최대 발사 힘
    public float deceleration = 0.5f;          // 감속률
    public float energyLossFactor = 0.8f;      // 충돌 시 에너지 손실 계수

    [Header("공 개수, UI 설정")]
    public static int ballCounter = 3;         // 남은 공 개수
    public TextMeshProUGUI ballCounterText;    // UI에 표시할 텍스트

    // private 변수
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;

    // 마우스 입력 관련
    private Vector2 startMousePos;
    private Vector2 endMousePos;
    private bool isDragging = false;
    private bool isDead = false;
    private bool isLaunched = false;

    // 시작 위치 저장 (Hole에 닿았을 때만 복귀)
    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        rb.gravityScale = 0f;
        rb.drag = 0f;

        // 라인 렌더러 초기 설정
        //lineRenderer.positionCount = 2;
        //lineRenderer.enabled = false;
        //lineRenderer.startWidth = 0.1f;
        //lineRenderer.endWidth = 0.1f;

        initialPosition = transform.position;

        UpdateCounterText(); // 시작 시 UI 업데이트
    }

    void Update()
    {
        // 드래그 시작
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //lineRenderer.enabled = true;
        }

        // 드래그 중: 라인 표시 및 방향 계산
        if (isDragging)
        {
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = startMousePos - currentMousePos;

            float dragMagnitude = dragVector.magnitude;
            Vector2 clampedDirection = dragVector;

            // 최대 거리 제한
            if (dragMagnitude > maxForce)
            {
                clampedDirection = dragVector.normalized * maxForce;
                dragMagnitude = maxForce;
            }

            // 라인 시각화
            //lineRenderer.SetPosition(0, transform.position);
            //lineRenderer.SetPosition(1, transform.position + (Vector3)clampedDirection);

            // 세기에 따라 라인 색상 변화
            //Color dragColor = Color.Lerp(Color.green, Color.red, dragMagnitude / maxForce);

            //lineRenderer.startColor = dragColor;
            //lineRenderer.endColor = dragColor;
        }

        // 드래그 종료 → 힘 가해서 발사
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            endMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = startMousePos - endMousePos;

            // 최대 힘 제한
            if (dragVector.magnitude > maxForce)
                dragVector = dragVector.normalized * maxForce;

            // 힘을 가해 발사
            rb.AddForce(dragVector * forceMultiplier, ForceMode2D.Impulse);
            isLaunched = true;

            //lineRenderer.enabled = false;
            isDragging = false;
        }

        // 속도 0.5 이하 → 즉시 멈추고 카운트 감소
        if (!isDead && isLaunched && rb.velocity.magnitude <= 0.5f)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            isDead = true;
            DecreaseBallCountOnly();  // 위치는 유지하고 카운트만 감소
        }
    }

    void FixedUpdate()
    {
        // 감속 적용 (속도 0.5 이상일 때만)
        if (rb.velocity.magnitude > 0.5f)
        {
            rb.velocity *= (1 - deceleration * Time.fixedDeltaTime);
        }
    }

    // 충돌 시 에너지 손실
    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.velocity *= energyLossFactor;
    }

    // 변경 제안 : OnTriggerEnter2D 의 판정을 각 구멍에서 처리하는 편이 좋아 보임 - 이시현
    // Hole에 닿았을 경우만 위치 리셋
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDead && other.CompareTag("Hole"))
        {
            isDead = true;
            ResetBallAndDecreaseCount();
        }
    }

    // 공 멈췄을 때 카운트만 감소
    void DecreaseBallCountOnly()
    {
        ballCounter = Mathf.Max(0, ballCounter - 1);
        UpdateCounterText();
        isLaunched = false;
        isDead = false;
    }

    // Hole에 빠졌을 때 위치 리셋 + 카운트 감소
    void ResetBallAndDecreaseCount()
    {
        ballCounter = Mathf.Max(0, ballCounter - 1);
        UpdateCounterText();

        transform.position = initialPosition;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        isDead = false;
        isLaunched = false;
    }

    // UI 텍스트 업데이트
    void UpdateCounterText()
    {
        if (ballCounterText != null)
        {
            ballCounterText.text = "Balls: " + ballCounter;
        }
    }
}