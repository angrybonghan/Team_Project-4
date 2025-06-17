using System.Collections;
using TMPro;
using UnityEngine;

public class displayBall : MonoBehaviour
{
    public static displayBall Instance { get; private set; }

    [Header("표시용 공 프리팹")]
    public GameObject displayBallPrefab;

    [Header("보드 제일 왼쪽 칸의 중앙 위치")]
    public static Vector2 startPosition = new Vector2(-96.41f, -13f);

    [Header("칸과 칸 사이의 거리")]
    public float cellGap = 24.1025f;

    [Header("콤보 표시용 텍스트")]
    public TextMeshProUGUI comboText_;
    public static TextMeshProUGUI ComboText; // static 은 인스펙터에 안 뜬다.

    [Header("콤보 애니메이션 프리팹")]
    public GameObject comboAnimationPrefabs_;
    public static GameObject comboAnimationPrefabs;

    public static int DisplayBallCount = 0;
    private static int existingDisplayBallCount=0;
    private Coroutine comboCoroutine;

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ComboText = comboText_;
        comboAnimationPrefabs = comboAnimationPrefabs_;
        
        existingDisplayBallCount = 0;
        DisplayBallReset();
    }

    private void Update()
    {
        if (DisplayBallCount > 0)
        {
            DisplayBallCount--;
            SpawnDisplayBall(); // 이 함수는 static이 아니므로 Update에서 직접 호출 가능
        }
    }

    public static void DisplayBallReset() // 보드 UI에 존재하는 표시용 공 제거
    {
        if (existingDisplayBallCount > 1 && GameManager.scoredBallInChalk > 1)
        {
            SoundManager.PlaySound(6);
            ComboText.text = $"[초크 {existingDisplayBallCount - 1}개 회복됨]";
            Instance.comboCoroutine = Instance.StartCoroutine(Instance.comboAnimation());
        }
        
        DisplayBallCount = 0;
        existingDisplayBallCount = 0;

        GameObject[] displayBalls = GameObject.FindGameObjectsWithTag("DisplayBall");
        foreach (GameObject ball in displayBalls)
        {
            Destroy(ball);
        }
        
        ComboText.gameObject.SetActive(false);
        Instance.transform.position = startPosition;
    }

    public void SpawnDisplayBall() // 공 하나 생성
    {
        GameObject Display = Instantiate(displayBallPrefab, new Vector2(transform.position.x, 3.5f), transform.rotation);
        Display.transform.SetParent(this.transform.parent, false);
        transform.position = new Vector2(transform.position.x + cellGap, transform.position.y);
        existingDisplayBallCount++;
    }

    IEnumerator comboAnimation()
    {
        GameObject Animation = Instantiate(comboAnimationPrefabs);
        yield return new WaitForSeconds(0.5f);
        ComboText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.1f);
        ComboText.gameObject.SetActive(false);
        comboCoroutine=null;
    }
}