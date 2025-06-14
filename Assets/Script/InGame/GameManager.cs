using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("공 표시 UI 마커")]
    public GameObject displayBallMarker;

    [Header("플레이어 공")]
    public GameObject playerBall;

    [Header("보드 범위 설정")]
    public static float boardMinX = -2.5f; // X축 최소 보드 범위
    public static float boardMaxX = 2.5f;  // X축 최대 보드 범위
    public static float boardMinY = -3f; // Y축 최소 보드 범위
    public static float boardMaxY = 2f;  // Y축 최대 보드 범위

    [Header("남은 시도 횟수")]
    public int attemptsLeftReference=10;
    public static int attemptsLeft;
    public TextMeshProUGUI attemptsTextReference;

    public static TextMeshProUGUI attemptsText;

    [Header("화면전환용 이미지")]
    public Image screenChanger;


    public static bool canPlay = true;  // 전체 공이 정지해 게임 플레이가 가능한가?
    public static bool isGameOver = false; // 게임 오버되었는가?
    public static bool isGameWin = false; // 게임 승리했는가?
    public static int ballNumber;   // 전체 공의 숫자 (레벨) 수치
    public static int scoredBallInChalk;   // 한 초크에 들어간 공의 수

    public static bool isBallEight; // 공이 8에 도달했는가

    private bool anyBallMoving;  // 아무 공이나 움직이는가
    private bool gameManagerActivate=true;

    private void Awake()
    {
        gameManagerActivate = true;
        canPlay = true;
        attemptsLeft = attemptsLeftReference;
        attemptsText = attemptsTextReference;
    }

    private void Start()
    {
        ballNumber = 1;
        scoredBallInChalk = 0;
        isBallEight = false;
        attemptsText.text = attemptsLeft.ToString();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            scoredBallInChalk++;
            displayBall.DisplayBallCount++;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            RandomPick();
        }
        

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
                    ballNumber += scoredBallInChalk; // 공 숫자 지정
                    attemptsLeft--;
                    displayBall.DisplayBallReset();

                    if (ballNumber > 8)
                    {
                        scoredBallInChalk = 1;
                        while (ballNumber > 8)
                        {
                            ballNumber--;
                            attemptsLeft--;
                        }
                    }

                    if (scoredBallInChalk > 1) // 들어간 공 - 1 만큼 초크 회복 (콤보)
                    {
                        while (scoredBallInChalk > 1)
                        {
                            attemptsLeft++;
                            scoredBallInChalk--;
                        }
                    }

                    
                    attemptsText.text = attemptsLeft.ToString(); // 남은 기회 표시

                    if (attemptsLeft <= 0)
                    {

                        attemptsText.text = "X";
                        StartCoroutine(fadeOutScreenForGameover());
                    }

                    // 플레이어 공이 벽을 뚫었거나 (버그)
                    // 구멍 안에 들어갔다면 원점으로 되돌아오기
                    Vector3 playerBallPosition = playerBall.transform.position;
                    if (playerBallPosition.x < boardMinX || playerBallPosition.x > boardMaxX ||
                        playerBallPosition.y < boardMinY || playerBallPosition.y > boardMaxY)
                    {
                        playerBall.transform.position = Vector2.zero; //Vector2.zero = 원점 (X0,Y0)
                    }

                    if (scoredBallInChalk != 0 && !isBallEight) // 공이 하나도 들어가지 않았거나 이미 8볼일 경우 무시
                    {
                        BallLevelSet();
                        BallMergeAnimation();
                        scoredBallInChalk = 0;
                    }

                    if (ballNumber >= 8)
                    {
                        isBallEight = true;
                    }
                }
            }
        }
    }

    void CheckAllBalls() // 공이 움직이는지?
    {
        anyBallMoving = false; // 안전빵 리셋

        BallDeceleration[] allBilliardGameObjects = FindObjectsOfType<BallDeceleration>();
        // 두 GameObject 배열을 하나로 합침 (Concat)

        foreach (BallDeceleration ball in allBilliardGameObjects)
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

    void RandomPick()
    {
        GameObject[] eightBallObjects = GameObject.FindGameObjectsWithTag("8Ball");
        GameObject[] mergeBallObjects = GameObject.FindGameObjectsWithTag("MergeBall");
        List<GameObject> targetBallObjects = eightBallObjects.Concat(mergeBallObjects).ToList();

        int randomIndex = Random.Range(0, targetBallObjects.Count);
        GameObject selectedBall = targetBallObjects[randomIndex];

        switch (selectedBall.gameObject.tag)
        {
            case "MergeBall":
                Destroy(selectedBall);
                scoredBallInChalk++;
                displayBall.DisplayBallCount++;
                break;

            case "8Ball":
                Destroy(selectedBall);
                if (isBallEight)
                {
                    GameWin();
                }
                else
                {
                    GameOver();
                }
                break;

            default:
                Debug.LogError("[???] 태그가 뭣도 아닌 것이 스킬의 대상이 됨");
                break;
        }

    }

}
