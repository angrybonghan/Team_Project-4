using UnityEngine;
using TMPro;

namespace SeongnamSiGyeonggiDoSouthKorea
{
    // [RequireComponent(typeof(LineRenderer))] // Line Renderer�� ������� �ʴ´ٸ� �� ���� ����.
    public class MouseBall : MonoBehaviour
    {
        // ���� ���� ����
        [Header("���� ����")]
        public float forceMultiplier = 5f;         // �߻� �� ���
        public float maxForce = 4f;                // �ִ� �߻� ��
        public float deceleration = 0.5f;          // ���ӷ�
        public float energyLossFactor = 0.8f;      // �浹 �� ������ �ս� ���

        // UI ����
        [Header("UI ����")]
        public TextMeshProUGUI ballCounterText;    // UI�� ǥ���� �ؽ�Ʈ
        public static int ballCounter = 3;         // ���� �� ����

        // �ð��� ���
        [Header("�ð��� ��� ����")]
        public GameObject VisualUI; // �巡�׸� �����ϰ� ������ �ʾƾ��ϴ� ��ü���� �θ� ������Ʈ
        public GameObject arrowIndicator;   // ȭ��ǥ ��������Ʈ
        public GameObject dotLineIndicator;     // ȭ��ǥ�� �� ������ �� ��������Ʈ (SpriteRenderer�� Draw Mode: Tiled)
        public GameObject stick;    // �籸 ���� (ť��?) && �˼�, �̽���(����)�� �� ����� ���� �θ������� ��;;;;
        public GameObject stickPower; // �籸 ������ �Ŀ� ������ ��Ʈ
        public float minStickPowerX = -0.5f;      // �ּ� ���� �� stickPower�� X ��ǥ
        public float maxStickPowerX = -2.1f;      // �ִ� ���� �� stickPower�� X ��ǥ
        public float arrowDistance = 0.75f;     // ȭ��ǥ�� ������ �󸶳� �ָ� ��������
        public float dotLineDistance = 0.3f;    // �� ��������Ʈ ���� ���� (���ϸ� �ʹ� �����)

        [Header("���� ���� ���� ��ȭ")]
        public Color minForceColor = Color.green;  // �ּ� ���� ���� ���� (�ʷϻ�)
        public Color maxForceColor = Color.red;    // �ִ� ���� ���� ���� (������)


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

            UpdateCounterText(); // ���� �� UI ������Ʈ

            if (VisualUI != null)
            {
                VisualUI.SetActive(false); // ������ �� VisualUI ����ȭ
            }
        }

        void Update()
        {
            // �巡�� ����
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (VisualUI != null)
                {
                    VisualUI.SetActive(true); // VisualUI Ȱ��ȭ
                }
            }

            // �巡�� ��
            if (isDragging)
            {
                Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 dragVector = startMousePos - currentMousePos; // ���콺 �巡�� ����

                // �巡�� ���̸� �ִ� ������ �����ϰ�, �� ���̸� �������� ���� ���� ���
                float currentDragMagnitude = Mathf.Min(dragVector.magnitude, maxForce);
                // �� ���� (0.0..1.0) ���: ��� �� / �ִ� ��
                float forceRatio = currentDragMagnitude / maxForce;

                Vector2 clampedDirection = dragVector.normalized;                // ���� ����
                float angle = Mathf.Atan2(clampedDirection.y, clampedDirection.x) * Mathf.Rad2Deg; // ȸ�� ����

                // Lerp�� ����Ͽ� ���� ������ ���� ���� ����
                Color lerpedColor = Color.Lerp(minForceColor, maxForceColor, forceRatio);


                // ȭ��ǥ ����
                if (arrowIndicator != null)
                {
                    arrowIndicator.SetActive(true); // ȭ��ǥ Ȱ��ȭ
                    arrowIndicator.transform.rotation = Quaternion.Euler(0, 0, angle); // ȸ��
                    // ȭ��ǥ ��ġ�� ���ѵ� �巡�� ũ�⿡ ����Ͽ� ����
                    arrowIndicator.transform.position = transform.position + (Vector3)clampedDirection * currentDragMagnitude * arrowDistance;

                    // ȭ��ǥ ��������Ʈ ������ ���� ����
                    SpriteRenderer arrowRenderer = arrowIndicator.GetComponent<SpriteRenderer>();
                    if (arrowRenderer != null)
                    {
                        arrowRenderer.color = lerpedColor;
                    }
                }

                // ���� ����
                if (dotLineIndicator != null)
                {
                    dotLineIndicator.SetActive(true); // ���� Ȱ��ȭ
                    dotLineIndicator.transform.position = transform.position; // ���� ��ġ
                    dotLineIndicator.transform.rotation = Quaternion.Euler(0, 0, angle); // ȸ��

                    SpriteRenderer dotRenderer = dotLineIndicator.GetComponent<SpriteRenderer>();
                    if (dotRenderer != null && arrowIndicator != null)
                    {
                        // ���� ���̴� ȭ��ǥ ��ġ�� �� ������ �Ÿ��� ������� ���
                        float lineLength = (arrowIndicator.transform.position - transform.position).magnitude;
                        dotRenderer.size = new Vector2(lineLength * dotLineDistance, dotRenderer.size.y);

                        // ���� ��������Ʈ ������ ���� ����
                        dotRenderer.color = lerpedColor;
                    }
                }

                // �籸���� ����
                if (stick != null)
                {
                    stick.transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                // �籸���� �Ŀ� ����
                if (stick != null)
                {
                    stick.transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                // �籸���� �Ŀ� ����
                if (stickPower != null)
                {
                    // Lerp�� ����Ͽ� ���� ������ ���� X ��ǥ ����
                    stickPower.transform.localPosition = new Vector3(Mathf.Lerp(minStickPowerX, maxStickPowerX, forceRatio), stickPower.transform.localPosition.y, stickPower.transform.localPosition.z);
                }
            }



            // �巡�� ���� �� �� ���ؼ� �߻�
            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                endMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 dragVector = startMousePos - endMousePos;

                // �ִ� �� ����
                if (dragVector.magnitude > maxForce)
                    dragVector = dragVector.normalized * maxForce;

                // ���� ���� �߻� (���� ���콺�� ����� ������ �ݴ�� �߻�)
                rb.AddForce(dragVector * forceMultiplier, ForceMode2D.Impulse);
                isLaunched = true;

                if (VisualUI != null)
                {
                    VisualUI.SetActive(false); // VisualUI ��Ȱ��ȭ
                }

                isDragging = false; // isDragging ���� �������� ����
            }

            // �ӵ� 0.5 ���� �� ��� ���߰� ī��Ʈ ����
            if (!isDead && isLaunched && rb.velocity.magnitude <= 0.5f)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;

                isDead = true;
                DecreaseBallCountOnly();  // ��ġ�� �����ϰ� ī��Ʈ�� ����
            }
        }

        void FixedUpdate()
        {
            // ���� ���� (�ӵ� 0.5 �̻��� ����)
            if (rb.velocity.magnitude > 0.5f)
            {
                rb.velocity *= (1 - deceleration * Time.fixedDeltaTime);
            }
        }

        // �浹 �� ������ �ս�
        void OnCollisionEnter2D(Collision2D collision)
        {
            rb.velocity *= energyLossFactor;
        }

        // ���� ���� : OnTriggerEnter2D �� ������ �� ���ۿ��� ó���ϴ� ���� ���� ���� - �̽���
        // Hole�� ����� ��츸 ��ġ ����
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!isDead && other.CompareTag("Hole"))
            {
                isDead = true;
                ResetBallAndDecreaseCount();
            }
        }

        // �� ������ �� ī��Ʈ�� ����
        void DecreaseBallCountOnly()
        {
            ballCounter = Mathf.Max(0, ballCounter - 1);
            UpdateCounterText();
            isLaunched = false;
            isDead = false;
        }

        // Hole�� ������ �� ��ġ ���� + ī��Ʈ ����
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

        // UI �ؽ�Ʈ ������Ʈ
        void UpdateCounterText()
        {
            if (ballCounterText != null)
            {
                ballCounterText.text = "Balls: " + ballCounter;
            }
        }
    }
}