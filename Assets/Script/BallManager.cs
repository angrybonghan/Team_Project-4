using UnityEngine;

public class BallManager : MonoBehaviour
{
    [Header("자연 속도 감소값")]
    public float deceleration = 0.5f; // 자연적 속도 감소
    [Header("레벨에 따른 모양 스프라이트")]
    public Sprite[] ballSprites;
    [Header("공 병합 애니메이션 프리팹")]
    public GameObject animationPrefabs;

    //public float energyLossFactor = 0.8f; // 충돌 시 속도의 감소

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 감속
        if (rb.velocity.magnitude > 0.5f)
        {
            rb.velocity *= (1 - deceleration * Time.deltaTime);

        }
        else // 정지
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // 충돌 시 에너지 손실 (재질을 바꾸는 것으로 변경)
        /*
        void OnCollisionEnter2D(Collision2D collision)
        {
            rb.velocity *= energyLossFactor;
        }
        */
    }

    public void SetSprite(int level)
    {
        if (level > 8 || level < 0 || level==null)
        {
            Debug.LogError("배열에 맞는 숫자가 아니거나, NULL");
            return;
        }
        spriteRenderer.sprite = ballSprites[level-1];
    }

    public void PlayMergeAnimation()
    {
        GameObject Animation = Instantiate(animationPrefabs, transform.position, transform.rotation);
    }
}
