using UnityEngine;

public class InGameSingleton : MonoBehaviour
{
    public static InGameSingleton Instance { get; private set; }
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
}
