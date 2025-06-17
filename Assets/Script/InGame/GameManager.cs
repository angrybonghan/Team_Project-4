using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("다음 레벨 번호")]
    public int nextLevelNumber = 2;  

    [Header("공 표시 UI 마커")]
    public GameObject displayBallMarker;

    [Header("플레이어 공")]
    public GameObject playerBall_;
    public static GameObject playerBall;

    [Header("보드 범위 설정")]
    public float boardMinX_ = -2.5f;
    public float boardMaxX_ = 2.5f;
    public float boardMinY_ = -3f;
    public float boardMaxY_ = 2f;

    public static float boardMinX ; // X축 최소 보드 범위
    public static float boardMaxX;  // X축 최대 보드 범위
    public static float boardMinY; // Y축 최소 보드 범위
    public static float boardMaxY;  // Y축 최대 보드 범위

    [Header("남은 시도 횟수")]
    public int attemptsLeft_ = 10;
    public static int attemptsLeft;
    public TextMeshProUGUI attemptsText_;
    public static TextMeshProUGUI attemptsText;

    public TextMeshProUGUI RandomBallattemptsText;

    [Header("게임 승리 애니메이션")]
    public GameObject victoryAnimation;

    [Header("랜덤 볼 애니메이션")]
    public GameObject RandomPickAnimation;
    [Header("랜덤 볼 기회")]
    public int RandomPickChance = 3;

    public static bool canPlay = true;  // 전체 공이 정지해 게임 플레이가 가능한가?
    public static bool isGameOver = false; // 게임 오버되었는가?
    public static bool isGameWin = false; // 게임 승리했는가?
    public static bool isChaosBallActivate = false; // 카오스 볼 작동
    public static bool isBallEight; // 공이 8에 도달했는가

    public static int SpeedBallChance = 0;
    public static int ballNumber;   // 전체 공의 숫자 (레벨) 수치
    public static int scoredBallInChalk;   // 한 초크에 들어간 공의 수


    private bool anyBallMoving;  // 아무 공이나 움직이는가

    private void Awake()
    {
        attemptsLeft = attemptsLeft_;
        attemptsText = attemptsText_;

        boardMinX = boardMinX_;
        boardMaxX = boardMaxX_;
        boardMinY = boardMinY_;
        boardMaxY = boardMaxY_;

        playerBall = playerBall_;
    }

    private void Start()
    {
        ballNumber = 1;
        scoredBallInChalk = 0;
        isBallEight = false;
        canPlay = true;
        isChaosBallActivate = false;
        SpeedBallChance = 0;
        attemptsText.text = attemptsLeft.ToString();
        DataManager.SetPreviousLevel(nextLevelNumber-1);
    }


    void Update()
    {

        if (DataManager.isGameActionable)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                scoredBallInChalk++;
                displayBall.DisplayBallCount++;
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                SpeedBallChance=2;
            }

            if (isGameOver)
            {
                GameOver();
                isGameOver = false;
                return;
            }

            if (isGameWin)
            {
                GameWin();
                DataManager.isGameActionable = false;
                isGameWin = false;
                return;
            }

            if (!canPlay)
            {
                CheckAllBalls();
                if (!anyBallMoving)
                {
                    canPlay = true;
                    ballNumber += scoredBallInChalk; // 공 숫자 지정
                    attemptsLeft--;

                    if (ballNumber > 8)
                    {
                        scoredBallInChalk = 1;
                        while (ballNumber > 8)
                        {
                            ballNumber--;
                            attemptsLeft--;
                        }
                    }

                    displayBall.DisplayBallReset();

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
                        GameOver();
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

                    if (isChaosBallActivate)
                    {
                        isChaosBallActivate = false;

                        BallDeceleration[] allBallDecelerationScripts = FindObjectsOfType<BallDeceleration>();

                        List<GameObject> ballObjects = new List<GameObject>();
                        List<Vector3> originalPositions = new List<Vector3>();

                        foreach (BallDeceleration ballScript in allBallDecelerationScripts)
                        {
                            ballObjects.Add(ballScript.gameObject);
                            originalPositions.Add(ballScript.transform.position);
                        }
                        List<Vector3> shuffledPositions = new List<Vector3>(originalPositions);

                        int n = shuffledPositions.Count;
                        for (int i = 0; i < n; i++)
                        {
                            int randomIndex = Random.Range(i, n);
                            Vector3 temp = shuffledPositions[i];
                            shuffledPositions[i] = shuffledPositions[randomIndex];
                            shuffledPositions[randomIndex] = temp;
                        }
                        for (int i = 0; i < ballObjects.Count; i++)
                        {
                            GameObject currentBall = ballObjects[i];
                            Vector3 newPosition = shuffledPositions[i];

                            currentBall.transform.position = newPosition;
                        }
                    }

                    if (SpeedBallChance >= 1)
                    {
                        SpeedBallChance--;
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
        ScreenTransition.Goto("GameOver", 0.5f, 0f);
        DataManager.isGameActionable = false;
    }


    void GameWin()
    {
        DataManager.SetLevelAccess(nextLevelNumber);
        Instantiate(victoryAnimation);
        ScreenTransition.Goto("SelectStage", 2.2f, 0.5f);
    }

    public void RandomPick()
    {
        if (RandomPickChance <= 0)
        {
            return;
        }
        if (!DataManager.isGameActionable)
        {
            return;
        }

        attemptsLeft++;
        RandomPickChance--;
        RandomBallattemptsText.text = $"x{RandomPickChance}";

        BallDeceleration[] allBalls = FindObjectsOfType<BallDeceleration>();

        int randomBallIndex = Random.Range(0, allBalls.Length);
        BallDeceleration selectedBallScript = allBalls[randomBallIndex];
        GameObject selectedBallObject = selectedBallScript.gameObject;

        Instantiate(RandomPickAnimation, selectedBallObject.transform.position, selectedBallObject.transform.rotation);

        GameObject[] allHoles = GameObject.FindGameObjectsWithTag("Hole");
        int randomHoleIndex = Random.Range(0, allHoles.Length);
        GameObject selectedHoleObject = allHoles[randomHoleIndex];

        selectedBallObject.transform.position = selectedHoleObject.transform.position;
    }

}
