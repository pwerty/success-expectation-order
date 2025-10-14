using UnityEngine;
using System.Collections.Generic;

public class TabVisualSyncManager : MonoBehaviour
{
    [SerializeField] private List<TabGroup> managedTabGroups;

    void OnEnable()
    {
        UIEvents.OnPanelShown += HandlePanelChange;
    }

    void OnDisable()
    {
        UIEvents.OnPanelShown -= HandlePanelChange;
    }

    private void HandlePanelChange(UIPanel panel)
    {
        foreach (TabGroup group in managedTabGroups)
        {
            // TabGroup에게 "이 Panel ID를 가진 버튼이 너한테 있니?" 라고 물어봅니다.
            int tabIndex = FindTabIndexInGroup(group, panel.panelId);
            print("검색 실시 :" + panel.panelId);

            if (tabIndex != -1)
            {
                // 💡 변경점: 직접 색상을 바꾸는 대신, TabGroup에게 "너의 O번 탭을 강제로 선택 상태로 만들어" 라고 지시합니다.
                group.SelectTab(tabIndex, true); // force = true로 즉시 상태 변경
                return;
            }
            else
            {
                print("그런거 없대!!!");
            }
        }
    }
    
    /// <summary>
    /// 지정된 TabGroup 내에서 특정 Panel ID와 연결된 탭의 인덱스를 찾습니다.
    /// </summary>
    private int FindTabIndexInGroup(TabGroup group, string panelId)
    {
        
        for (int i = 0; i < group.tabButtons.Count; i++)
        {
            if (group.tabButtons[i].panelIdToOpen == panelId)
            {
                return i;
            }
        }
        return -1;
    }
}