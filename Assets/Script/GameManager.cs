using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static bool canPlay = true;  // 전체 공이 정지해 게임 플레이가 가능한가?
    public static bool isGameOver = false; // 게임 오버되었는가?
    public static int ballNumber = 1;   // 전체 공의 숫자 (레벨) 수치
    public static int scoredBallInChalk = 0;   // 한 초크에 들어간 공의 수

    public TextMeshProUGUI testText;

    private bool anyBallMoving;  // 아무 공이나 움직이는가?

    private void Start()
    {
        ballNumber = 1;
        scoredBallInChalk = 0;
    }


    void Update()
    {
        if (isGameOver)
        {
            GameOver();
            isGameOver = false;
        }

        if (!canPlay)
        {
            CheckAllBalls();
            if (!anyBallMoving)
            {
                canPlay = true;
                Debug.Log("플레이 가능");
                if (scoredBallInChalk != 0) // 공이 하나도 들어가지 않는 경우를 대비
                {
                    BallLevelSet();
                    BallMergeAnimation();
                    scoredBallInChalk = 0;
                }
            }
        }
        testText.text=scoredBallInChalk.ToString();
    }

    void CheckAllBalls()
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

    void BallLevelSet()
    {
        ballNumber += scoredBallInChalk;

        BallManager[] foundBallManagers = FindObjectsOfType<BallManager>();
        foreach (BallManager bm in foundBallManagers)
        {
            bm.SetSprite(ballNumber);
        }
    }
    void BallMergeAnimation()
    {
        BallManager[] foundBallManagers = FindObjectsOfType<BallManager>();
        foreach (BallManager bm in foundBallManagers)
        {
            bm.PlayMergeAnimation();
        }
    }

    void GameOver()
    {
        Debug.Log("GameOver 함수 작동!"); // 테스트 코드
        // 이후 UI가 정돈되면 작성
    }

}
