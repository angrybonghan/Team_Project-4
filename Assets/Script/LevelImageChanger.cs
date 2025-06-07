using UnityEngine;
using UnityEngine.UI; // Required for using the Image component

public class LevelImageChanger : MonoBehaviour
{
    [Header("������ ���� ��� ��������Ʈ")]
    public Sprite[] ballSprites; // Still uses Sprite type for images

    private Image ballImage; // Changed from SpriteRenderer to Image
    private int level;

    private void Awake()
    {
        // GameManager.ballNumber�� ������
        // GameManager Ŭ������ ballNumber ������ �ٸ� ��ũ��Ʈ���� public static���� ����Ǿ� �־�� ���� �����մϴ�.
        level = GameManager.ballNumber;

        // ���� GameObject���� Image ������Ʈ�� ������
        ballImage = GetComponent<Image>();

        // �� üũ �� ��ȿ�� �˻� ��ȭ
        if (ballImage == null)
        {
            Debug.LogError("Image ������Ʈ�� ���� GameObject�� �����ϴ�. UI Image ������Ʈ�� �� ��ũ��Ʈ�� �پ��ִ��� Ȯ�����ּ���.");
            return;
        }

        if (ballSprites == null || ballSprites.Length == 0)
        {
            Debug.LogError("ballSprites �迭�� ����ְų� �Ҵ���� �ʾҽ��ϴ�. �ν����Ϳ��� Sprite���� �Ҵ����ּ���.");
            return;
        }

        // level �� ��ȿ�� �˻�
        // level�� 1���� ballSprites.Length�������� �մϴ�.
        // GameManager.ballNumber�� 0�� ��� ballSprites[-1]�� �ǹǷ�, 0�� ��쵵 ���� ó���մϴ�.
        if (level <= 0 || level > ballSprites.Length) // level�� 1���� �����ϰ�, �迭 �ε����� 0���� �����ϹǷ� ballSprites.Length�� ��
        {
            Debug.LogError($"���� ���� ({level})�� ballSprites �迭�� ���� (1 ~ {ballSprites.Length})�� ������ϴ�. " +
                             "������ 1 �̻��̾�� �ϰ�, �迭 ���̸� �ʰ��� �� �����ϴ�.");
            return;
        }

        // ������ �´� ��������Ʈ�� Image ������Ʈ�� sprite �Ӽ��� �Ҵ�
        ballImage.sprite = ballSprites[level - 1];
    }
}