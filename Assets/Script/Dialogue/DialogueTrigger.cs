using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTrigger : MonoBehaviour
{
    [Header("대화 데이터 SO 배열")]
    public DialogueSO[] dialogueSO;

    private static DialogueTrigger instance; // 싱글톤을 위한

    void Awake()
    {
        if (instance == null) // 싱글톤
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
        SceneManager.sceneLoaded += OnSceneLoadedCallback; // 메소드 이름 변경 (충돌 방지 및 명확성)
    }

    // 메모리 누수 방지 (이시현 노트 : 나도 모르겠는데 이렇게 하면 메모리 누수가 방지가 된대)
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedCallback; // 메소드 이름 변경에 따라 수정
    }

    private void OnSceneLoadedCallback(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Stage_1")
        {
            // 오류방지
            if (dialogueSO != null && dialogueSO.Length > 0 && dialogueSO[0] != null)
            {
                DialogueManager.StartDialogue(dialogueSO[0]);
            }
            else
            {
                Debug.LogWarning("DialogueSO 배열이 없음");
            }
        }
    }
}