using UnityEngine;
using UnityEngine.UI; // Required for using the Image component

public class LevelImageChanger : MonoBehaviour
{
    [Header("������ ���� ��� ��������Ʈ")]
    public Sprite[] ballSprites;

    private Image ballImage;
    private int level;

    private void Awake()  // ������ �´� ��������Ʈ�� Image ������Ʈ�� sprite �Ӽ��� �Ҵ�
    {
        level = GameManager.ballNumber;

        ballImage = GetComponent<Image>();

        // �� ��ȿ�� �˻�
        if (ballImage == null)
        {
            return;
        }
        if (ballSprites == null || ballSprites.Length == 0)
        {
            return;
        }
        if (level <= 0 || level > ballSprites.Length)
        {
            return;
        }

        ballImage.sprite = ballSprites[level - 1];
    }
}