using UnityEngine;
using UnityEngine.UI; // Required for using the Image component

public class LevelImageChanger : MonoBehaviour
{
    [Header("레벨에 따른 모양 스프라이트")]
    public Sprite[] ballSprites; // Still uses Sprite type for images

    private Image ballImage; // Changed from SpriteRenderer to Image
    private int level;

    private void Awake()
    {
        // GameManager.ballNumber를 가져옴
        // GameManager 클래스와 ballNumber 변수가 다른 스크립트에서 public static으로 선언되어 있어야 접근 가능합니다.
        level = GameManager.ballNumber;

        // 현재 GameObject에서 Image 컴포넌트를 가져옴
        ballImage = GetComponent<Image>();

        // 널 체크 및 유효성 검사 강화
        if (ballImage == null)
        {
            Debug.LogError("Image 컴포넌트가 현재 GameObject에 없습니다. UI Image 오브젝트에 이 스크립트가 붙어있는지 확인해주세요.");
            return;
        }

        if (ballSprites == null || ballSprites.Length == 0)
        {
            Debug.LogError("ballSprites 배열이 비어있거나 할당되지 않았습니다. 인스펙터에서 Sprite들을 할당해주세요.");
            return;
        }

        // level 값 유효성 검사
        // level은 1부터 ballSprites.Length까지여야 합니다.
        // GameManager.ballNumber가 0일 경우 ballSprites[-1]이 되므로, 0인 경우도 에러 처리합니다.
        if (level <= 0 || level > ballSprites.Length) // level이 1부터 시작하고, 배열 인덱스는 0부터 시작하므로 ballSprites.Length와 비교
        {
            Debug.LogError($"현재 레벨 ({level})이 ballSprites 배열의 범위 (1 ~ {ballSprites.Length})를 벗어났습니다. " +
                             "레벨은 1 이상이어야 하고, 배열 길이를 초과할 수 없습니다.");
            return;
        }

        // 레벨에 맞는 스프라이트를 Image 컴포넌트의 sprite 속성에 할당
        ballImage.sprite = ballSprites[level - 1];
    }
}