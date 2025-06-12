using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance; // 싱글톤을 위한

    [Header("UI Components")]
    public GameObject dialogueUIPanel; // Dialogue UI 전체를 감싸는 패널 (활성화/비활성화용)
    public Image characterImageUI;   // 캐릭터 이미지
    public TMP_Text characterNameUI;  // 캐릭터 이름
    public TMP_Text dialogueTextUI;       // 대화 텍스트

    public static bool isDialogueActive = false;

    private DialogueSO currentDialogue;
    private int currentDialogueIndex;

    // 현재 대사 타이핑이 완료되었는지 여부
    private bool isTypingComplete = false;

    void Awake()
    {
        if (instance == null) // 싱글톤 박음 (중첩 방지까지)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void StartDialogue(DialogueSO dialogue)
    {
        if (instance == null) // null 오류 원천차단
        {
            Debug.LogError("DialogueManager 인스턴스가 존재하지 않음");
            return;
        }
        if (instance.dialogueUIPanel == null)
        {
            Debug.LogError("다이얼로그 UI 패널 없음");
            return;
        }

        if (isDialogueActive)
        {
            Debug.LogWarning("다이얼로그 실행 중첩됨");
            return;
        }

        // 다 있으면 실행

        instance.dialogueUIPanel.SetActive(true);
        instance.currentDialogue = dialogue;
        instance.currentDialogueIndex = 0;
        isDialogueActive = true;
        instance.StartCoroutine(instance.DisplayDialogue());
    }

    private IEnumerator DisplayDialogue()
    {
        isTypingComplete = false; // 새 대사 시작 시 타이핑 미완료 상태로 설정

        dialogueTextUI.text = "";
        characterNameUI.text = "";
        characterImageUI.sprite = null;

        characterImageUI.sprite = currentDialogue.characterImage[currentDialogueIndex];
        characterNameUI.text = currentDialogue.characterName[currentDialogueIndex];

        string fullText = currentDialogue.DialogueText[currentDialogueIndex];
        for (int i = 0; i <= fullText.Length; i++)
        {
            if (dialogueTextUI != null) dialogueTextUI.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(0.05f);

            // 타이핑 도중 스페이스바를 누르면 바로 전체 텍스트 표시
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (dialogueTextUI != null) dialogueTextUI.text = fullText;
                break; // 루프 종료
            }
        }
        isTypingComplete = true; // 타이핑 완료

        // 다음 대화 진행을 위한 대기 로직은 Update()에서 처리
    }

    private void EndDialogue() // 대화 종료 시 초기화
    {
        isDialogueActive = false;
        isTypingComplete = false;
        currentDialogue = null;
        currentDialogueIndex = 0;
        dialogueTextUI.text = "";
        characterNameUI.text = "";
        characterImageUI.sprite = null;
        instance.dialogueUIPanel.SetActive(false);
    }
    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space)) // 대화가 활성화된 상태에서 스페이스바 누름
        {
            if (isTypingComplete) // 타이핑이 완료된 경우에만 다음 대사로 진행
            {
                currentDialogueIndex++;
                if (currentDialogueIndex < currentDialogue.DialogueText.Length)
                {
                    StartCoroutine(DisplayDialogue());
                }
                else
                {
                    EndDialogue();
                }
            }
            else // 타이핑이 아직 진행 중이라면 바로 타이핑 완료 상태로
            {
                StopAllCoroutines(); // 코루틴 전부 중단
                if (dialogueTextUI != null) dialogueTextUI.text = currentDialogue.DialogueText[currentDialogueIndex]; // 전체 텍스트 표시
                isTypingComplete = true; // 타이핑 완료 상태로 설정
            }
        }
    }
}