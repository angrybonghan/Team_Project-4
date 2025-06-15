using System.Collections;
using UnityEngine;

public class AfterImageEffect : MonoBehaviour
{

    [Header("레벨에 따른 모양 스프라이트")]
    public Sprite[] ballSprites;
    [Header("동작 설정")]
    public float runTime;
    public float operatingFrequency = 30f;


    private int level;
    private SpriteRenderer spriteRenderer;
    

    private Vector3 initialScale;
    private Color initialColor;


    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        
        level = GameManager.ballNumber-1;
        spriteRenderer.sprite = ballSprites[level];

        initialScale = transform.localScale;
        initialColor = spriteRenderer.color;
    }

    void Start()
    {
        float sleepTime = runTime / operatingFrequency;
        float alphaAdditions = initialColor.a / operatingFrequency;
        Vector3 sizeAdditions = initialScale / operatingFrequency;

        StartCoroutine(AfterImage(sleepTime, alphaAdditions, sizeAdditions));
    }

    IEnumerator AfterImage(float sleepTime, float alphaAdditions, Vector3 sizeAdditions)
    {
        for (int i = 0; i < operatingFrequency; i++)
        {
            Color currentColor = spriteRenderer.color;
            currentColor.a -= alphaAdditions;
            spriteRenderer.color = currentColor;

            initialScale = transform.localScale;
            initialScale -= sizeAdditions;
            transform.localScale = initialScale;


            yield return new WaitForSeconds(sleepTime);
        }

        Destroy(gameObject);
    }

    /*

    public void StartFadeOutAndShrink(float runTime)
    {
        // 이미 효과가 진행 중이거나, SpriteRenderer가 없으면 실행하지 않습니다.
        if (spriteRenderer == null || !gameObject.activeInHierarchy)
        {
            Debug.LogWarning("FadeOutAndShrink: SpriteRenderer가 없거나 오브젝트가 비활성화되어 효과를 시작할 수 없습니다.");
            return;
        }

        // 만약 이 오브젝트에 이미 동일한 코루틴이 실행 중이라면 중지하고 새로 시작하는 것이 좋습니다.
        // 예를 들어: StopAllCoroutines(); // 이 스크립트의 모든 코루틴 중지
        // 또는 특정 코루틴 참조를 저장하여 StopCoroutine(specificCoroutine);

        StartCoroutine(FadeOutAndShrink(runTime));
    }


    private IEnumerator FadeOutAndShrink(float runTime)
    {
        // runTime이 0이거나 음수이면 즉시 투명/축소 후 파괴
        if (runTime <= 0f)
        {
            SetSpriteProperties(0f, Vector3.zero); // 알파 0, 스케일 0
            Debug.Log("FadeOutAndShrink: runTime이 0이므로 즉시 효과 적용 후 오브젝트 파괴.");
            Destroy(gameObject);
            yield break;
        }

        // 페이드 및 축소의 시작 상태를 현재 상태로 설정
        float currentAlpha = spriteRenderer.color.a;
        Vector3 currentScale = transform.localScale;

        float timeElapsed = 0f;
        float stepDuration = runTime / operatingFrequency; // 각 단계의 지속 시간

        // Debug.Log($"FadeOutAndShrink 시작: runTime={runTime}, initialAlpha={currentAlpha}, initialScale={currentScale}");

        while (timeElapsed < runTime)
        {
            timeElapsed += stepDuration;
            // 시간에 따른 비율 (0에서 1로)
            float normalizedTime = Mathf.Min(timeElapsed / runTime, 1f); // 1f를 넘지 않도록 보장

            // 1. 알파 값 계산: 현재 알파에서 0으로 Lerp
            float targetAlpha = Mathf.Lerp(currentAlpha, 0f, normalizedTime);

            // 2. 스케일 값 계산: 현재 스케일에서 Vector3.zero로 Lerp
            // Lerp는 벡터에도 적용 가능
            Vector3 targetScale = Vector3.Lerp(currentScale, Vector3.zero, normalizedTime);

            // 스프라이트의 알파와 오브젝트의 스케일 동시에 적용
            SetSpriteProperties(targetAlpha, targetScale);

            yield return new WaitForSeconds(stepDuration);
        }

        // ⭐️⭐️⭐️ 최종 상태 보정: 부동 소수점 오차 방지를 위해 최종적으로 정확한 값 설정 ⭐️⭐️⭐️
        SetSpriteProperties(0f, Vector3.zero); // 알파 0, 스케일 0

        Debug.Log("FadeOutAndShrink: 효과 완료. 오브젝트 파괴.");
        Destroy(gameObject); // 효과 완료 후 이 스크립트가 붙은 GameObject 파괴
    }

    /// <summary>
    /// SpriteRenderer의 알파와 GameObject의 스케일을 동시에 설정하는 헬퍼 메서드.
    /// </summary>
    /// <param name="alpha">설정할 알파 값 (0.0 ~ 1.0)</param>
    /// <param name="scale">설정할 스케일 Vector3</param>
    private void SetSpriteProperties(float alpha, Vector3 scale)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
        transform.localScale = scale;
    }

    // 오브젝트 생성 후 바로 이 효과를 적용하고 싶다면 (예: 폭발 파편)
    // 이 스크립트가 붙을 오브젝트에 SpriteRenderer가 반드시 있어야 합니다.
    // 또한, 스프라이트의 Order in Layer를 조절하여 다른 오브젝트보다 위에 보이게 할 수 있습니다.
    */
}