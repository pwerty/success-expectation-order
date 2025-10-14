using UnityEngine;

/// <summary>
/// 씬 전체의 사운드 재생을 관리하는 싱글톤 클래스입니다.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    private AudioSource audioSource;
    
    [Header("공용 UI 사운드")]
    [Tooltip("버튼 위에 마우스를 올렸을 때 재생될 기본 사운드입니다.")]
    public AudioClip hoverSound;
    [Tooltip("버튼을 클릭했을 때 재생될 기본 사운드입니다.")]
    public AudioClip clickSound;

    void Awake()
    {
        // 싱글톤 패턴: 씬에 SoundManager가 하나만 존재하도록 보장합니다.
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 지정된 오디오 클립을 한 번 재생합니다.
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            // 여러 사운드가 겹쳐서 재생될 수 있도록 PlayOneShot을 사용합니다.
            audioSource.PlayOneShot(clip);
        }
    }
    
    /// <summary>
    /// 미리 설정된 공용 'Hover' 사운드를 재생합니다.
    /// </summary>
    public void PlayHoverSound()
    {
        PlaySound(hoverSound);
    }

    /// <summary>
    /// 미리 설정된 공용 'Click' 사운드를 재생합니다.
    /// </summary>
    public void PlayClickSound()
    {
        PlaySound(clickSound);
    }
}