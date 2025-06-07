using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("공 표시 UI 마커")]
    public GameObject displayBallMarker;

    public static bool canPlay = true;  // 전체 공이 정지해 게임 플레이가 가능한가?
    public static bool isGameOver = false; // 게임 오버되었는가?
    public static int ballNumber = 1;   // 전체 공의 숫자 (레벨) 수치
    public static int scoredBallInChalk = 0;   // 한 초크에 들어간 공의 수

    private bool anyBallMoving;  // 아무 공이나 움직이는가?
    private displayBall displayBall; // 스크립트 참조

    private void Start()
    {
        ballNumber = 1;
        scoredBallInChalk = 0;
        displayBall = displayBallMarker.GetComponent<displayBall>();
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
                    displayBall.DisplayBallReset();
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
        ballNumber += scoredBallInChalk;

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
        Debug.Log("GameOver 함수 작동!"); // 테스트 코드
        // 이후 UI가 정돈되면 작성
    }

}
