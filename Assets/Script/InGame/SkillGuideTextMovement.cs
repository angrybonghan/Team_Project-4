using System.Collections;
using TMPro;
using UnityEngine;

public class SkillGuideTextMovement : MonoBehaviour
{
    [Header("글자")]
    public TextMeshPro guideText;

    private float moveSpeed = 20f;
    private float stayTime = 5; // 유지 시간

    Vector3 startScale;
    Vector3 startPosition;
    Quaternion startRotation;

    Vector3 targetScale;
    Quaternion targetRotation;
    Vector3 targetPosition;

    void Start()
    {
        guideText.text = SkillGuideManager.guideText;

        startScale = transform.localScale;
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;

        targetScale = new Vector3(1f, 1f, 1f);
        targetRotation = Quaternion.Euler(0, 0, 0);
        targetPosition = new Vector3(3, 0, 0);

        StartCoroutine(Movement());
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, moveSpeed*Time.deltaTime);
        transform.localPosition = Vector3.Lerp(transform.localPosition , targetPosition, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
    }


    IEnumerator Movement()
    {
        
        yield return new WaitForSeconds(stayTime);

        targetScale = startScale;
        targetRotation = Quaternion.Euler(0, 0, 90);
        targetPosition = startPosition;

        yield return new WaitForSeconds(10/ moveSpeed);

        Destroy(transform.parent.gameObject);
    }

}
