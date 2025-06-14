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
        ScreenTransition.Goto("SelectStage", 0.6f, 0.6f);
    }
}
