using UnityEngine;
using System.Collections;

public class BulletBallBullet : MonoBehaviour
{
    [Header("처음 앞으로 나갈 값")]
    public float initialMoveDistance = 0.105f;
    [Header("총알 강도 (밀치는 값)")]
    public float moveSpeed = 10f;
    [Header("총알 깜빡거리는 시간")]
    public float destroyDelay = 0.1f;

    [Header("레이케스팅 레이어")]
    public LayerMask obstacleLayer;
    [Header("맞는 애니메이션 프리팹")]
    public GameObject hitEffectPrefab;

    [Header("애니메이션 시간, 주기")]
    public float introAnimationDuration = 0.35f;
    public int animationSteps = 30;

    private LineRenderer lineRenderer;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        if (spriteRenderer == null)
        {
            Debug.LogWarning("BulletBallBullet 오브젝트에 SpriteRenderer 컴포넌트가 없습니다. 인트로 애니메이션이 작동하지 않을 수 있습니다.", this);
        }

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        //Gradient gradient = new Gradient();
        //gradient.SetKeys(
        //    new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.yellow, 1.0f) },
        //    new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        //);
        //lineRenderer.colorGradient = gradient;

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false; // 처음에는 비활성화
    }

    void Start()
    {
        StartCoroutine(BeginBulletAction());
    }

    public IEnumerator BeginBulletAction()
    {
        if (spriteRenderer != null)
        {
            Quaternion initialTargetRotation = transform.rotation;

            float randomZOffset = Random.Range(0.0f, 360.0f);
            transform.rotation = Quaternion.Euler(initialTargetRotation.eulerAngles.x, initialTargetRotation.eulerAngles.y, initialTargetRotation.eulerAngles.z + randomZOffset);
            Quaternion startAnimationRotation = transform.rotation;

            Color startColor = spriteRenderer.color;
            startColor.a = 0f;
            spriteRenderer.color = startColor;

            float sleepTimePerStep = introAnimationDuration / animationSteps;

            for (int i = 0; i <= animationSteps; i++)
            {
                float t = (float)i / animationSteps;

                Color currentColor = spriteRenderer.color;
                currentColor.a = Mathf.Lerp(0f, 1f, t);
                spriteRenderer.color = currentColor;

                transform.rotation = Quaternion.Slerp(startAnimationRotation, initialTargetRotation, t);

                yield return new WaitForSeconds(sleepTimePerStep);
            }

            Color finalColor = spriteRenderer.color;
            finalColor.a = 0f;
            spriteRenderer.color = finalColor;
            transform.rotation = initialTargetRotation;
        }
        // ========================== 인트로 애니메이션 끝 ==========================

        // 초기 이동
        Vector3 initialMoveDirection = transform.right;
        transform.position += initialMoveDirection * initialMoveDistance;

        Vector3 raycastStartPoint = transform.position;

        // 레이 빨싸!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!@
        RaycastHit2D hit = Physics2D.Raycast(raycastStartPoint, initialMoveDirection, Mathf.Infinity, obstacleLayer);

        if (hit.collider != null) // 무언가에 맞았을 경우
        {
            lineRenderer.SetPosition(0, raycastStartPoint);
            lineRenderer.SetPosition(1, hit.point);
            lineRenderer.enabled = true;

            Rigidbody2D hitRigidbody = hit.collider.GetComponent<Rigidbody2D>();
            if (hitRigidbody != null)
            {
                hitRigidbody.AddForce(initialMoveDirection * moveSpeed, ForceMode2D.Impulse);
            }

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hit.point, transform.rotation);
            }

            if (hit.collider.CompareTag("PlayerBall"))
            {
                GameManager.attemptsLeft--;
            }
        }
        else // 아무것도 맞히지 못했을 경우
        {
            lineRenderer.SetPosition(0, raycastStartPoint);
            lineRenderer.SetPosition(1, raycastStartPoint + initialMoveDirection * 100f);
            lineRenderer.enabled = true;
        }

        // 대기 후 오브젝트 파괴
        CameraMovement.Shake(0.1f, 0.2f, 0.05f);
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}