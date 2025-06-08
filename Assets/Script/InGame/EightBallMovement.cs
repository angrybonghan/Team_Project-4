using UnityEngine;

public class EightBallMovement : MonoBehaviour
{
    [Header("자연 속도 감소값")]
    public float deceleration = 0.5f; // 자연적 속도 감소

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
    }
}
