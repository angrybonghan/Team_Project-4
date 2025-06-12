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
        if (scene.name == "Stage_1")
        {
            // ��������
            if (dialogueSO != null && dialogueSO.Length > 0 && dialogueSO[0] != null)
            {
                DialogueManager.StartDialogue(dialogueSO[0]);
            }
            else
            {
                Debug.LogWarning("DialogueSO �迭�� ����");
            }
        }
    }
}