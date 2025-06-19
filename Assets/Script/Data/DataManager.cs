using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    // 데이터 저장 경로 상수
    private const string levelAccessKey = "levelAccess"; // 레벨 접근 권한 레벨
    private const string previousLevelKey = "previousLevel"; // 가장 최근 있었던 레벨
    private const string playCutsceneOPKey = "playCutsceneOP"; // 오프닝 컷씬 재생 여부

    public static int levelAccess { get; private set; }
    public static int previousLevel { get; private set; }

    public static bool isGameActionable = false; // 전체 게임이 작동 가능한지 (일시정지 또는 게임오버)

    public static bool PlayCutsceneOP { get; private set; } // 오프닝 컷씬 재생 여부 (읽기 전용으로 변경)


    void Awake()
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

        LoadData();
        isGameActionable = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.E) && Input.GetKeyDown(KeyCode.R))
        {
            ResetData();
            Debug.Log("데이터 리셋 작동");
            ScreenTransition.Goto("SelectStage", 0.1f, 0.1f);
        }

        if (Input.GetKey(KeyCode.E) && Input.GetKeyDown(KeyCode.T))
        {
            SetLevelAccess(8);
            Debug.Log("데이터 올인 작동");
            ScreenTransition.Goto("SelectStage", 0.1f, 0.1f);
        }
    }

    public static void LoadData() // 데이터 불러오기
    {
        bool needsSave = false; // 보정이 한 번만 작동되도록 함

        // 레벨 입장 권한 데이터
        int loadedLevelAccess = PlayerPrefs.GetInt(levelAccessKey, 1);
        if (loadedLevelAccess < 1)
        {
            levelAccess = 1;
            needsSave = true;
        }
        else
        {
            levelAccess = loadedLevelAccess;
        }

        // 이전 레벨 데이터
        int loadedPreviousLevel = PlayerPrefs.GetInt(previousLevelKey, 1);
        if (loadedPreviousLevel < 1)
        {
            previousLevel = 1;
            needsSave = true;
        }
        else
        {
            previousLevel = loadedPreviousLevel;
        }

        int loadedPlayCutsceneOP = PlayerPrefs.GetInt(playCutsceneOPKey, 1);
        PlayCutsceneOP = (loadedPlayCutsceneOP == 1);


        // 보정 발생
        if (needsSave)
        {
            PlayerPrefs.SetInt(levelAccessKey, levelAccess);
            PlayerPrefs.SetInt(previousLevelKey, previousLevel);
            PlayerPrefs.Save();
        }
    }

    public static int GetLevelAccess() // 래벨 접근 권한 반환 (불러옴)
    {
        return levelAccess;
    }

    public static int GetPreviousLevel() // 이전 레벨 반환 (불러옴)
    {
        return previousLevel;
    }

    public static bool GetPlayCutsceneOP() // 오프닝 컷씬 재생 여부 반환 (불러옴)
    {
        return PlayCutsceneOP;
    }


    // 저장
    public static void SetLevelAccess(int data) // 레벨 권한 지정
    {
        PlayerPrefs.SetInt(levelAccessKey, data);
        PlayerPrefs.Save();
        levelAccess = data;
    }

    public static void SetPreviousLevel(int data) // 이전 레벨 지정
    {
        PlayerPrefs.SetInt(previousLevelKey, data);
        PlayerPrefs.Save();
        previousLevel = data;
    }

    // PlayCutsceneOP 저장 메소드 추가
    public static void SetPlayCutsceneOP(bool data)
    {
        PlayerPrefs.SetInt(playCutsceneOPKey, data ? 1 : 0); // bool을 int로 변환 (true=1, false=0)
        PlayerPrefs.Save();
        PlayCutsceneOP = data;
    }

    public void ResetData() // 데이터 리셋
    {
        PlayerPrefs.SetInt(levelAccessKey, 1);
        levelAccess = 1;
        PlayerPrefs.SetInt(previousLevelKey, 1);
        previousLevel = 1;
        PlayerPrefs.SetInt(playCutsceneOPKey, 1);
        PlayCutsceneOP = true;
        PlayerPrefs.Save();
    }

    public static void GamePause() // 일시정지
    {
        Time.timeScale = 0f;
        DataManager.isGameActionable = false;
    }

    public static void GameUnPause() // 일시정지 해제
    {
        Time.timeScale = 1f;
        if (!BossSkillManager.bossAcivate)
        {
            DataManager.isGameActionable = true;
        }
    }
}