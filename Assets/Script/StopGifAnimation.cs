using System.Collections;
using System.Collections.Generic;
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
