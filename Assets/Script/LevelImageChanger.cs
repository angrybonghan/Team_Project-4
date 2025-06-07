using UnityEngine;
using UnityEngine.UI; // Required for using the Image component

public class LevelImageChanger : MonoBehaviour
{
    [Header("레벨에 따른 모양 스프라이트")]
    public Sprite[] ballSprites;

    private Image ballImage;
    private int level;

    private void Awake()  // 레벨에 맞는 스프라이트를 Image 컴포넌트의 sprite 속성에 할당
    {
        level = GameManager.ballNumber;

        ballImage = GetComponent<Image>();

        // 값 유효성 검사
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