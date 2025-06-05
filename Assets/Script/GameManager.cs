using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlay = true;  // ��ü ���� ������ ���� �÷��̰� �����Ѱ�?
    public static bool isGameOver = false; // ���� �����Ǿ��°�?
    public static int ballNumber = 1;   // ��ü ���� ���� (����) ��ġ
    public static int scoredBallInChalk = 0;   // �� ��ũ�� �� ���� ��

    private bool anyBallMoving;  // �ƹ� ���̳� �����̴°�?

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
                if (scoredBallInChalk != 0)
                {
                    BallLevelSet();
                }
                scoredBallInChalk = 0;
            }
        }
    }

    void CheckAllBalls()
    {
        anyBallMoving = false; // ������ ����

        GameObject[] ballObjects = GameObject.FindGameObjectsWithTag("MergeBall");
        // Ball �±� ���� ��� GameObject ã�� �迭�� �ھƵ�
        GameObject[] playerBallObjects = GameObject.FindGameObjectsWithTag("PlayerBall");
        // PlayerBall �±� ���� ��� GameObject ã�� �迭�� �ھƵ�

        GameObject[] allBilliardGameObjects = ballObjects.Concat(playerBallObjects).ToArray();
        // �� GameObject �迭�� �ϳ��� ��ħ (Concat)

        foreach (GameObject obj in allBilliardGameObjects) // GameObject foreach
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>(); // ������ Rigidbody2D ��
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

    void GameOver()
    {
        Debug.Log("GameOver �Լ� �۵�!"); // �׽�Ʈ �ڵ�
        // ���� UI�� �����Ǹ� �ۼ�
    }
}
