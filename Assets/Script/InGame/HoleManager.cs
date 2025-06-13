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
                GameManager.scoredBallInChalk += 1; //���� ��ũ�� �� ���� ���� �ø���
                Debug.Log("MergeBall Ȧ�Ŵ���");
                displayBall.DisplayBallCount += 1;
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
