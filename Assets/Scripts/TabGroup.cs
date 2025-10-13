using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// 탭이 선택되었을 때, 해당 탭이 열어야 할 패널의 ID를 함께 전달하는 이벤트입니다.
/// </summary>
[System.Serializable]
public class TabSelectedIntEvent : UnityEvent<int> { }

public class TabGroup : MonoBehaviour
{
    [Tooltip("이 탭 그룹이 관리할 버튼들입니다.")]
    public List<TabButton> tabButtons;

    [Tooltip("선택된 탭의 버튼 색상입니다.")]
    public Color selectedColor = Color.grey;

    [Tooltip("기본 상태의 탭 버튼 색상입니다.")]
    public Color normalColor = Color.white;

    [Tooltip("시작 시 또는 활성화될 때 기본으로 선택될 탭의 인덱스입니다.")]
    public int defaultTabIndex = 0;

    // "어떤 탭이 눌렸고, 그 탭이 열어야 할 Panel ID는 이것이다"라고 외부에 알리는 이벤트
    public TabSelectedIntEvent OnTabSelectedInt; // <-- 이 줄을 추가
    
    private TabButton currentSelectedTab;
    private int lastSelectedIndex;

    void Start()
    {
        SelectTab(defaultTabIndex);
        lastSelectedIndex = defaultTabIndex;
        // 각 버튼에 리스너를 동적으로 할당합니다.
        for (int i = 0; i < tabButtons.Count; i++)
        {
            tabButtons[i].button.onClick.AddListener(() => SelectTab(tabButtons.IndexOf(tabButtons.Find(item => item.button == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>()))));
        }
    }

    void OnEnable()
    {
        // 패널이 다시 활성화될 때, 마지막으로 선택했던 상태를 복원합니다.
        SelectTab(lastSelectedIndex);
    }

    /// <summary>
    /// 특정 인덱스의 탭을 선택합니다.
    /// </summary>
    /// <param name="index">선택할 탭의 리스트 인덱스</param>
    public void SelectTab(int index)
    {
        if (index < 0 || index >= tabButtons.Count) return;

        TabButton selectedTab = tabButtons[index];

        // 이전에 선택된 탭이 있었다면, 원래 상태로 되돌립니다.
        if (currentSelectedTab != null)
        {
            currentSelectedTab.button.image.color = normalColor;
            currentSelectedTab.button.interactable = true;
        }

        // 새로 선택된 탭을 활성 상태로 변경합니다.
        selectedTab.button.image.color = selectedColor;
        selectedTab.button.interactable = false;

        currentSelectedTab = selectedTab;
        lastSelectedIndex = index;

        // 이 탭이 열어야 할 패널의 ID를 UIManager에게 알립니다.
        if (!string.IsNullOrEmpty(selectedTab.panelIdToOpen))
        {
            OnTabSelectedInt?.Invoke(index);
        }
    }
}

/// <summary>
/// 각 탭 버튼의 정보를 담는 보조 클래스입니다. Inspector 창에서 설정할 수 있습니다.
/// </summary>
[System.Serializable]
public class TabButton
{
    public Button button;
    [Tooltip("이 버튼을 눌렀을 때 열려야 할 UIPanel의 ID입니다.")]
    public string panelIdToOpen;
}