using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // FirstOrDefault()를 사용하기 위해 필요

/// <summary>
/// UI 상태에 따른 음악 레이어 정의
/// Inspector에서 쉽게 선택할 수 있는 드롭다운 메뉴를 제공
/// </summary>
public enum UIMusicLayer
{
    // 필요에 따라 얼마든지 추가하거나 이름을 변경하세요.
    None,       // 음악 없음
    Home,       // 홈 화면
    Garage,     // 격납고/인벤토리
    Gacha,      // 뽑기 화면
    Upgrade,    // 강화 화면
    InGame,     // 인게임 플레이
}

/// <summary>
/// UIMusicLayer에 매핑될 오디오 볼륨 믹스 프리셋입니다.
/// [System.Serializable]을 통해 Inspector 창에 노출됩니다.
/// </summary>
[System.Serializable]
public class AudioMixPreset
{
    public UIMusicLayer layer;
    [Range(0f, 1f)] public float drumVolume = 1f;
    [Range(0f, 1f)] public float vocalVolume = 1f;
    [Range(0f, 1f)] public float instVolume = 1f;
    [Range(0f, 1f)] public float bassVolume = 1f;
}



public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("오디오 소스")]
    [SerializeField] private AudioSource DrumSource;
    [SerializeField] private AudioSource VocalSource;
    [SerializeField] private AudioSource InstSource;
    [SerializeField] private AudioSource BassSource;

    [Header("오디오 믹스 설정")]
    [Tooltip("UI 레이어별로 적용될 오디오 믹스 프리셋 목록입니다.")]
    [SerializeField] private List<AudioMixPreset> mixPresets;

    [Tooltip("볼륨이 변경될 때 걸리는 시간(초)입니다.")]
    [SerializeField] private float fadeDuration = 0.5f;

    // 현재 실행 중인 볼륨 페이드 코루틴을 저장하여 중복 실행을 방지합니다.
    private Coroutine activeFadeCoroutine;

    void Awake()
    {
        // --- 싱글턴 패턴 구현 ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 지정된 UI 음악 레이어에 맞게 오디오 믹스를 부드럽게 변경합니다.
    /// UIManager가 이 함수를 호출하게 됩니다.
    /// </summary>
    /// <param name="layer">변경할 UI 음악 레이어</param>
    public void SetMusicLayer(UIMusicLayer layer)
    {
        // 프리셋 목록에서 요청된 레이어에 맞는 프리셋을 찾습니다.
        AudioMixPreset preset = mixPresets.FirstOrDefault(p => p.layer == layer);

        if (preset != null)
        {
            // 이전에 실행 중이던 페이드 코루틴이 있다면 즉시 중지합니다.
            if (activeFadeCoroutine != null)
            {
                StopCoroutine(activeFadeCoroutine);
            }
            // 새로운 페이드 코루틴을 시작하고, 그 참조를 저장합니다.
            activeFadeCoroutine = StartCoroutine(FadeVolumesCoroutine(preset));
        }
        else
        {
            Debug.LogWarning($"요청한 UIMusicLayer '{layer}'에 대한 프리셋을 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 지정된 프리셋의 목표 볼륨까지 부드럽게 볼륨을 조절하는 코루틴입니다.
    /// </summary>
    private IEnumerator FadeVolumesCoroutine(AudioMixPreset targetPreset)
    {
        // 시작 시점의 각 오디오 소스 볼륨을 저장합니다.
        float startDrum = DrumSource.volume;
        float startVocal = VocalSource.volume;
        float startInst = InstSource.volume;
        float startBass = BassSource.volume;

        float elapsedTime = 0f;

        // fadeDuration 시간 동안 반복하여 볼륨을 서서히 변경합니다.
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fadeDuration);

            // Lerp (선형 보간)를 사용하여 현재 볼륨과 목표 볼륨 사이의 값을 계산합니다.
            DrumSource.volume = Mathf.Lerp(startDrum, targetPreset.drumVolume, progress);
            VocalSource.volume = Mathf.Lerp(startVocal, targetPreset.vocalVolume, progress);
            InstSource.volume = Mathf.Lerp(startInst, targetPreset.instVolume, progress);
            BassSource.volume = Mathf.Lerp(startBass, targetPreset.bassVolume, progress);

            // 다음 프레임까지 대기합니다.
            yield return null;
        }

        // 페이드가 끝난 후, 목표 볼륨으로 정확하게 값을 설정합니다. (부동 소수점 오차 보정)
        DrumSource.volume = targetPreset.drumVolume;
        VocalSource.volume = targetPreset.vocalVolume;
        InstSource.volume = targetPreset.instVolume;
        BassSource.volume = targetPreset.bassVolume;
    }
}