using UnityEngine;

public class StopGifAnimation : MonoBehaviour
{
    [Header("�� GIF �ִϸ��̼��� ���� (��)")]
    public float destroyDelay;

    void Start()
    {
        Destroy(gameObject, destroyDelay);
    }
}

/*
��������������������
���� �ּ��� �����̴�
��� �ڵ带 �ּ����� �ٲ㼭
������� �ڵ带 ��@������ ������ָ�!!!!!!!!!!!!!!!!!!!!!
���������������������Ϥ������������Ϥ���!!!!
*/