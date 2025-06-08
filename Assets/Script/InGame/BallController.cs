using UnityEngine;

public class BallController : MonoBehaviour
{
    // ���� ���� ����
    [Header("���� ����")]
    public float forceMultiplier = 5f;             // �߻� �� ���
    public float maxForce = 4f;                    // �ִ� �߻� ��
    public float minLaunchDragMagnitude = 0.25f;    // �߻縦 ���� �ּ� �巡�� ����

    [Header("�ð��� ��� ����")]
    public GameObject VisualUI; // �巡�׸� �����ϰ� ������ �ʾƾ��ϴ� ��ü���� �θ� ������Ʈ
    public GameObject arrowIndicator;    // ȭ��ǥ ��������Ʈ
    public GameObject dotLineIndicator;      // ȭ��ǥ�� �� ������ �� ��������Ʈ (SpriteRenderer�� Draw Mode: Tiled)
    public GameObject stick;     // �籸 ���� (ť��?)
    public GameObject stickPower; // �籸 ������ �Ŀ� ������ ��Ʈ
    public float minStickPowerX = -0.5f;       // �ּ� ���� �� stickPower�� X ��ǥ
    public float maxStickPowerX = -2.1f;       // �ִ� ���� �� stickPower�� X ��ǥ
    public float arrowDistance = 0.75f;       // ȭ��ǥ�� ������ �󸶳� �ָ� ��������
    public float dotLineDistance = 0.3f;    // �� ��������Ʈ ���� ���� (���ϸ� �ʹ� �����)

    [Header("���� ���� ���� ��ȭ")]
    public Color minForceColor = Color.green;    // �ּ� ���� ���� ���� (�ʷϻ�)
    public Color maxForceColor = Color.red;    // �ִ� ���� ���� ���� (������)

    private Rigidbody2D rb;

    private Vector2 startMousePos;
    private Vector2 endMousePos;

    private bool isDragging = false;
    private bool hasReachedMinDrag = false; //�ּ� �巡�� ���̿� �����ߴ���?

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.gravityScale = 0f;
        rb.drag = 0f;

        if (VisualUI != null)
        {
            VisualUI.SetActive(false); // ������ �� VisualUI ��Ȱ��ȭ
        }
    }

    void Update()
    {
        // �巡�� ����
        if (Input.GetMouseButtonDown(0))
        {
            if (!GameManager.canPlay)
            {
                return;
            }

            isDragging = true;
            hasReachedMinDrag = false; // �巡�� ���� �� �ʱ�ȭ
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
            float rawDragMagnitude = dragVector.magnitude; // ���� �巡�� ����

            //�ּ� �巡�� ���̿� �����ߴ��� Ȯ�� �� ������ �� ���
            if (rawDragMagnitude >= minLaunchDragMagnitude)
            {
                hasReachedMinDrag = true; // �ּ� �巡�� ���̿� ��������
            }
            // ���� �� ���̶� �ּ� ���̿� �����߾��µ�, ���� �巡�� ���̰� �ٽ� �ּ� ���� ���Ϸ� �����Դٸ� �巡�� ���
            else if (hasReachedMinDrag && rawDragMagnitude < minLaunchDragMagnitude)
            {
                isDragging = false; // �巡�� ���� ����
                hasReachedMinDrag = false; // ���� �巡�׸� ���� �ʱ�ȭ

                if (VisualUI != null)
                {
                    VisualUI.SetActive(false); // UI ��Ȱ��ȭ
                }
                // GameManager.canPlay�� �߻� �����̹Ƿ� true �����ϰ���
                return; // ���� ��@@�ٸ�
            }

            // �巡�� ���̸� �ִ� ������ �����ϰ�, �� ���̸� �������� ���� ���� ���
            float currentDragMagnitude = Mathf.Min(rawDragMagnitude, maxForce);
            // �� ���� (0.0..1.0) ���: ��� �� / �ִ� ��
            float forceRatio = currentDragMagnitude / maxForce;

            Vector2 clampedDirection = dragVector.normalized;        // ���� ����
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

            // �籸���� �Ŀ� ������ ����
            if (stickPower != null)
            {
                // Lerp�� ����Ͽ� ���� ������ ���� X ��ǥ ����
                stickPower.transform.localPosition = new Vector3(Mathf.Lerp(minStickPowerX, maxStickPowerX, forceRatio), stickPower.transform.localPosition.y, stickPower.transform.localPosition.z);
            }
        }



        // �巡�� ���� �� �� ���ؼ� �߻� �Ǵ� �߻� ���
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            endMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = startMousePos - endMousePos;

            // �ּ� �巡�� ���� üũ
            if (dragVector.magnitude < minLaunchDragMagnitude)
            {
                // �߻� ��� �� �ʱ�ȭ �۾�
                if (VisualUI != null)
                {
                    VisualUI.SetActive(false); // UI ��Ȱ��ȭ
                }
                isDragging = false; // �巡�� ���� ����
                hasReachedMinDrag = false; // ���� �巡�׸� ���� �ʱ�ȭ
                return; // �Լ� ����
            }

            // �ִ� �� ����
            if (dragVector.magnitude > maxForce)
            {
                dragVector = dragVector.normalized * maxForce;
            }

            // ���� ���� �߻� (���� ���콺�� ����� ������ �ݴ�� �߻�)
            rb.AddForce(dragVector * forceMultiplier, ForceMode2D.Impulse);

            if (VisualUI != null)
            {
                VisualUI.SetActive(false); // VisualUI ��Ȱ��ȭ
            }

            isDragging = false; // isDragging ���� �������� ����
            hasReachedMinDrag = false; // ���� �巡�׸� ���� �ʱ�ȭ
            GameManager.canPlay = false; // ���������� �߻������Ƿ� �÷��� �Ұ��� ���·� ����
        }
    }
}