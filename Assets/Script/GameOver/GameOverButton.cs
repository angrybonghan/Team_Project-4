using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverButton : MonoBehaviour
{
    public void onRestartButtonClicked()
    {
        SceneManager.LoadScene("SelectStage");
    }

    public void onStageButtonClicked()
    {
        SceneManager.LoadScene("SelectStage");
    }
}
