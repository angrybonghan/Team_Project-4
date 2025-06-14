using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    // 데이터 저장 경로 상수
    private const string levelAccessKey = "levelAccess"; // 레벨 접근 권한 레벨
    private const string previousLevelKey = "previousLevel"; // 가장 최근 있었던 레벨

    public static int levelAccess { get; private set; }
    public static int previousLevel { get; private set; }


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
    }

    private void LoadData() // 데이터 불러오기
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

    // 저장
    public static void SetLevelAccess(int data) // 레벨 권한 지정
    {
        PlayerPrefs.SetInt(levelAccessKey, data);
        PlayerPrefs.Save();
    }
    public static void SetPreviousLevel(int data) // 이전 레벨 지정
    {
        PlayerPrefs.SetInt(previousLevelKey, data);
        PlayerPrefs.Save();
    }

    public void ResetData() // 데이터 리셋
    {
        PlayerPrefs.SetInt(levelAccessKey, 1);
        PlayerPrefs.SetInt(previousLevelKey, 1);
        PlayerPrefs.Save();
    }
}
