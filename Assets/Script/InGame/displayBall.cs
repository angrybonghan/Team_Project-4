using UnityEngine;

public class displayBall : MonoBehaviour
{
    [Header("표시용 공 프리팹")]
    public GameObject displayBallPrefab;

    [Header("보드 제일 왼쪽 칸의 중앙 위치")]
    public Vector2 startPosition = new Vector2(-96.41f, -13f);

    [Header("칸과 칸 사이의 거리")]
    public float cellGap = 24.1025f;

    public static int DisplayBallCount = 0;

    void Awake()
    {
        DisplayBallReset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            DisplayBallCount++;
        }

        if (DisplayBallCount > 0)
        {
            DisplayBallCount--;
            SpawnDisplayBall();
        }
    }

    public void DisplayBallReset() // 보드 UI에 존재하는 표시용 공 제거
    {
        transform.position = startPosition; // 생성용 마커 위치 재정렬
        DisplayBallCount = 0;

        GameObject[] displayBalls = GameObject.FindGameObjectsWithTag("DisplayBall");
        foreach (GameObject ball in displayBalls)
        {
            Destroy(ball);
        }
    }

    public void SpawnDisplayBall()
    {
        GameObject Display = Instantiate(displayBallPrefab, new Vector2(transform.position.x, 3.5f), transform.rotation);
        Display.transform.SetParent(this.transform.parent, false);
        transform.position = new Vector2(transform.position.x + cellGap, transform.position.y);
    }
}
