using UnityEngine;
using System.Collections;

public class UiToggle : MonoBehaviour
{
    public GameObject UI;

    void Start()
    {
        StartCoroutine(UIToggle());
    }

    IEnumerator UIToggle()
    {
        yield return Sleep(1.1f);
        SoundManager.PlaySound(5);
        yield return Sleep(0.7f);
        UI.SetActive(true);
    }

    IEnumerator Sleep(double SleepSeconds)
    {
        yield return new WaitForSeconds((float)SleepSeconds);
    }
}
