using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static bool canPlay = true;  // ��ü ���� ������ ���� �÷��̰� �����Ѱ�?
    public static bool isGameOver = false; // ���� �����Ǿ��°�?
    public static int ballNumber = 1;   // ��ü ���� ���� (����) ��ġ
    public static int scoredBallInChalk = 0;   // �� ��ũ�� �� ���� ��

    public TextMeshProUGUI testText;

    private bool anyBallMoving;  // �ƹ� ���̳� �����̴°�?

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
                Debug.Log("�÷��� ����");
                if (scoredBallInChalk != 0) // ���� �ϳ��� ���� �ʴ� ��츦 ���
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
        Debug.Log("GameOver �Լ� �۵�!"); // �׽�Ʈ �ڵ�
        // ���� UI�� �����Ǹ� �ۼ�
    }

}
