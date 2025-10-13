using System;
using UnityEngine;

/// <summary>
/// 모든 UI 패널의 기본 클래스입니다.
/// UIManager가 패널을 식별하고 관리하기 위한 '신분증' 역할을 합니다.
/// </summary>
public class UIPanel : MonoBehaviour
{
    [Tooltip("UIManager가 이 패널을 찾기 위해 사용할 고유 ID입니다. 오타 없이 정확하게 입력해주세요.")]
    public string panelId;
    public UIMusicLayer musicLayer = UIMusicLayer.None;

    // 💡 아래 4줄을 추가합니다.
    [Header("카메라 제어")]
    [Tooltip("이 패널이 활성화될 때 카메라를 움직일지 결정합니다.")]
    public bool moveCameraOnShow = false;
    public Vector3 cameraTargetPosition;
    public Vector3 cameraTargetRotation;
    public bool isControlledByManager = false;
    
    // 애니메이션 적용 여부를 정하기 위해 논리적 정의 여부를 확인
    public bool isLogicalPanel = false;

    private void Awake()
    {
        panelId = name;
    }

    /// <summary>
    /// 패널이 화면에 표시될 때 호출됩니다.
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 패널이 화면에서 사라질 때 호출됩니다.
    /// </summary>
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}