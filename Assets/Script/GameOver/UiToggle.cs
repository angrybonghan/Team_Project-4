using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UiToggle : MonoBehaviour
{
    public AudioClip GunShot;
    public AudioClip FingerSnap;

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
            SoundManager.PlaySound(FingerSnap, 0.5f, 1.2f);
            yield return Sleep(0.7f);
            UI.SetActive(true);
        }
        else
        {
            yield return Sleep(0.5f);
            SoundManager.PlaySound(GunShot, 1f, 1f);
            yield return Sleep(0.5f);
            UI.SetActive(true);
        }
    }

    IEnumerator Sleep(double SleepSeconds)
    {
        yield return new WaitForSeconds((float)SleepSeconds);
    }
}
