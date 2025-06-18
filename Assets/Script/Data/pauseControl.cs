using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseControl : MonoBehaviour
{
    public static pauseControl Instance { get; private set; }


    public static bool isGuidebookActivation { get; private set; } = false;
    private bool isPauseScreenActivation = false;
    public static bool doTurnOnGuidebook { get; private set; } = false;
    public static int currentPage { get; private set; } = 1;

    public GameObject BG;
    public GameObject pauseScreen;
    public GameObject Guide;
    public GameObject GuideP1_;
    public GameObject GuideP2_;

    public static GameObject GuideP1;
    public static GameObject GuideP2;

    private void Awake()
    {
        if (Instance == null) // 신 변경에도 유지하는 싱글톤
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        GuideP1 = GuideP1_;
        GuideP2 = GuideP2_;
    }

    private void Start()
    {
        BG.SetActive(false);
        pauseScreen.SetActive(false);
        Guide.SetActive(false);
        GuideP1.SetActive(true);
        GuideP2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (doTurnOnGuidebook)
        {
            doTurnOnGuidebook = false;
            if (ScreenTransition.isTransitioning)
            {
                return;
            }

            BG.SetActive(true);
            Guide.SetActive(true);
            GuideP1 .SetActive(true);
            GuideP2 .SetActive(false);
            isGuidebookActivation=true;
            currentPage = 1;
            DataManager.GamePause();
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (ScreenTransition.isTransitioning)
            {
                return;
            }
            if (DialogueManager.isDialogueActive)
            {
                return ;
            }


            string currentSceneName = SceneManager.GetActiveScene().name;
            if (
                currentSceneName == "MainMenu"||
                currentSceneName == "GameOver"
                ) return;

            if (currentSceneName == "SelectStage") ScreenTransition.Goto("MainMenu", 0.5f, 0.5f);

            BG.SetActive(true);

            if (!isGuidebookActivation && !isPauseScreenActivation)
            {
                pauseScreen.SetActive(true);
                isPauseScreenActivation=true;
                DataManager.GamePause();
            }
            else if (isPauseScreenActivation)
            {
                pauseScreen.SetActive(false);
                isPauseScreenActivation=false;
                BG.SetActive(false);
                DataManager.GameUnPause();
            }
            else if (isGuidebookActivation)
            {
                BG.SetActive(false);
                Guide.SetActive(false);
                isGuidebookActivation = false;
                DataManager.GameUnPause();
            }

        }
    }

    public static void TurnOnGuidebook()
    {
        doTurnOnGuidebook=true;
    }

    public static void GotoP1()
    {
        if (currentPage != 2 || !isGuidebookActivation)
        {
            return ;
        }
        currentPage = 1;
        GuideP1.SetActive(true);
        GuideP2.SetActive(false);
    }

    public static void GotoP2()
    {
        if (currentPage != 1 || !isGuidebookActivation)
        {
            return;
        }
        currentPage = 2;
        GuideP1.SetActive(false);
        GuideP2.SetActive(true);
    }
}
