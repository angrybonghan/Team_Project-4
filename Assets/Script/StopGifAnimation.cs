using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopGifAnimation : MonoBehaviour
{
    [Header("이 GIF 애니메이션의 길이 (초)")]
    public float destroyDelay;

    void Start()
    {
        Destroy(gameObject, destroyDelay);
    }
}
