using UnityEngine;

public class ShockwaveBall : MonoBehaviour
{
    [Header("충격파 설정")]
    public float shockwaveRadius = 0.5f;
    public float pushPower = 2f;

    [Header("발동 이펙트")]
    public GameObject effectPrefabs;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EmitShockwave();
        GameObject Effect = Instantiate(effectPrefabs, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void EmitShockwave()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, shockwaveRadius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == gameObject)
            {
                continue;
            }
            BallDeceleration ballDecelScript = hitCollider.GetComponent<BallDeceleration>();
            if (ballDecelScript != null)
            {
                Rigidbody2D rb = hitCollider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 directionToTarget = (hitCollider.transform.position - transform.position).normalized;
                    rb.AddForce(directionToTarget * pushPower, ForceMode2D.Impulse);
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shockwaveRadius);
    }
}
