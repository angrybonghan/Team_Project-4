using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [Header("신 전환 애니메이션 프리팹")]
    public GameObject animationPrefabs;

    public void onStartButtonClicked()
    {
        if (ScreenTransition.isTransitioning)
        {
            return;
        }

        GameObject Animation = Instantiate(animationPrefabs, transform.position, transform.rotation);

        if (DataManager.GetPlayCutsceneOP())
        {
            ScreenTransition.Goto("Cutscene_OP", 0.6f, 0.6f);
        }
        else
        {
            ScreenTransition.Goto("SelectStage", 0.6f, 0.6f);
        }
        
    }

    public void onCreditButtonClicked()
    {
        if (ScreenTransition.isTransitioning)
        {
            return;
        }

        ScreenTransition.Goto("CreditScene", 0.6f, 0.6f);
    }

    public void QuitGameButton()
    {
        QuitGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("게임 종료 (에디터 모드)");
#else
        // 빌드된 게임에서 실행 중일 때
        Application.Quit();
        Debug.Log("게임 종료 (빌드된 게임)");
#endif
    }
}
