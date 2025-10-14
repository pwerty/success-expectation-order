using UnityEngine;

/// <summary>
/// 3D 아이템 미리보기와 관련된 모든 것을 총괄하는 싱글턴입니다.
/// </summary>
public class PreviewManager : MonoBehaviour
{
    public static PreviewManager Instance { get; private set; }

    [Tooltip("3D 모델이 소환될 위치입니다.")]
    [SerializeField] private Transform spawnPoint;

    // 현재 선택되어 표시되고 있는 아이템의 정보입니다.
    public UserItem CurrentSelectedItem { get; private set; }
    
    // 현재 소환되어 있는 3D 모델 게임 오브젝트입니다.
    private GameObject currentPreviewObject;

    void Awake()
    {
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

    void OnEnable()
    {
        // 패널이 열렸다는 전역 방송을 구독합니다.
        UIEvents.OnPanelShown += HandlePanelChange;
    }

    void OnDisable()
    {
        // 방송 구독을 해제합니다.
        UIEvents.OnPanelShown -= HandlePanelChange;
    }

    /// <summary>
    /// UIEvents 방송을 수신했을 때 실행될 함수입니다.
    /// </summary>
    private void HandlePanelChange(UIPanel panel)
    {
        // 특정 패널(예: "Garage")이 열렸을 때만 반응하도록 합니다.
        if (panel.panelId == "DriverPanel")
        {
            // 인벤토리에서 첫 번째 차를 찾아 표시합니다.
            var playerCars = InventoryManager.Instance.GetPlayerCars();
            if (playerCars != null && playerCars.Count > 0)
            {
                DisplayItem(playerCars[0]);
            }
        }
        else
        {
            // 다른 패널이 열리면, 기존에 있던 미리보기 모델을 숨깁니다.
            DisplayItem(null);
        }
    }

    /// <summary>
    /// 지정된 아이템을 3D 필드에 표시합니다.
    /// </summary>
    /// <param name="itemToDisplay">표시할 아이템 (null이면 숨김)</param>
    public void DisplayItem(UserItem itemToDisplay)
    {
        // 1. 이전에 있던 모델이 있다면 파괴합니다.
        if (currentPreviewObject != null)
        {
            Destroy(currentPreviewObject);
        }

        // 2. 현재 선택된 아이템 정보를 업데이트합니다.
        CurrentSelectedItem = itemToDisplay;

        // 3. 표시할 아이템이 있다면, 새로운 모델을 소환합니다.
        if (itemToDisplay != null)
        {
            ItemData data = DataManager.Instance.GetItemData(itemToDisplay.itemDataId);
            if (data.modelPrefab != null)
            {
                currentPreviewObject = Instantiate(data.modelPrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
        
        // TODO: "미리보기가 업데이트되었다!"는 새로운 전역 이벤트를 방송하여 UI가 반응하게 합니다.
        // PreviewEvents.PreviewUpdated(CurrentSelectedItem);
    }
}