using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public static AudioSource audioSource { get; private set; } // 오디오 소스 컴포넌트 참조

    [Header("1-버튼")]
    public AudioClip button_;
    [Header("2-다이얼로그")]
    public AudioClip dialogue_;
    [Header("3-다이얼로그")]
    public AudioClip GuideText_;



    public static AudioClip button { get; private set; }
    public static AudioClip dialogue { get; private set; }
    public static AudioClip GuideText {  get; private set; }

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

        button = button_ ;
        dialogue = dialogue_ ;
        GuideText = GuideText_ ;
    }

    public static void PlaySound(int soundType)
    {
        switch (soundType)
        {
            case 1:
                audioSource.PlayOneShot(button);
                break;
            case 2:
                audioSource.PlayOneShot(dialogue);
                break;
            case 3:
                audioSource.PlayOneShot(GuideText);
                break;
        }

        
    }
}
