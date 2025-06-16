using UnityEngine;

public class SkillGuideManager : MonoBehaviour
{
    public static SkillGuideManager Instance { get; private set; }

    [Header("생성 위치")]
    public Vector3 startPos = new Vector3(-7, 1, 0);

    public float detectionRadius = 0.1f;


    private void Awake()
    {
        if (Instance == null) // ㅅㄱㅌ
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
    void Start()
    {
        
    }
}
