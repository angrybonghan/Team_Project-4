using UnityEngine;

public class BallDeceleration : MonoBehaviour
{
    [Header("자연 속도 감소값")]
    public float deceleration = 0.5f; // 자연적 속도 감소
    private AudioClip BallHitClip;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        BallHitClip = DataManager.BallHit;
    }

    void Update()
    {
        // 감속
        if (rb.velocity.magnitude > 0.25f)
        {
            rb.velocity *= (1 - deceleration * Time.deltaTime);

        }
        else // 정지
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        BallDeceleration otherBallDeceleration = collision.gameObject.GetComponent<BallDeceleration>();
        if (otherBallDeceleration != null)
        {
            // 2. 두 오브젝트 중 한 쪽만 작동하도록 조건 부여
            // 예를 들어, 현재 오브젝트의 인스턴스 ID가 상대방 오브젝트의 인스턴스 ID보다 작을 경우에만 작동
            // 이는 충돌한 두 오브젝트 중 하나만 이 로직을 실행하도록 보장하는 일반적인 방법입니다.
            if (this.gameObject.GetInstanceID() < otherBallDeceleration.gameObject.GetInstanceID())
            {
                SoundManager.PlaySound(BallHitClip,0.25f,Random.Range(0.5f, 1.5f));
            }
        }
    }
}
