using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderWithDelay : MonoBehaviour
{
    public string nextSceneName;      // 전환할 씬 이름
    public float delaySeconds = 0.5f;   // 지연 시간 (초)

    // 버튼에서 이 메서드를 호출하세요
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

