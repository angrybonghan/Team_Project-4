using UnityEngine;

// SoundManager 클래스는 MonoBehaviour를 상속받지 않음.
// 이렇게 하면 씬에 오브젝트로 존재할 필요 없이 순수 static 유틸리티 클래스가 됨.
public static class SoundManager
{
    // static AudioSource는 PlaySound를 호출할 때 동적으로 생성하거나,
    // 미리 씬에 배치된 AudioSource를 참조하여 사용할 수 있도록 해야 함.
    // 여기서는 동적으로 생성하는 방식으로 구현함.
    private static AudioSource _audioSource;

    // AudioSource가 없으면 생성하여 반환하는 프로퍼티
    private static AudioSource GlobalAudioSource
    {
        get
        {
            if (_audioSource == null)
            {
                // 게임 시작 시 이 SoundManager가 붙는 GameObject가 없을 수 있으므로,
                // 새로운 임시 GameObject를 생성하여 AudioSource를 붙임.
                // DontDestroyOnLoad를 사용하여 씬 전환에도 유지되도록 함.
                GameObject soundGameObject = new GameObject("GlobalSoundManagerAudioSource");
                _audioSource = soundGameObject.AddComponent<AudioSource>();
                Object.DontDestroyOnLoad(soundGameObject);
            }
            return _audioSource;
        }
    }

    /// <summary>
    /// 사운드 재생 : 오디오클립, 불륨, 피치
    /// </summary>
    /// <param name="clip">재생할 오디오 클립.</param>
    /// <param name="volume">사운드 크기 (0.0f ~ 1.0f).</param>
    /// <param name="pitch">피치 (0.0f ~ 3.0f, 기본 1.0f).</param>
    public static void PlaySound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clip == null)
        {
            Debug.LogWarning("재생할 오디오 클립이 null.");
            return;
        }

        AudioSource source = GlobalAudioSource;

        // PlayOneShot 전에 피치와 볼륨을 설정.
        // PlayOneShot은 직접적인 volumeScale 인자를 제공하지만, pitch는 제공하지 않으므로
        // AudioSource의 pitch를 설정하고 재생.
        source.pitch = Mathf.Clamp(pitch, 0.01f, 3.0f);
        source.volume = Mathf.Clamp01(volume);

        source.PlayOneShot(clip, source.volume);
    }

    /// <summary>
    /// SoundManager가 사용하는 AudioSource에서 재생되는 모든 소리를 즉시 정지.
    /// </summary>
    public static void StopSound()
    {
        if (_audioSource != null && _audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }

    // 선택 사항: GlobalAudioSource의 볼륨을 설정하는 static 메서드
    public static void SetGlobalVolume(float volume)
    {
        GlobalAudioSource.volume = Mathf.Clamp01(volume);
    }

    // 선택 사항: GlobalAudioSource의 피치를 설정하는 static 메서드 (주로 모든 소리의 피치 변경 시)
    public static void SetGlobalPitch(float pitch)
    {
        GlobalAudioSource.pitch = Mathf.Clamp(pitch, 0.01f, 3.0f);
    }
}