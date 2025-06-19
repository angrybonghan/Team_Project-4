using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Coroutine 사용을 위해 추가

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

    // --- ESC 길게 누르기 관련 변수 ---
    private const float LONG_PRESS_DURATION = 0.75f;
    private Coroutine escapePressCoroutine;

    private void Awake()
    {
        if (Instance == null)
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
            GuideP1.SetActive(true);
            GuideP2.SetActive(false);
            isGuidebookActivation = true;
            currentPage = 1;
            DataManager.GamePause();
        }

        // --- ESC 키 입력 처리 로직 시작 ---

        // 화면 전환 중이거나 대화 중일 때는 모든 ESC 입력 무시
        if (ScreenTransition.isTransitioning || DialogueManager.isDialogueActive)
        {
            if (escapePressCoroutine != null)
            {
                StopCoroutine(escapePressCoroutine);
                escapePressCoroutine = null;
            }
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;

        // 절대 ESC 동작을 허용하지 않는 씬 (일시정지/가이드북, 씬 전환 불가)
        if (currentSceneName == "MainMenu" ||
            currentSceneName == "GameOver" ||
            currentSceneName == "GameOverToBoss" ||
            currentSceneName == "Cutscene_ED"||
            currentSceneName == "Cutscene_OP")
        {
            if (escapePressCoroutine != null)
            {
                StopCoroutine(escapePressCoroutine);
                escapePressCoroutine = null;
            }
            return; // 이 씬들에서는 어떤 ESC 입력도 무시하고 Update() 함수를 종료합니다.
        }

        // ESC를 누르면 MainMenu로 바로 이동하는 씬들 (SelectStage, CreditScene)
        if (currentSceneName == "SelectStage" || currentSceneName == "CreditScene")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 이 씬들에서 ESC를 누르면 바로 MainMenu로 이동합니다.
                DataManager.GameUnPause(); // 씬 전환 전에 게임 일시 정지 상태를 해제합니다.
                ScreenTransition.Goto("MainMenu", 0.5f, 0.5f);
            }
            // 이 씬들에서는 길게 누르기 코루틴이 시작되거나 유지될 필요가 없으므로 중단합니다.
            if (escapePressCoroutine != null)
            {
                StopCoroutine(escapePressCoroutine);
                escapePressCoroutine = null;
            }
            return; // 이 씬들에서는 ESC 입력 시 위의 로직만 실행하고 Update() 함수를 종료합니다.
        }

        // --- 기본 게임 플레이 씬에서의 ESC 키 입력 감지 및 코루틴 시작/중단 ---
        // (위의 조건문들을 통과한 씬들, 즉 일반적인 게임 플레이 씬에만 적용됩니다.)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escapePressCoroutine == null) // 현재 ESC 코루틴이 실행 중이 아니라면 시작
            {
                escapePressCoroutine = StartCoroutine(DetectEscapeLongPress());
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (escapePressCoroutine != null) // ESC 코루틴이 실행 중이었다면
            {
                StopCoroutine(escapePressCoroutine); // 코루틴을 중단하고
                escapePressCoroutine = null; // 참조를 해제합니다.

                // 길게 누르기 동작이 실행되지 않았다면 (코루틴이 씬 전환을 실행하지 않았다면)
                // 짧게 누르기 동작 (일시정지/가이드북 토글)을 실행합니다.
                TogglePauseOrGuidebookScreen();
            }
        }
    }

    /// <summary>
    /// ESC 키 길게 누르기를 감지하는 코루틴입니다.
    /// </summary>
    private IEnumerator DetectEscapeLongPress()
    {
        float timer = 0f;

        // ESC 키가 눌려있는 동안 타이머를 증가시킵니다 (Time.timeScale의 영향을 받지 않음).
        while (Input.GetKey(KeyCode.Escape))
        {
            timer += Time.unscaledDeltaTime;

            // 타이머가 LONG_PRESS_DURATION을 초과하고 일시정지 화면이 활성화된 상태일 경우
            if (timer >= LONG_PRESS_DURATION && isPauseScreenActivation)
            {
                // 씬 전환 전에 게임 일시 정지 상태를 해제합니다 (Time.timeScale을 1로 복구).
                DataManager.GameUnPause();

                // 일시정지 UI를 즉시 비활성화하여 화면 멈춤처럼 보이는 현상을 방지합니다.
                BG.SetActive(false);
                pauseScreen.SetActive(false);
                isPauseScreenActivation = false;

                // "SelectStage" 씬으로 전환을 실행합니다.
                ScreenTransition.Goto("SelectStage", 0.5f, 0.5f);
                break; // 코루틴을 종료합니다.
            }
            yield return null; // 다음 프레임까지 대기합니다.
        }
    }

    /// <summary>
    /// 현재 활성화된 화면 상태에 따라 일시정지 또는 가이드북 화면을 토글합니다.
    /// </summary>
    private void TogglePauseOrGuidebookScreen()
    {
        if (!isGuidebookActivation && !isPauseScreenActivation) // 아무것도 활성화되어 있지 않을 때 (일시정지 화면 켜기)
        {
            BG.SetActive(true); // 배경을 활성화합니다.
            pauseScreen.SetActive(true); // 일시정지 화면을 활성화합니다.
            isPauseScreenActivation = true;
            DataManager.GamePause(); // 게임을 일시 정지합니다.
        }
        else if (isPauseScreenActivation) // 일시정지 화면이 활성화되어 있을 때 (일시정지 화면 끄기)
        {
            pauseScreen.SetActive(false); // 일시정지 화면을 비활성화합니다.
            isPauseScreenActivation = false;
            BG.SetActive(false); // 배경도 비활성화합니다.
            DataManager.GameUnPause(); // 게임을 재개합니다.
        }
        else if (isGuidebookActivation) // 가이드북이 활성화되어 있을 때 (가이드북 끄기)
        {
            BG.SetActive(false); // 배경을 비활성화합니다.
            Guide.SetActive(false); // 가이드북을 비활성화합니다.
            isGuidebookActivation = false;
            DataManager.GameUnPause(); // 게임을 재개합니다.
        }
    }

    public static void TurnOnGuidebook()
    {
        doTurnOnGuidebook = true;
    }

    public static void GotoP1()
    {
        if (currentPage != 2 || !isGuidebookActivation)
        {
            return;
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