using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    // ������ ���� ��� ���
    private const string levelAccessKey = "levelAccess"; // ���� ���� ���� ����
    private const string previousLevelKey = "previousLevel"; // ���� �ֱ� �־��� ����

    public static int levelAccess { get; private set; }
    public static int previousLevel { get; private set; }


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
        LoadData();
    }

    private void LoadData() // ������ �ҷ�����
    {
        bool needsSave = false; // ������ �� ���� �۵��ǵ��� ��

        // ���� ���� ���� ������
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

        // ���� ���� ������
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

        // ���� �߻�
        if (needsSave)
        {
            PlayerPrefs.SetInt(levelAccessKey, levelAccess);
            PlayerPrefs.SetInt(previousLevelKey, previousLevel);
            PlayerPrefs.Save();
        }
    }

    public static int GetLevelAccess() // ���� ���� ���� ��ȯ (�ҷ���)
    {
        return levelAccess;
    }

    public static int GetPreviousLevel() // ���� ���� ��ȯ (�ҷ���)
    {
        return previousLevel;
    }

    // ����
    public static void SetLevelAccess(int data) // ���� ���� ����
    {
        PlayerPrefs.SetInt(levelAccessKey, data);
        PlayerPrefs.Save();
    }
    public static void SetPreviousLevel(int data) // ���� ���� ����
    {
        PlayerPrefs.SetInt(previousLevelKey, data);
        PlayerPrefs.Save();
    }

    public void ResetData() // ������ ����
    {
        PlayerPrefs.SetInt(levelAccessKey, 1);
        PlayerPrefs.SetInt(previousLevelKey, 1);
        PlayerPrefs.Save();
    }
}
