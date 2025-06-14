using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("�� ǥ�� UI ��Ŀ")]
    public GameObject displayBallMarker;

    [Header("�÷��̾� ��")]
    public GameObject playerBall;

    [Header("���� ���� ����")]
    public static float boardMinX = -2.5f; // X�� �ּ� ���� ����
    public static float boardMaxX = 2.5f;  // X�� �ִ� ���� ����
    public static float boardMinY = -3f; // Y�� �ּ� ���� ����
    public static float boardMaxY = 2f;  // Y�� �ִ� ���� ����

    [Header("���� �õ� Ƚ��")]
    public int attemptsLeftReference=10;
    public static int attemptsLeft;
    public TextMeshProUGUI attemptsTextReference;

    public static TextMeshProUGUI attemptsText;

    [Header("ȭ����ȯ�� �̹���")]
    public Image screenChanger;


    public static bool canPlay = true;  // ��ü ���� ������ ���� �÷��̰� �����Ѱ�?
    public static bool isGameOver = false; // ���� �����Ǿ��°�?
    public static bool isGameWin = false; // ���� �¸��ߴ°�?
    public static int ballNumber;   // ��ü ���� ���� (����) ��ġ
    public static int scoredBallInChalk;   // �� ��ũ�� �� ���� ��

    public static bool isBallEight; // ���� 8�� �����ߴ°�

    private bool anyBallMoving;  // �ƹ� ���̳� �����̴°�
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
                    ballNumber += scoredBallInChalk; // �� ���� ����
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

                    if (scoredBallInChalk > 1) // �� �� - 1 ��ŭ ��ũ ȸ�� (�޺�)
                    {
                        while (scoredBallInChalk > 1)
                        {
                            attemptsLeft++;
                            scoredBallInChalk--;
                        }
                    }

                    
                    attemptsText.text = attemptsLeft.ToString(); // ���� ��ȸ ǥ��

                    if (attemptsLeft <= 0)
                    {

                        attemptsText.text = "X";
                        StartCoroutine(fadeOutScreenForGameover());
                    }

                    // �÷��̾� ���� ���� �վ��ų� (����)
                    // ���� �ȿ� ���ٸ� �������� �ǵ��ƿ���
                    Vector3 playerBallPosition = playerBall.transform.position;
                    if (playerBallPosition.x < boardMinX || playerBallPosition.x > boardMaxX ||
                        playerBallPosition.y < boardMinY || playerBallPosition.y > boardMaxY)
                    {
                        playerBall.transform.position = Vector2.zero; //Vector2.zero = ���� (X0,Y0)
                    }

                    if (scoredBallInChalk != 0 && !isBallEight) // ���� �ϳ��� ���� �ʾҰų� �̹� 8���� ��� ����
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

    void CheckAllBalls() // ���� �����̴���?
    {
        anyBallMoving = false; // ������ ����

        BallDeceleration[] allBilliardGameObjects = FindObjectsOfType<BallDeceleration>();
        // �� GameObject �迭�� �ϳ��� ��ħ (Concat)

        foreach (BallDeceleration ball in allBilliardGameObjects)
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
                Debug.LogError("[???] �±װ� ���� �ƴ� ���� ��ų�� ����� ��");
                break;
        }

    }

}
