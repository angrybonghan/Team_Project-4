using UnityEngine;

public class InGameSingleton : MonoBehaviour
{
    public static InGameSingleton Instance { get; private set; }
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
}
