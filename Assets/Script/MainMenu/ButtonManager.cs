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
}
