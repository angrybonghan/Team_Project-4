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

/*
음무흐흐하하하하하하
나는 주석의 마왕이당
모든 코드를 주석으로 바꿔서
너희들의 코드를 엉@망으로 만들어주마!!!!!!!!!!!!!!!!!!!!!
으히히하하하하하하하하ㅏ하하하하하하ㅏㅎ!!!!
*/