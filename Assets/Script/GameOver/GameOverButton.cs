using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverButton : MonoBehaviour
{
    public void onRestartButtonClicked()
    {
        int sceneNumber = DataManager.GetPreviousLevel();
        ScreenTransition.Goto("Stage_" + sceneNumber, 0.5f, 0.5f);
    }

    public void onStageButtonClicked()
    {
        ScreenTransition.Goto("SelectStage", 0.5f, 0.5f);
    }
}
