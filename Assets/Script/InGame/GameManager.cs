using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("공 표시 UI 마커")]
    public GameObject displayBallMarker;
    [Header("플레이어 공")]
    public GameObject playerBall;
    [Header("보드 범위 설정")]
    public float boardMinX = -2.5f; // X축 최소 보드 범위
    public float boardMaxX = 2.5f;  // X축 최대 보드 범위
    public float boardMinY = -3f; // Y축 최소 보드 범위
    public float boardMaxY = 2f;  // Y축 최대 보드 범위
    [Header("남은 시도 횟수")]
    public int attemptsLeft = 10;
    public TextMeshProUGUI attemptsText;
    [Header("화면전환용 이미지")]
    public Image screenChanger;


    public static bool canPlay = true;  // 전체 공이 정지해 게임 플레이가 가능한가?
    public static bool isGameOver = false; // 게임 오버되었는가?
    public static bool isGameWin = false; // 게임 승리했는가?
    public static int ballNumber;   // 전체 공의 숫자 (레벨) 수치
    public static int scoredBallInChalk;   // 한 초크에 들어간 공의 수

    private bool anyBallMoving;  // 아무 공이나 움직이는가?
    private bool gameManagerActivate=true;
    private displayBall displayBall; // 스크립트 참조

    private void Start()
    {
        ballNumber = 1;
        scoredBallInChalk = 0;
        attemptsText.text = attemptsLeft.ToString();
        displayBall = displayBallMarker.GetComponent<displayBall>();
    }


    void Update()
    {
        if (gameManagerActivate)
        {
            if (isGameOver)
            {
                StartCoroutine(fadeOutScreenForGameover());
                gameManagerActivate=false;
                isGameOver = false;
            }

            if (isGameWin)
            {
                GameWin();
                gameManagerActivate = false;
                isGameWin = false;
            }

            if (!canPlay)
            {
                CheckAllBalls();
                if (!anyBallMoving)
                {
                    canPlay = true;

                    if (ballNumber != 8)
                    {
                        ballNumber += scoredBallInChalk; // 공 숫자 지정
                        while (ballNumber > 8)
                        {
                            ballNumber--;
                            attemptsLeft--;
                        }
                    }
                    else
                    {
                        attemptsLeft--;
                    }
                    attemptsLeft--; // 남은 기회 -1
                    attemptsText.text = attemptsLeft.ToString(); // 남은 기회 표시

                    if (attemptsLeft <= 0)
                    {

                        attemptsText.text = "X";
                        StartCoroutine(fadeOutScreenForGameover());
                    }

                    Vector3 playerBallPosition = playerBall.transform.position;
                    if (playerBallPosition.x < boardMinX || playerBallPosition.x > boardMaxX ||
                        playerBallPosition.y < boardMinY || playerBallPosition.y > boardMaxY)
                    {
                        playerBall.transform.position = Vector2.zero; //Vector2.zero = 원점 (X0,Y0)
                    }

                    if (scoredBallInChalk != 0 && ballNumber != 9) // 공이 하나도 들어가지 않는 경우를 대비
                    {
                        BallLevelSet();
                        BallMergeAnimation();
                        scoredBallInChalk = 0;
                        displayBall.DisplayBallReset();
                    }
                }
            }
        }
    }

    void CheckAllBalls() // 공이 움직이는지?
    {
        anyBallMoving = false; // 안전빵 리셋

        BallManager[] allBilliardGameObjects = FindObjectsOfType<BallManager>();
        // 두 GameObject 배열을 하나로 합침 (Concat)

        foreach (BallManager ball in allBilliardGameObjects)
        {
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>(); // 볼에서 Rigidbody2D 뽑
            if (rb != null && Mathf.Abs(rb.velocity.magnitude) >= 0.5f) // 해당 볼이 움직이고 있는지 확인
            {
                anyBallMoving = true; // 하나라도 움직이면 true
                break; // 하나 움직였음으로 다른건 체크 필요 없, foreach 폭파
            }
        }
    }

    void BallLevelSet() // 현재 레벨에 맞는 모양으로 공 모양 변경
    {
        BallManager[] foundBallManagers = FindObjectsOfType<BallManager>();
        foreach (BallManager bm in foundBallManagers)
        {
            bm.SetSprite(ballNumber);
        }
    }
    void BallMergeAnimation() // 모든 존재하는 공 위치에 병합 애니메이션 재생
    {
        BallManager[] foundBallManagers = FindObjectsOfType<BallManager>();
        foreach (BallManager bm in foundBallManagers)
        {
            bm.PlayMergeAnimation();
        }
    }
    

    void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }


    void GameWin()
    {
        Debug.Log("게임 승리!");
    }

    IEnumerator fadeOutScreenForGameover()
    {
        for (int i = 0; i < 50; i++)
        {
            Color currentColor = screenChanger.color;
            currentColor.a += 0.02f;
            screenChanger.color = currentColor;
            yield return Sleep(0.01);
        }
        GameOver();
    }

    IEnumerator Sleep(double SleepSeconds)
    {
        yield return new WaitForSeconds((float)SleepSeconds);
    }
}
