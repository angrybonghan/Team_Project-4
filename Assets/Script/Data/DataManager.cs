using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    // 데이터 저장 경로 상수
    private const string levelAccessKey = "levelAccess"; // 레벨 접근 권한 레벨
    private const string previousLevelKey = "previousLevel"; // 가장 최근 있었던 레벨

    private int levelAccess;
    private int previousLevel;


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


    }

    private void Start()
    {
        PlayerPrefs.SetInt("", 1);
    }
}
