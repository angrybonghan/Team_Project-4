using UnityEngine;

public class BallStopper : MonoBehaviour
{


    public float deceleration = 0.5f; // �ڿ��� �ӵ� ����
    public float energyLossFactor = 0.8f; // �浹 �� �ӵ��� ����
    Rigidbody2D rb;

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

        // �浹 �� ������ �ս� (������ �ٲٴ� ������ ����)
        /*
        void OnCollisionEnter2D(Collision2D collision)
        {
            rb.velocity *= energyLossFactor;
        }
        */
    }
}
