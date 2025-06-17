using UnityEngine;

public class SkillGuideManager : MonoBehaviour
{
    public static SkillGuideManager Instance { get; private set; }

    [Header("생성 위치")]
    public Vector3 startPosition = new Vector3(-7, 1, 0);
    [Header("간격")]
    public float interval = 0.45f;
    [Header("생성할 가이드 프리팹")]
    public GameObject GuidePrefabs;

    public float detectionRadius = 0.1f;
    public static string guideText;


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

    public static void summonGuidePaper(string setGuideText)
    {
        guideText = setGuideText;

        Instance.transform.position = Instance.startPosition;
        if (DetectSkillGuideInRadius())
        {
            while (DetectSkillGuideInRadius())
            {
                Instance.transform.position = new Vector3
                    (Instance.transform.position.x,
                    Instance.transform.position.y - Instance.interval,
                    Instance.transform.position.z);
            }
        }
        Instantiate(Instance.GuidePrefabs, Instance.transform.position, Instance.transform.rotation);
        SoundManager.PlaySound(3);
    }


    public static bool DetectSkillGuideInRadius()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(Instance.transform.position, Instance.detectionRadius);
        // OverlapCircleAll을 찬양하다
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider != null && hitCollider.gameObject.CompareTag("SkillGuide"))
            {
                return true;
            }
        }

        return false;
    }
}
