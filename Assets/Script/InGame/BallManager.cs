using UnityEngine;

public class BallManager : MonoBehaviour
{
    [Header("������ ���� ��� ��������Ʈ")]
    public Sprite[] ballSprites;
    [Header("�� ���� �ִϸ��̼� ������")]
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
            Debug.LogError("�迭�� �´� ���ڰ� �ƴ�");
            return;
        }
        spriteRenderer.sprite = ballSprites[level-1];
    }

    public void PlayMergeAnimation()
    {
        GameObject Animation = Instantiate(animationPrefabs, transform.position, transform.rotation);
    }
}
