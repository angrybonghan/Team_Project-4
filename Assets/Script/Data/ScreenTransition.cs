using System.Collections;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTransition : MonoBehaviour
{
    public static ScreenTransition Instance { get; private set; }

    public static bool isTransitioning = true;
    private float operatingFrequency = 30; // 작동 주기
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
        isTransitioning=true;
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
        isTransitioning = false;
    }

    public static void Goto(string targetScene, float fadeInTime, float fadeOutTime)
    {
        if (isTransitioning)
        {
            return;
        }

        isTransitioning = true;
        DataManager.isGameActionable = false;

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


        switch (targetScene)
        {
            case "SelectStage":
                DataManager.isGameActionable = true;
                break;
        }



        isTransitioning = false;
        Instance.currentTransitionCoroutine = null;
    }
}
