using System.Collections;
using UnityEngine;

public class BlackHoleEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private float runTime = 0.2f; // 진행할 시간
    private float operatingFrequency = 40; // 작동 주기


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Effect());
    }




    IEnumerator Effect()
    {

        Vector3 startScale = transform.localScale;
        UnityEngine.Color StartColor = spriteRenderer.color;
        Vector3 targetScale = new Vector3(0f, 0f, 0f);

        Quaternion startRotation = transform.localRotation;
        UnityEngine.Color targetColor = new UnityEngine.Color(StartColor.r, StartColor.g, StartColor.b, 1f);
        Quaternion targetRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, 225f);

        float timeElapsed = 0f;
        float stepDuration = 1f / operatingFrequency; // 각 단계의 지속 시간

        while (timeElapsed < runTime)
        {
            timeElapsed += stepDuration;
            float normalizedTime = Mathf.Min(timeElapsed / runTime, 1f); // 0에서 1까지의 진행
            transform.localScale = Vector3.Lerp(startScale, targetScale, normalizedTime);
            spriteRenderer.color = UnityEngine.Color.Lerp(StartColor, targetColor, normalizedTime);
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, normalizedTime); // (Lerp도 가능)

            yield return new WaitForSeconds(stepDuration);
        }

        Destroy(gameObject);
    }
}
