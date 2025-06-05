using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [Header("�� ������ ��ȣ (1..6)")]
    public int holeID = 0;

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
        if (holeID == null) // ������ �ۼ����� �ʾ��� ���, ��� ��ȯ
        {
            Debug.LogError($"ID ������ NULL ��");
            return;
        }

        switch (holeID) // ������ ��ġ ID�� ���� �ٸ� ����
        {
            case 1:
                Debug.Log("Hole 1"); // ���� 1 : ���� ��
                break;
            case 2:
                Debug.Log("Hole 2"); // ���� 2 : �߾� ��
                break;
            case 3:
                Debug.Log("Hole 3"); // ���� 3 : ������ ��
                break;
            case 4:
                Debug.Log("Hole 4"); // ���� 4 : ���� �Ʒ�
                break;
            case 5:
                Debug.Log("Hole 5"); // ���� 5 : �߾� �Ʒ�
                break;
            case 6:
                Debug.Log("Hole 6"); // ���� 6 : ������ �Ʒ�
                break;
            default:
                Debug.LogError($"ID ������ 1���� 6 ������ ������ �ƴ�! // ���� �� : {holeID}");
                // Ȥ�� �𸣴ϱ� ���� ó��.
                break;
        }
    }
}
