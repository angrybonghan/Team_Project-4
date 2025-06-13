using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public TextMeshProUGUI comboTextReference;
    public static TextMeshProUGUI ComboText; // static 은 인스펙터에 안 뜬다.

    public static int DisplayBallCount = 0;
    private static int existingDisplayBallCount=0;

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

        ComboText = comboTextReference;
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
        DisplayBallCount = 0;
        existingDisplayBallCount = 0;

        GameObject[] displayBalls = GameObject.FindGameObjectsWithTag("DisplayBall");
        foreach (GameObject ball in displayBalls)
        {
            Destroy(ball);
        }

        ComboText.gameObject.SetActive(false); // 텍스트 리셋
        ComboText.text = "UHH, NOPE.";
        Instance.transform.position = startPosition;
    }

    public void SpawnDisplayBall() // 공 하나 생성
    {
        GameObject Display = Instantiate(displayBallPrefab, new Vector2(transform.position.x, 3.5f), transform.rotation);
        Display.transform.SetParent(this.transform.parent, false);
        transform.position = new Vector2(transform.position.x + cellGap, transform.position.y);
        existingDisplayBallCount++;

        if (existingDisplayBallCount > 1 && GameManager.ballNumber < 8)
        {
            ComboText.gameObject.SetActive(true);
            ComboText.text = $"[COMBO × {existingDisplayBallCount}]";
        }
    }
}