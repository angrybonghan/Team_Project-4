using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTransition : MonoBehaviour
{
    public static ScreenTransition Instance { get; private set; }

    private float operatingFrequency = 30; // 작동 주기
    private static bool isActive = true;
    private SpriteRenderer spriteRenderer;
    private Coroutine currentTransitionCoroutine;

    void Awake()
    {
        if (Instance == null) // 신 변경에도 유지하는 싱글톤
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        StartCoroutine(FadeOut(1f));
    }

    
    IEnumerator FadeOut(float runTime)
    {
        isActive=true;
        float sleepTime = runTime / operatingFrequency;
        float alphaAdditions = 1f / operatingFrequency;
        Instance.transform.position = Vector3.zero;

        for (int i = 0; i < operatingFrequency; i++)
        {
            Color currentColor = spriteRenderer.color;
            currentColor.a -= alphaAdditions;
            spriteRenderer.color = currentColor;

            yield return new WaitForSeconds(sleepTime);
        }
        isActive = false;
    }

    public static void Goto(string targetScene, float fadeInTime, float fadeOutTime)
    {
        if (isActive)
        {
            return;
        }
        isActive = true;
        Instance.currentTransitionCoroutine = Instance.StartCoroutine(Instance.Transition(targetScene, fadeInTime, fadeOutTime));
    }

    IEnumerator Transition(string targetScene, float fadeInTime, float fadeOutTime)
    {
        float sleepTime = fadeInTime / operatingFrequency;
        float alphaAdditions = 1f / operatingFrequency;
        Instance.transform.position = Vector3.zero;
        for (int i = 0; i < operatingFrequency; i++)
        {
            Color currentColor = spriteRenderer.color;
            currentColor.a += alphaAdditions;
            spriteRenderer.color = currentColor;

            yield return new WaitForSeconds(sleepTime);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        sleepTime = fadeOutTime / operatingFrequency;
        alphaAdditions = 1f / operatingFrequency;
        Instance.transform.position = Vector3.zero;
        for (int i = 0; i < operatingFrequency; i++)
        {
            Color currentColor = spriteRenderer.color;
            currentColor.a -= alphaAdditions;
            spriteRenderer.color = currentColor;

            yield return new WaitForSeconds(sleepTime);
        }

        transform.position = new Vector3(9999999999999, 9999999999999999, transform.position.z);
        isActive = false;
        Instance.currentTransitionCoroutine=null;
    }
}
