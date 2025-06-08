using UnityEngine;

public class EightBallMovement : MonoBehaviour
{
    [Header("�ڿ� �ӵ� ���Ұ�")]
    public float deceleration = 0.5f; // �ڿ��� �ӵ� ����

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
    }
}
