using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderWithDelay : MonoBehaviour
{
    public string nextSceneName;      // ��ȯ�� �� �̸�
    public float delaySeconds = 0.5f;   // ���� �ð� (��)

    // ��ư���� �� �޼��带 ȣ���ϼ���
    public void LoadNextSceneWithDelay()
    {
        StartCoroutine(LoadSceneAfterDelay());
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);
        SceneManager.LoadScene(nextSceneName);
    }
}

