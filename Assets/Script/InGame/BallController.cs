using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("���� ����")]
    public float forceMultiplier = 5f; // �� �Ŀ� ����
    public float maxForce = 4f; // �ִ� �� �Ŀ�
    public float minLaunchDragMagnitude = 0.25f; // �ּ� ���콺 �巡�� ����

    [Header("�ð��� ��� ����")]
    public GameObject VisualUI; // �巡���Ҷ��� ���� ������Ʈ���� ���� UI
    public GameObject arrowIndicator; // ȭ��ǥ
    public GameObject dotLineIndicator; // ȭ��ǥ ������ ���� ����
    public GameObject stick; // �籸 �����
    public GameObject stickPower; // �籸 ����⿡ ǥ�õ� �Ŀ� ������
    public float minStickPowerX = -0.5f; //�ְ�, �ּ� ����� �Ŀ� ��ġ
    public float maxStickPowerX = -2.1f;
    public float arrowDistance = 0.75f; // ȭ��ǥ�� �ִ�� �þ �� �ִ� �Ÿ�
    public float arrowSensitivity = 1.0f; // ȭ��ǥ �þ�� ����
    public float dotLineDistance = 0.3f; // �� ��������Ʈ ���� ���� (���ϸ� �ʹ� �����)

    // Raycast�� ���� ���̾� ����ũ
    [Header("Raycast ����")]
    public LayerMask obstacleLayer; // ����ĳ��Ʈ ���̾�

    [Header("���� ���� ���� ��ȭ")]
    public Color minForceColor = Color.green;
    public Color maxForceColor = Color.red;

    private Rigidbody2D rb;

    private Vector2 startMousePos;
    private Vector2 endMousePos;

    private bool isDragging = false;
    private bool hasReachedMinDrag = false;

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
            VisualUI.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!GameManager.canPlay)
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
            // ���콺 ��ġ�� ���ϰ�, ���� ��ġ���� ���� �巡�� �Ÿ��� ����
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = startMousePos - currentMousePos;
            float rawDragMagnitude = dragVector.magnitude;

            
            if (rawDragMagnitude >= minLaunchDragMagnitude) // �ѹ� �ּ� ���̸� �Ѿ����� Ȯ��
            {
                hasReachedMinDrag = true;
            }
            else if (hasReachedMinDrag && rawDragMagnitude < minLaunchDragMagnitude) // �ּ� ���̸� �Ѱ� �ٽ� ���ƿ��� ���
            {
                // UI ���� ��¼����¼�� ����
                isDragging = false;
                hasReachedMinDrag = false;

                if (VisualUI != null) 
                {
                    VisualUI.SetActive(false);
                }
                return;
            }

            float currentDragMagnitude = Mathf.Min(rawDragMagnitude, maxForce); // ���콺 �巡�� ����
            float forceRatio = currentDragMagnitude / maxForce;
            //Debug.Log($"currentDragMagnitude = {currentDragMagnitude}");
            //Debug.Log($"forceRatio = {forceRatio}");

            Vector2 clampedDirection = dragVector.normalized;
            float angle = Mathf.Atan2(clampedDirection.y, clampedDirection.x) * Mathf.Rad2Deg;
            Color lerpedColor = Color.Lerp(minForceColor, maxForceColor, forceRatio);

            // ����ĳ��Ʈ
            Vector2 raycastOrigin = transform.position; // ���� ��ġ ����
            float raycastDistance = Mathf.Min(currentDragMagnitude * forceMultiplier * arrowSensitivity, arrowDistance * forceMultiplier);
            // �� ���� ���̿� ����Ʈ ����

            // ���� �߽�!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!@@@@
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, clampedDirection, raycastDistance, obstacleLayer);

            Vector3 arrowTargetPosition; // ȭ��ǥ�� ���� ��ġ, �� �������� �ִ� ���� ��ġ
            float actualLineLength; // ���� ���� ����
            // ���⼭ ���� �����ϴ� ���� �𸣰����� �̽��� DM���� ����� ����
            // �ƴϴ� �׳� ���⼭ ���ϰ���
            // �ؿ� if �ȿ��� �ΰ��� �������� �۵��� �ؾ����ݾ� �� ��@���ʾ�~~

            if (hit.collider != null)
            {
                // �浹 �������� ����ϴ�.
                arrowTargetPosition = hit.point;
                actualLineLength = (hit.point - raycastOrigin).magnitude;
                //Debug.Log($"ȭ��ǥ�� {hit.collider.name}�� �浹��"); �� �� �׽�Ʈ�� �α�
            }
            else
            {
                // �浹���� ������ �ִ�ġ�� ����
                arrowTargetPosition = raycastOrigin + (Vector2)clampedDirection * raycastDistance;
                actualLineLength = raycastDistance;
            }

            // ȭ��ǥ ����
            if (arrowIndicator != null)
            {
                arrowIndicator.SetActive(true);
                arrowIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
                arrowIndicator.transform.position = arrowTargetPosition; // ����ĳ��Ʈ ������ġ

                SpriteRenderer arrowRenderer = arrowIndicator.GetComponent<SpriteRenderer>();
                if (arrowRenderer != null)
                {
                    arrowRenderer.color = lerpedColor;
                }
            }

            // ���� ����
            if (dotLineIndicator != null)
            {
                dotLineIndicator.SetActive(true);
                dotLineIndicator.transform.position = transform.position;
                dotLineIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);

                SpriteRenderer dotRenderer = dotLineIndicator.GetComponent<SpriteRenderer>();
                if (dotRenderer != null)
                {
                    // ���� ���̴� ���� ���� ���̸� ����մϴ�.
                    dotRenderer.size = new Vector2(actualLineLength * dotLineDistance, dotRenderer.size.y);
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
                stickPower.transform.localPosition = new Vector3(Mathf.Lerp(minStickPowerX, maxStickPowerX, forceRatio), stickPower.transform.localPosition.y, stickPower.transform.localPosition.z);
            }
        }

        // �巡�� ����
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

            rb.AddForce(dragVector * forceMultiplier, ForceMode2D.Impulse);

            if (VisualUI != null)
            {
                VisualUI.SetActive(false);
            }

            isDragging = false;
            hasReachedMinDrag = false;
            GameManager.canPlay = false;
        }
    }
}