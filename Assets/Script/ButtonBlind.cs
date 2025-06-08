using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBlind : MonoBehaviour
{
    public GameObject buttonObject;
    public float delaySeconds = 1.6f;

    void Start()
    {
        if (buttonObject != null)
        {
            buttonObject.SetActive(false);
            StartCoroutine(ShowButton());
        }
    }

    IEnumerator ShowButton()
    {
        yield return new WaitForSeconds(delaySeconds);
        buttonObject.SetActive(true);
    }
}
