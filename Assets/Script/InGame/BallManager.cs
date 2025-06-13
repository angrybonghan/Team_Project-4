using UnityEngine;

public class BallManager : MonoBehaviour
{
    [Header("레벨에 따른 모양 스프라이트")]
    public Sprite[] ballSprites;
    [Header("공 병합 애니메이션 프리팹")]
    public GameObject animationPrefabs;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(int level)
    {
        if (level > 8 || level < 0)
        {
            Debug.LogError("배열에 맞는 숫자가 아님");
            return;
        }
        spriteRenderer.sprite = ballSprites[level-1];
    }

    public void PlayMergeAnimation()
    {
        GameObject Animation = Instantiate(animationPrefabs, transform.position, transform.rotation);
    }
}
