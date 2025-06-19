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

        // 특정 씬에서는 ESC 키 동작 제한
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "MainMenu" || currentSceneName == "GameOver")
        {
            if (escapePressCoroutine != null)
            {
                StopCoroutine(escapePressCoroutine);
                escapePressCoroutine = null;
            }
            return;
        }
        if (currentSceneName == "SelectStage")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ScreenTransition.Goto("MainMenu", 0.5f, 0.5f);
            }
            if (escapePressCoroutine != null)
            {
                StopCoroutine(escapePressCoroutine);
                escapePressCoroutine = null;
            }
            return;
        }

        // --- 실제 ESC 키 입력 감지 및 코루틴 시작/중단 ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escapePressCoroutine == null)
            {
                escapePressCoroutine = StartCoroutine(DetectEscapeLongPress());
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (escapePressCoroutine != null)
            {
                StopCoroutine(escapePressCoroutine);
                escapePressCoroutine = null;

                // 길게 누르기 동작이 실행되지 않았을 경우에만 짧게 누르기 동작 실행
                // (코루틴 내에서 longPressDetected 플래그를 통해 이미 판단)
                // 코루틴이 길게 눌러서 종료되지 않았다면, 짧게 누른 것으로 간주
                // isLongPressHandled 플래그를 코루틴 내부에만 두는 경우 여기서 판단해야 함.
                // 혹은, 코루틴에서 반환하는 값으로 판단하거나.
                // 여기서는 안전하게 코루틴 종료 후 토글 로직을 호출.
                TogglePauseOrGuidebookScreen();
            }
        }
    }

    /// <summary>
    /// ESC 키 길게 누르기를 감지하는 코루틴.
    /// </summary>
    private IEnumerator DetectEscapeLongPress()
    {
        float timer = 0f;

        // 키가 눌려있는 동안 타이머 증가 (Time.timeScale의 영향을 받지 않음)
        // isPauseScreenActivation 상태를 밖에서 먼저 체크했으므로 여기서는 굳이 반복할 필요 없음
        while (Input.GetKey(KeyCode.Escape))
        {
            timer += Time.unscaledDeltaTime;

            if (timer >= LONG_PRESS_DURATION && isPauseScreenActivation)
            {
                // --- 길게 누르기 동작이 실행될 때의 수정된 부분 ---
                // 씬 전환 전에 게임을 일시 정지 해제 (Time.timeScale을 1로 복구)
                DataManager.GameUnPause(); // GameManager의 GameUnPause()가 Time.timeScale = 1로 설정할 것이라고 가정

                // 일시정지 UI를 즉시 비활성화하여 화면 멈춤처럼 보이지 않게 함
                BG.SetActive(false);
                pauseScreen.SetActive(false);
                isPauseScreenActivation = false;

                // 씬 전환 실행
                ScreenTransition.Goto("SelectStage", 0.5f, 0.5f);
                break; // 코루틴 종료
            }
            yield return null;
        }
    }

    /// <summary>
    /// 현재 활성화된 화면 상태에 따라 일시정지 또는 가이드북 화면을 토글합니다.
    /// </summary>
    private void TogglePauseOrGuidebookScreen()
    {
        // 배경은 항상 토글 전에 활성화하는 것은 아니고, 상태에 따라 달라짐.
        // 이 함수 자체가 토글 역할을 하므로, 내부에서 BG 활성화/비활성화를 적절히 처리해야 함.

        if (!isGuidebookActivation && !isPauseScreenActivation) // 아무것도 활성화되어 있지 않을 때 (일시정지 켜기)
        {
            BG.SetActive(true); // 배경 활성화
            pauseScreen.SetActive(true);
            isPauseScreenActivation = true;
            DataManager.GamePause();
        }
        else if (isPauseScreenActivation) // 일시정지 화면이 활성화되어 있을 때 (일시정지 끄기)
        {
            pauseScreen.SetActive(false);
            isPauseScreenActivation = false;
            BG.SetActive(false); // 일시정지 화면이 꺼지면 배경도 끔
            DataManager.GameUnPause();
        }
        else if (isGuidebookActivation) // 가이드북이 활성화되어 있을 때 (가이드북 끄기)
        {
            BG.SetActive(false); // 가이드북이 꺼지면 배경도 끔
            Guide.SetActive(false);
            isGuidebookActivation = false;
            DataManager.GameUnPause();
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