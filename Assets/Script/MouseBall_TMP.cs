using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]
public class MouseBall : MonoBehaviour
{
    // public ����
    [Header("�߻� ����")]
    public float forceMultiplier = 5f;        // �߻� �� ���
    public float maxForce = 4f;                // �ִ� �߻� ��
    public float deceleration = 0.5f;          // ���ӷ�
    public float energyLossFactor = 0.8f;      // �浹 �� ������ �ս� ���

    [Header("�� ����, UI ����")]
    public static int ballCounter = 3;         // ���� �� ����
    public TextMeshProUGUI ballCounterText;    // UI�� ǥ���� �ؽ�Ʈ

    // private ����
    private Rigidbody2D rb;
    private LineRenderer lineRenderer;

    // ���콺 �Է� ����
    private Vector2 startMousePos;
    private Vector2 endMousePos;
    private bool isDragging = false;
    private bool isDead = false;
    private bool isLaunched = false;

    // ���� ��ġ ���� (Hole�� ����� ���� ����)
    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        rb.gravityScale = 0f;
        rb.drag = 0f;

        // ���� ������ �ʱ� ����
        //lineRenderer.positionCount = 2;
        //lineRenderer.enabled = false;
        //lineRenderer.startWidth = 0.1f;
        //lineRenderer.endWidth = 0.1f;

        initialPosition = transform.position;

        UpdateCounterText(); // ���� �� UI ������Ʈ
    }

    void Update()
    {
        // �巡�� ����
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //lineRenderer.enabled = true;
        }

        // �巡�� ��: ���� ǥ�� �� ���� ���
        if (isDragging)
        {
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = startMousePos - currentMousePos;

            float dragMagnitude = dragVector.magnitude;
            Vector2 clampedDirection = dragVector;

            // �ִ� �Ÿ� ����
            if (dragMagnitude > maxForce)
            {
                clampedDirection = dragVector.normalized * maxForce;
                dragMagnitude = maxForce;
            }

            // ���� �ð�ȭ
            //lineRenderer.SetPosition(0, transform.position);
            //lineRenderer.SetPosition(1, transform.position + (Vector3)clampedDirection);

            // ���⿡ ���� ���� ���� ��ȭ
            //Color dragColor = Color.Lerp(Color.green, Color.red, dragMagnitude / maxForce);

            //lineRenderer.startColor = dragColor;
            //lineRenderer.endColor = dragColor;
        }

        // �巡�� ���� �� �� ���ؼ� �߻�
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            endMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = startMousePos - endMousePos;

            // �ִ� �� ����
            if (dragVector.magnitude > maxForce)
                dragVector = dragVector.normalized * maxForce;

            // ���� ���� �߻�
            rb.AddForce(dragVector * forceMultiplier, ForceMode2D.Impulse);
            isLaunched = true;

            //lineRenderer.enabled = false;
            isDragging = false;
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