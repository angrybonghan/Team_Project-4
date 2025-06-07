using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("�� ǥ�� UI ��Ŀ")]
    public GameObject displayBallMarker;
    [Header("���� �õ� Ƚ��")]
    public int attemptsLeft = 10;
    public TextMeshProUGUI attemptsText;

    public static bool canPlay = true;  // ��ü ���� ������ ���� �÷��̰� �����Ѱ�?
    public static bool isGameOver = false; // ���� �����Ǿ��°�?
    public static int ballNumber;   // ��ü ���� ���� (����) ��ġ
    public static int scoredBallInChalk;   // �� ��ũ�� �� ���� ��

    private bool anyBallMoving;  // �ƹ� ���̳� �����̴°�?
    private displayBall displayBall; // ��ũ��Ʈ ����

    private void Start()
    {
        ballNumber = 1;
        scoredBallInChalk = 0;
        attemptsText.text = attemptsLeft.ToString();
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
                attemptsLeft--; // ���� ��ȸ -1
                attemptsText.text = attemptsLeft.ToString(); // ���� ��ȸ ǥ��
                ballNumber += scoredBallInChalk; // �� ���� ����

                if (ballNumber == 9) // ���� �¸� or �й� ����
                {
                    GameWin(); // ��
                }
                else if (attemptsLeft == 0)
                {
                    GameOver(); // ��
                }

                if (scoredBallInChalk != 0 && ballNumber != 9) // ���� �ϳ��� ���� �ʴ� ��츦 ���
                {
                    BallLevelSet();
                    BallMergeAnimation();
                    scoredBallInChalk = 0;
                    displayBall.DisplayBallReset();
                }
            }
        }
    }

    void CheckAllBalls() // ���� �����̴���?
    {
        anyBallMoving = false; // ������ ����

        BallManager[] allBilliardGameObjects = FindObjectsOfType<BallManager>();
        // �� GameObject �迭�� �ϳ��� ��ħ (Concat)

        foreach (BallManager ball in allBilliardGameObjects)
        {
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>(); // ������ Rigidbody2D ��
            if (rb != null && Mathf.Abs(rb.velocity.magnitude) >= 0.5f) // �ش� ���� �����̰� �ִ��� Ȯ��
            {
                anyBallMoving = true; // �ϳ��� �����̸� true
                break; // �ϳ� ������������ �ٸ��� üũ �ʿ� ��, foreach ����
            }
        }
    }

    void BallLevelSet() // ���� ������ �´� ������� �� ��� ����
    {
        BallManager[] foundBallManagers = FindObjectsOfType<BallManager>();
        foreach (BallManager bm in foundBallManagers)
        {
            bm.SetSprite(ballNumber);
        }
    }
    void BallMergeAnimation() // ��� �����ϴ� �� ��ġ�� ���� �ִϸ��̼� ���
    {
        BallManager[] foundBallManagers = FindObjectsOfType<BallManager>();
        foreach (BallManager bm in foundBallManagers)
        {
            bm.PlayMergeAnimation();
        }
    }
    

    void GameOver()
    {
        Debug.Log("GameOver �Լ� �۵�!"); // �׽�Ʈ �ڵ�
        // ���� UI�� �����Ǹ� �ۼ�
    }


    void GameWin()
    {
        Debug.Log("���� �¸�!");
    }
}
