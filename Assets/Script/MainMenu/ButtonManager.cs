using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [Header("신 전환 애니메이션 프리팹")]
    public GameObject animationPrefabs;

    public void onStartButtonClicked()
    {
        GameObject Animation = Instantiate(animationPrefabs, transform.position, transform.rotation);
        ScreenTransition.Goto("SelectStage", 0.6f, 0.6f);
    }
}
