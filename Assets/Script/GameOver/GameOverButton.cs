using UnityEngine;

public class GameOverButton : MonoBehaviour
{
    public void onRestartButtonClicked()
    {
        int sceneNumber = DataManager.GetPreviousLevel();
        Debug.Log(sceneNumber);
        ScreenTransition.Goto("Stage_" + sceneNumber, 0.5f, 0.5f);
    }

    public void onStageButtonClicked()
    {
        ScreenTransition.Goto("SelectStage", 0.5f, 0.5f);
    }
}
