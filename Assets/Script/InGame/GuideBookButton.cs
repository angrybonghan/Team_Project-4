using UnityEngine;
using UnityEngine.UI;

public class GuideBookButton : MonoBehaviour
{
    private Button selfButton;
    private AudioClip OpenBook;

    void Awake()
    {
        selfButton = GetComponent<Button>();

        // (아래는 AI 설명임)
        // 버튼의 onClick 이벤트에 OnButtonClick 함수를 리스너로 추가합니다.
        // 즉, 버튼이 클릭될 때마다 OnButtonClick 함수가 호출되도록 설정합니다.
        selfButton.onClick.AddListener(OnButtonClick);
    }

    private void Start()
    {
        OpenBook = DataManager.OpenBook;
    }

    public void OnButtonClick()
    {
        SoundManager.PlaySound(OpenBook, 1, 0.7f);
        pauseControl.TurnOnGuidebook();
    }

}