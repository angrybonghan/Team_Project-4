using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UiToggle : MonoBehaviour
{
    private bool boss = false;


    public GameObject UI;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "GameOverToBoss")
        {
            boss = true;
        }

        StartCoroutine(UIToggle());
    }

    IEnumerator UIToggle()
    {
        if (!boss)
        {
            yield return Sleep(1.1f);
            // 손가락 스냅 사운드
            yield return Sleep(0.7f);
            UI.SetActive(true);
        }
        else
        {
            yield return Sleep(1f);
            UI.SetActive(true);
        }
    }

    IEnumerator Sleep(double SleepSeconds)
    {
        yield return new WaitForSeconds((float)SleepSeconds);
    }
}
