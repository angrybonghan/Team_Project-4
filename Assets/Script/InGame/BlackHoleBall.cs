using System.Collections;
using UnityEngine;

public class BlackHoleBall : MonoBehaviour
{
    [Header("끌어당기는 힘의 크기")]
    public float attractionForce = 1f;

    [Header("범위 반지름")]
    public float attractionRadius = 1;

    [Header("최소 시간, 최대 시간")]
    public float minAttractionDuration = 0.5f;
    public float maxAttractionDuration = 1f;

    [Header("이펙트 프리팹")]
    public GameObject BlackHoleEffect;

    private bool isAttracting = false;
    private Collider2D[] hitColliders;
    private Coroutine currentAttractionCoroutine;

    public void BlackHoleStart() // 시작
    {
        if (isAttracting)
        {
            return;
        }

        isAttracting = true;

        if (currentAttractionCoroutine != null)
        {
            StopCoroutine(currentAttractionCoroutine);
        }
        currentAttractionCoroutine = StartCoroutine(AttractionRoutine());
        StartCoroutine(Effect());
    }

    public void BlackHoleStop()
    {
        if (currentAttractionCoroutine != null)
        {
            StopCoroutine(currentAttractionCoroutine);
        }
        isAttracting = false;
    }

    private IEnumerator AttractionRoutine()
    {
        // 랜덤한 작동 시간
        float attractionDuration = Random.Range(minAttractionDuration, maxAttractionDuration);

        // 찾아보니까 Time.time 이라는 유니티 자동 타이머가 있었음 (float임)
        // 게임 시작으로부터 경과한 시간을 알아서 째는듯.

        while (0 < attractionDuration)
        {
            attractionDuration-=Time.deltaTime;

            // 원형 범위 내 모든 Collider2D
            // "OverlapCircleAll" 진짜 개꿀
            hitColliders = Physics2D.OverlapCircleAll(transform.position, attractionRadius);

            foreach (Collider2D hitCollider in hitColliders)
            {
                // 자기 자신은 작동안할거임
                if (hitCollider.gameObject == gameObject)
                {
                    continue;
                }

                Rigidbody2D otherRb = hitCollider.GetComponent<Rigidbody2D>();

                if (otherRb == null)
                {
                    continue;
                }
                
                // 방향 계산
                Vector2 directionToBlackHole = (Vector2)transform.position - otherRb.position;
                // 거리 제곱을 사용하여 거리가 멀수록 힘이 약해짐
                float distance = directionToBlackHole.magnitude;

                if (distance > 0.1f) // 너무 가까우면 힘이 너무 큰 값에 수렴해서 게임터짐
                {
                    // 힘 계산 (거리에 반비례)
                    float forceMagnitude = attractionForce / distance;
                    // float forceMagnitude = attractionForce / (distance * distance); // 제곱에 반비례 (더 강한 효과)

                    // 힘 적용 (Impulse로 하면 게임 터질거같아서 안했음)
                    otherRb.AddForce(directionToBlackHole.normalized * forceMagnitude);
                }
            }
            yield return null; // 다음 프레임까지 대기
        }

        // 지정된 시간이 지나면 중지
        isAttracting = false;
    }

    private IEnumerator Effect()
    {
        while (isAttracting)
        {
            // 아 뭐야 이래도 되는듯?
            Instantiate(BlackHoleEffect, transform.position, transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }
    }


    // 개발 편의를 위한 시각화 (유니티 에디터에서만 보임)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }
}