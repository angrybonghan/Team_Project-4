using UnityEngine;
using TMPro;

namespace SeongnamSiGyeonggiDoSouthKorea
{
    // [RequireComponent(typeof(LineRenderer))] // Line Renderer를 사용하지 않는다면 이 줄은 제거.
    public class MouseBall : MonoBehaviour
    {
        // 물리 관련 설정
        [Header("물리 설정")]
        public float forceMultiplier = 5f;         // 발사 힘 계수
        public float maxForce = 4f;                // 최대 발사 힘
        public float deceleration = 0.5f;          // 감속률
        public float energyLossFactor = 0.8f;      // 충돌 시 에너지 손실 계수

        // UI 관련
        [Header("UI 설정")]
        public TextMeshProUGUI ballCounterText;    // UI에 표시할 텍스트
        public static int ballCounter = 3;         // 남은 공 개수

        // 시각적 요소
        [Header("시각적 요소 설정")]
        public GameObject VisualUI; // 드래그를 제외하고 보이지 않아야하는 개체들의 부모 오브젝트
        public GameObject arrowIndicator;   // 화살표 스프라이트
        public GameObject dotLineIndicator;     // 화살표와 공 사이의 점 스프라이트 (SpriteRenderer의 Draw Mode: Tiled)
        public GameObject stick;    // 당구 막대 (큐대?) && 죄송, 이시현(본인)이 그 막대기 뭐라 부르는지를 모름;;;;
        public GameObject stickPower; // 당구 막대의 파워 게이지 도트
        public float minStickPowerX = -0.5f;      // 최소 힘일 때 stickPower의 X 좌표
        public float maxStickPowerX = -2.1f;      // 최대 힘일 때 stickPower의 X 좌표
        public float arrowDistance = 0.75f;     // 화살표가 공에서 얼마나 멀리 떨어질지
        public float dotLineDistance = 0.3f;    // 점 스프라이트 길이 감소 (안하면 너무 길어짐)

        [Header("힘에 따른 색상 변화")]
        public Color minForceColor = Color.green;  // 최소 힘일 때의 색상 (초록색)
        public Color maxForceColor = Color.red;    // 최대 힘일 때의 색상 (빨간색)


        private Rigidbody2D rb;

        private Vector2 startMousePos;
        private Vector2 endMousePos;

        private bool isDragging = false;
        private bool isDead = false;
        private bool isLaunched = false;

        private Vector3 initialPosition;


        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            rb.gravityScale = 0f;
            rb.drag = 0f;

            initialPosition = transform.position;

            UpdateCounterText(); // 시작 시 UI 업데이트

            if (VisualUI != null)
            {
                VisualUI.SetActive(false); // 시작할 때 VisualUI 투명화
            }
        }

        void Update()
        {
            // 드래그 시작
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (VisualUI != null)
                {
                    VisualUI.SetActive(true); // VisualUI 활성화
                }
            }

            // 드래그 중
            if (isDragging)
            {
                Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 dragVector = startMousePos - currentMousePos; // 마우스 드래그 벡터

                // 드래그 길이를 최대 힘으로 제한하고, 그 길이를 바탕으로 힘의 비율 계산
                float currentDragMagnitude = Mathf.Min(dragVector.magnitude, maxForce);
                // 힘 비율 (0.0..1.0) 계산: 당긴 힘 / 최대 힘
                float forceRatio = currentDragMagnitude / maxForce;

                Vector2 clampedDirection = dragVector.normalized;                // 방향 벡터
                float angle = Mathf.Atan2(clampedDirection.y, clampedDirection.x) * Mathf.Rad2Deg; // 회전 각도

                // Lerp를 사용하여 힘의 비율에 따라 색상 보간
                Color lerpedColor = Color.Lerp(minForceColor, maxForceColor, forceRatio);


                // 화살표 제어
                if (arrowIndicator != null)
                {
                    arrowIndicator.SetActive(true); // 화살표 활성화
                    arrowIndicator.transform.rotation = Quaternion.Euler(0, 0, angle); // 회전
                    // 화살표 위치는 제한된 드래그 크기에 비례하여 조절
                    arrowIndicator.transform.position = transform.position + (Vector3)clampedDirection * currentDragMagnitude * arrowDistance;

                    // 화살표 스프라이트 렌더러 색상 변경
                    SpriteRenderer arrowRenderer = arrowIndicator.GetComponent<SpriteRenderer>();
                    if (arrowRenderer != null)
                    {
                        arrowRenderer.color = lerpedColor;
                    }
                }

                // 점선 제어
                if (dotLineIndicator != null)
                {
                    dotLineIndicator.SetActive(true); // 점선 활성화
                    dotLineIndicator.transform.position = transform.position; // 시작 위치
                    dotLineIndicator.transform.rotation = Quaternion.Euler(0, 0, angle); // 회전

                    SpriteRenderer dotRenderer = dotLineIndicator.GetComponent<SpriteRenderer>();
                    if (dotRenderer != null && arrowIndicator != null)
                    {
                        // 점선 길이는 화살표 위치와 공 사이의 거리를 기반으로 계산
                        float lineLength = (arrowIndicator.transform.position - transform.position).magnitude;
                        dotRenderer.size = new Vector2(lineLength * dotLineDistance, dotRenderer.size.y);

                        // 점선 스프라이트 렌더러 색상 변경
                        dotRenderer.color = lerpedColor;
                    }
                }

                // 당구막대 제어
                if (stick != null)
                {
                    stick.transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                // 당구막대 파워 제어
                if (stick != null)
                {
                    stick.transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                // 당구막대 파워 제어
                if (stickPower != null)
                {
                    // Lerp를 사용하여 힘의 비율에 따라 X 좌표 보간
                    stickPower.transform.localPosition = new Vector3(Mathf.Lerp(minStickPowerX, maxStickPowerX, forceRatio), stickPower.transform.localPosition.y, stickPower.transform.localPosition.z);
                }
            }



            // 드래그 종료 → 힘 가해서 발사
            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                endMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 dragVector = startMousePos - endMousePos;

                // 최대 힘 제한
                if (dragVector.magnitude > maxForce)
                    dragVector = dragVector.normalized * maxForce;

                // 힘을 가해 발사 (공은 마우스가 당겨진 방향의 반대로 발사)
                rb.AddForce(dragVector * forceMultiplier, ForceMode2D.Impulse);
                isLaunched = true;

                if (VisualUI != null)
                {
                    VisualUI.SetActive(false); // VisualUI 비활성화
                }

                isDragging = false; // isDragging 변수 거짓으로 변경
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
}