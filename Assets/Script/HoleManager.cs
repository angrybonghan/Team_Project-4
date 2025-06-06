using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [Header("�� ������ �ִϸ��̼� ������")]
    public GameObject animationPrefabs;

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "MergeBall":   // �� ���ۿ� �������� ����
                Destroy(other.gameObject);
                PlayAnimation(); // �ִϸ��̼� ���� �Լ�
                GameManager.scoredBallInChalk += 1; //���� ��ũ�� �� ���� ���� �ø���
                break;
            case "PlayerBall":  // �÷��̾� ���� ���ۿ� ���� GameManager.cs �� ���� ���� ȣ��
                GameManager.isGameOver = true;
                break;
        }
    }

    void PlayAnimation()
    {
        GameObject Animation = Instantiate(animationPrefabs, transform.position, transform.rotation);
    }
}
