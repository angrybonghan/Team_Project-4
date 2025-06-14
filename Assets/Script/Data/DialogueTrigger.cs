using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTrigger : MonoBehaviour
{
    [Header("��ȭ ������ SO �迭")]
    public DialogueSO[] dialogueSO;

    private static DialogueTrigger instance; // �̱����� ����

    void Awake()
    {
        if (instance == null) // �̱���
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoadedCallback; // �޼ҵ� �̸� ���� (�浹 ���� �� ��Ȯ��)
    }

    // �޸� ���� ���� (�̽��� ��Ʈ : ���� �𸣰ڴµ� �̷��� �ϸ� �޸� ������ ������ �ȴ�)
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedCallback; // �޼ҵ� �̸� ���濡 ���� ����
    }

    private void OnSceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Stage_1":
                StartDialogue(1);
                break;

            case "Stage_2":
                StartDialogue(2);
                break;

            case "Stage_3":
                StartDialogue(3);
                break;

            case "Stage_4":
                StartDialogue(4);
                break;

            case "Stage_5":
                StartDialogue(5);
                break;

        }
    }

    private void StartDialogue(int index)
    {
        // ��������
        if (dialogueSO[index-1] != null)
        {
            DialogueManager.StartDialogue(dialogueSO[0]);
        }
        else
        {
            Debug.LogWarning("DialogueSO �迭�� ����");
        }
    }
}