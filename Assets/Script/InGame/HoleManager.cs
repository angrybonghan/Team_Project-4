using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [Header("�� ������ �ִϸ��̼� ������")]
    public GameObject animationPrefabs;

    private Rigidbody2D rb;


    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "MergeBall":   // �� ���ۿ� �������� ����
                Destroy(other.gameObject);
                PlayAnimation(); // �ִϸ��̼� ���� �Լ�
                GameManager.scoredBallInChalk ++; //���� ��ũ�� �� ���� ���� �ø���
                displayBall.DisplayBallCount ++;
                break;
            case "PlayerBall":  // �÷��̾� ���� ���ۿ� ���� �׳� ���ָ� ��������
                GameManager.attemptsLeft--; // �÷��̾� ���� �� �� ���� ��ũ --
                GameManager.attemptsText.text = GameManager.attemptsLeft.ToString(); // �ؽ�Ʈ UI ������Ʈ
                rb = other.GetComponent<Rigidbody2D>();
                other.transform.position = new Vector2(999999, 999999);
                rb.velocity = Vector2.zero;

                PlayAnimation();
                break;
            case "8Ball":
                PlayAnimation();
                Destroy(other.gameObject);

                if (GameManager.isBallEight)
                {
                    GameManager.isGameWin = true;
                }
                else
                {
                    GameManager.isGameOver = true;
                }
                break;

            case "OB_Level_Up":
                Destroy(other.gameObject);
                PlayAnimation(); // �ִϸ��̼� ����
                GameManager.scoredBallInChalk += 2; //���� ��ũ�� �� ���� ���� �ΰ� �ø���
                displayBall.DisplayBallCount += 2;
                break;

            case "OB_Level_Down":
                Destroy(other.gameObject);
                PlayAnimation(); // �ִϸ��̼� ����
                if (GameManager.ballNumber >= 1)
                {
                    GameManager.ballNumber -= 2;
                    GameManager.scoredBallInChalk++;
                }
                break;

            default:
                Debug.LogError("[???] �±װ� ���� �ƴ� ���� ���ۿ� ����");
                break;
        }
    }

    void PlayAnimation()
    {
        GameObject Animation = Instantiate(animationPrefabs, transform.position, transform.rotation);
    }
}
