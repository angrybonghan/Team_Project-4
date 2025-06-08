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
        yield return Sleep(1.8);
        UI.SetActive(true);
    }

    IEnumerator Sleep(double SleepSeconds)
    {
        yield return new WaitForSeconds((float)SleepSeconds);
    }
}
