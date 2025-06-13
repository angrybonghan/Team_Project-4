using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    // ������ ���� ��� ���
    private const string levelAccessKey = "levelAccess"; // ���� ���� ���� ����
    private const string previousLevelKey = "previousLevel"; // ���� �ֱ� �־��� ����

    private int levelAccess;
    private int previousLevel;


    void Awake()
    {
        if (Instance == null) // �� ���濡�� �����ϴ� �̱���
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
