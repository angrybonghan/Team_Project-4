using UnityEngine;

[CreateAssetMenu(fileName = "My New Dialogue", menuName = "Dialogue System/Dialogue Line")]

public class DialogueSO : ScriptableObject
{
    [Header("ĳ���� �̹���")]
    public Sprite[] characterImage;
    [Header("ĳ���� �̸�")]
    public string[] characterName;
    [Header("��ȭ�� ����Ʈ")]
    public string[] DialogueText;
}
