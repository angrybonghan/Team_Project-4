using System.Collections;
using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private float runTime = 0.25f; // ������ �ð�
    private float operatingFrequency = 25; // �۵� �ֱ�
    private float maxSize = 5f; // �ִ� ������


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Effect());
    }
    
    IEnumerator Effect()
    {
        float sleepTime = runTime / operatingFrequency;
        float size = 0f;
        float sizeAdditions = maxSize/operatingFrequency;
        float alphaAdditions = 1f / operatingFrequency;


        for (int i = 0; i < operatingFrequency; i++)
        {
            size += sizeAdditions;
            transform.localScale = new Vector3(size, size, size);

            Color currentColor = spriteRenderer.color;
            currentColor.a -= alphaAdditions;
            spriteRenderer.color = currentColor;

            yield return new WaitForSeconds(sleepTime);
        }

        Destroy(gameObject);
    }
}
