using UnityEngine;
using UnityEngine.UI;

public class GuideLeftButton : MonoBehaviour
{
    private Button selfButton;

    void Awake()
    {
        selfButton = GetComponent<Button>();

        // (아래는 AI 설명임)
        // 버튼의 onClick 이벤트에 OnButtonClick 함수를 리스너로 추가합니다.
        // 즉, 버튼이 클릭될 때마다 OnButtonClick 함수가 호출되도록 설정합니다.
        selfButton.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        pauseControl.GotoP1();
    }
}