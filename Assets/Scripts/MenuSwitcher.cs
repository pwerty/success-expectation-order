using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Panel ID와 그에 해당하는 Context Menu를 매핑하기 위한 데이터 구조입니다.
/// </summary>
[System.Serializable]
public class ContextMenuMapping
{
    public string panelId;
    public GameObject contextMenu;
}

/// <summary>
/// 전역 UI 이벤트에 반응하여 상황에 맞는 메뉴(NavBar, ContextMenu 등)를 전환합니다.
/// </summary>
public class MenuSwitcher : MonoBehaviour
{
    [Tooltip("기본으로 표시될 NavBar 또는 메뉴입니다.")]
    [SerializeField] private GameObject defaultMenu;
    
    [Tooltip("특정 Panel ID가 활성화되었을 때 대신 표시될 Context Menu 목록입니다.")]
    [SerializeField] private List<ContextMenuMapping> contextMenuMappings;

    // 빠른 조회를 위해 List를 Dictionary로 변환하여 저장합니다.
    private Dictionary<string, GameObject> menuRegistry = new Dictionary<string, GameObject>();

    private GameObject currentActiveMenu;

    void Awake()
    {
        // Inspector에서 설정한 리스트를 Dictionary로 변환하여 '주소록'을 만듭니다.
        foreach (var mapping in contextMenuMappings)
        {
            if (!menuRegistry.ContainsKey(mapping.panelId))
            {
                menuRegistry.Add(mapping.panelId, mapping.contextMenu);
            }
        }
    }

    void Start()
    {
        foreach (var mapping in contextMenuMappings)
        {
            if (!menuRegistry.ContainsKey(mapping.panelId))
            {
                mapping.contextMenu.SetActive(false);
            }
        }
    }

    void OnEnable()
    {
        // UI 방송을 구독합니다.
        UIEvents.OnPanelShown += HandlePanelChange;
    }

    void OnDisable()
    {
        // UI 방송 구독을 해제합니다.
        UIEvents.OnPanelShown -= HandlePanelChange;
    }

    /// <summary>
    /// 패널이 열렸다는 방송을 수신했을 때 호출될 함수입니다.
    /// </summary>
    private void HandlePanelChange(UIPanel panel)
    {
        if (panel.isLogicalPanel) return;
        // 현재 활성화된 메뉴가 있다면 우선 끈다.
        if (currentActiveMenu != null)
        {
            currentActiveMenu.SetActive(false);
        }

        // 1. 주소록(Dictionary)에서 방송된 패널 ID에 해당하는 특별한 Context Menu가 있는지 찾아봅니다.
        if (menuRegistry.TryGetValue(panel.panelId, out GameObject contextMenuToShow))
        {
            // 2. 있다면, 그 Context Menu를 켠다.
            contextMenuToShow.SetActive(true);
            currentActiveMenu = contextMenuToShow;
        }
        else
        {
            print("없다는데요");
                
            }
        }
    }