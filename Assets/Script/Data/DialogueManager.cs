using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance; // �̱����� ����

    [Header("UI Components")]
    public GameObject dialogueUIPanel; // Dialogue UI ��ü�� ���δ� �г� (Ȱ��ȭ/��Ȱ��ȭ��)
    public Image characterImageUI;   // ĳ���� �̹���
    public TMP_Text characterNameUI;  // ĳ���� �̸�
    public TMP_Text dialogueTextUI;       // ��ȭ �ؽ�Ʈ

    public static bool isDialogueActive = false;

    private DialogueSO currentDialogue;
    private int currentDialogueIndex;

    // ���� ��� Ÿ������ �Ϸ�Ǿ����� ����
    private bool isTypingComplete = false;

    void Awake()
    {
        if (instance == null) // �̱��� ���� (��ø ��������)
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
        if (instance == null) // null ���� ��õ����
        {
            Debug.LogError("DialogueManager �ν��Ͻ��� �������� ����");
            return;
        }
        if (instance.dialogueUIPanel == null)
        {
            Debug.LogError("���̾�α� UI �г� ����");
            return;
        }

        if (isDialogueActive)
        {
            Debug.LogWarning("���̾�α� ���� ��ø��");
            return;
        }

        // �� ������ ����

        instance.dialogueUIPanel.SetActive(true);
        instance.currentDialogue = dialogue;
        instance.currentDialogueIndex = 0;
        isDialogueActive = true;
        instance.StartCoroutine(instance.DisplayDialogue());
    }

    private IEnumerator DisplayDialogue()
    {
        isTypingComplete = false; // �� ��� ���� �� Ÿ���� �̿Ϸ� ���·� ����

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

            // Ÿ���� ���� �����̽��ٸ� ������ �ٷ� ��ü �ؽ�Ʈ ǥ��
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (dialogueTextUI != null) dialogueTextUI.text = fullText;
                break; // ���� ����
            }
        }
        isTypingComplete = true; // Ÿ���� �Ϸ�

        // ���� ��ȭ ������ ���� ��� ������ Update()���� ó��
    }

    private void EndDialogue() // ��ȭ ���� �� �ʱ�ȭ
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
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space)) // ��ȭ�� Ȱ��ȭ�� ���¿��� �����̽��� ����
        {
            if (isTypingComplete) // Ÿ������ �Ϸ�� ��쿡�� ���� ���� ����
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
            else // Ÿ������ ���� ���� ���̶�� �ٷ� Ÿ���� �Ϸ� ���·�
            {
                StopAllCoroutines(); // �ڷ�ƾ ���� �ߴ�
                if (dialogueTextUI != null) dialogueTextUI.text = currentDialogue.DialogueText[currentDialogueIndex]; // ��ü �ؽ�Ʈ ǥ��
                isTypingComplete = true; // Ÿ���� �Ϸ� ���·� ����
            }
        }
    }
}