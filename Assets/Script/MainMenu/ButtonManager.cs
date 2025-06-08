using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [Header("�� ��ȯ �ִϸ��̼� ������")]
    public GameObject animationPrefabs;

    public void onStartButtonClicked()
    {
        GameObject Animation = Instantiate(animationPrefabs, transform.position, transform.rotation);
        StartCoroutine(GoToSelectScene());
    }

    IEnumerator GoToSelectScene()
    {
        yield return Sleep(0.6);
        SceneManager.LoadScene("SelectStage");
    }

    IEnumerator Sleep(double SleepSeconds)
    {
        yield return new WaitForSeconds((float)SleepSeconds);
    }
}
