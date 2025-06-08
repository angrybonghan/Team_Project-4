using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("�� ǥ�� UI ��Ŀ")]
    public GameObject displayBallMarker;
    [Header("�÷��̾� ��")]
    public GameObject playerBall;
    [Header("���� ���� ����")]
    public float boardMinX = -2.5f; // X�� �ּ� ���� ����
    public float boardMaxX = 2.5f;  // X�� �ִ� ���� ����
    public float boardMinY = -3f; // Y�� �ּ� ���� ����
    public float boardMaxY = 2f;  // Y�� �ִ� ���� ����
    [Header("���� �õ� Ƚ��")]
    public int attemptsLeft = 10;
    public TextMeshProUGUI attemptsText;
    [Header("ȭ����ȯ�� �̹���")]
    public Image screenChanger;


    public static bool canPlay = true;  // ��ü ���� ������ ���� �÷��̰� �����Ѱ�?
    public static bool isGameOver = false; // ���� �����Ǿ��°�?
    public static bool isGameWin = false; // ���� �¸��ߴ°�?
    public static int ballNumber;   // ��ü ���� ���� (����) ��ġ
    public static int scoredBallInChalk;   // �� ��ũ�� �� ���� ��

    private bool anyBallMoving;  // �ƹ� ���̳� �����̴°�?
    private bool gameManagerActivate=true;
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
                        ballNumber += scoredBallInChalk; // �� ���� ����
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
                    attemptsLeft--; // ���� ��ȸ -1
                    attemptsText.text = attemptsLeft.ToString(); // ���� ��ȸ ǥ��

                    if (attemptsLeft <= 0)
                    {

                        attemptsText.text = "X";
                        StartCoroutine(fadeOutScreenForGameover());
                    }

                    Vector3 playerBallPosition = playerBall.transform.position;
                    if (playerBallPosition.x < boardMinX || playerBallPosition.x > boardMaxX ||
                        playerBallPosition.y < boardMinY || playerBallPosition.y > boardMaxY)
                    {
                        playerBall.transform.position = Vector2.zero; //Vector2.zero = ���� (X0,Y0)
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
        SceneManager.LoadScene("GameOver");
    }


    void GameWin()
    {
        Debug.Log("���� �¸�!");
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
