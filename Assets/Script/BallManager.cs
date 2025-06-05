using UnityEngine;

public class BallManager : MonoBehaviour
{
    [Header("�ڿ� �ӵ� ���Ұ�")]
    public float deceleration = 0.5f; // �ڿ��� �ӵ� ����
    [Header("������ ���� ��� ��������Ʈ")]
    public Sprite[] ballSprites;

    //public float energyLossFactor = 0.8f; // �浹 �� �ӵ��� ����

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // ����
        if (rb.velocity.magnitude > 0.5f)
        {
            rb.velocity *= (1 - deceleration * Time.deltaTime);

        }
        else // ����
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // �浹 �� ������ �ս� (������ �ٲٴ� ������ ����)
        /*
        void OnCollisionEnter2D(Collision2D collision)
        {
            rb.velocity *= energyLossFactor;
        }
        */
    }

    public void SetSprite(int level)
    {
        if (level > 8)
        {
            return;
        }
        spriteRenderer.sprite = ballSprites[level-1];
    }
}
