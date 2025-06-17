using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public static AudioSource audioSource { get; private set; } // 오디오 소스 컴포넌트 참조

    [Header("1-버튼")]
    public AudioClip button;
    [Header("2-다이얼로그")]
    public AudioClip dialogue;
    [Header("3-가이드 텍스트")]
    public AudioClip GuideText;
    [Header("4-랜덤볼")]
    public AudioClip RandomBall;
    [Header("5-손가락 튕기기")]
    public AudioClip Tick;
    [Header("6-콤보")]
    public AudioClip Blast;


    void Awake()
    {
        if (Instance == null) // 신 변경에도 유지하는 싱글톤
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // AudioSource 컴포넌트
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(int soundType)
    {
        audioSource.pitch = 1f;

        switch (soundType)
        {
            case 1:
                audioSource.PlayOneShot(Instance.button);
                break;
            case 2:
                audioSource.pitch = 0.7f;
                audioSource.PlayOneShot(Instance.dialogue);
                break;
            case 3:
                audioSource.PlayOneShot(Instance.GuideText);
                break;
            case 4:
                audioSource.PlayOneShot(Instance.RandomBall);
                break;
            case 5:
                audioSource.PlayOneShot(Instance.Tick);
                break;
            case 6:
                audioSource.pitch = 1.5f;
                audioSource.PlayOneShot(Instance.Blast);
                break;
        }

        
    }
}
