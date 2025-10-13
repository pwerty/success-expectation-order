using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// TabGroup의 요청을 받아 자식 UIPanel들을 켜고 끄며,
/// 그 사실을 전역 이벤트로 방송하는 '중간 관리자'입니다.
/// </summary>
public class ContentSwitcher : MonoBehaviour
{
    // 💡 변경점 1: GameObject 대신 UIPanel 리스트를 사용합니다.
    [Tooltip("이 스위처가 관리할 UIPanel 목록입니다. TabGroup의 버튼 순서와 일치시켜야 합니다.")]
    [SerializeField] private List<UIPanel> contentPanels;
    
    // 💡 변경점 2: GameObject 대신 UIPanel을 추적합니다.
    private UIPanel currentActiveContent;

    void Awake()
    {
        // UIManager의 Start()보다 먼저 실행되어, 자식들을 완벽한 초기 상태로 만듭니다.
        foreach (var panel in contentPanels)
        {
            panel.Hide();
        }
    }

    /// <summary>
    /// 지정된 인덱스의 콘텐츠를 활성화하고 그 사실을 전역에 방송합니다.
    /// </summary>
    /// <param name="index">활성화할 콘텐츠의 리스트 인덱스</param>
    public void ShowContentByIndex(int index)
    {
        if (index < 0 || index >= contentPanels.Count) return;

        if (currentActiveContent != null)
        {
            currentActiveContent.Hide();
        }

        UIPanel targetPanel = contentPanels[index];
        
        // 💡 변경점 3: SetActive(true) 대신 Show()를 호출합니다.
        targetPanel.Show();
        currentActiveContent = targetPanel;
        
        // 💡 여기가 핵심: "내가 이 패널을 열었다!" 라고 UIManager와 똑같은 방식으로 전역 방송을 합니다.
        UIEvents.PanelShown(targetPanel);
    }
}