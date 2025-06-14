using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [Header("�� ������ �ִϸ��̼� ������")]
    public GameObject animationPrefabs;

    [Header("8�� ������")]
    public GameObject eightBallPrefabs;

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

            case "OB_Level_Up": //���� �� ��
                Destroy(other.gameObject);
                PlayAnimation(); // �ִϸ��̼� ����
                GameManager.scoredBallInChalk += 2; //���� ��ũ�� �� ���� ���� �ΰ� �ø���
                displayBall.DisplayBallCount += 2;
                break;

            case "OB_Level_Down":   //���� �ٿ� ��
                Destroy(other.gameObject);
                PlayAnimation(); // �ִϸ��̼� ����
                if (GameManager.ballNumber >= 2)    // �� ������ 0 �Ʒ��� ���������� ����
                {
                    GameManager.ballNumber -= 2;    // ��ü �� ������ 2 ������
                    GameManager.scoredBallInChalk++;    // ��ũ�� ���� �ϳ� ���� ������ �ش� (�� ������Ʈ�� ��Ű�� ����)
                    // ��� -2 + 1 = -1
                    // ���������� 1�� ����
                }
                break;

            case "OB_Copy":
                PlayAnimation(); // �ִϸ��̼� ����

                for (int i = 0; i < 2; i++)
                {
                    Vector3 CopyPos = new Vector3
                        (
                        UnityEngine.Random.Range(-1f, 1f),
                        UnityEngine.Random.Range(-1f, 1f),
                        0
                        );
                    GameObject EightBall = Instantiate(eightBallPrefabs, CopyPos, transform.rotation);
                }
                Destroy(other.gameObject);
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
