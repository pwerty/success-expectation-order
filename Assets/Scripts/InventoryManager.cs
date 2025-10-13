using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    [SerializeField] private StartingInventorySO startingInventoryPreset;
    // 사용자가 소유한 모든 아이템 '인스턴스'를 보관하는 리스트 (지갑)
    private List<UserItem> userInventory = new List<UserItem>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지되도록 설정
        }
        else
        {
            Destroy(gameObject);
            
        }
        InitializeFromPreset();
    }
    
    /// <summary>
    /// 지정된 ScriptableObject 프리셋을 기반으로 인벤토리를 초기화합니다.
    /// </summary>
    private void InitializeFromPreset()
    {
        userInventory.Clear(); // 만약을 위해 기존 인벤토리를 비웁니다.

        if (startingInventoryPreset == null)
        {
            Debug.LogWarning("초기 인벤토리 프리셋이 지정되지 않았습니다.");
            return;
        }

        foreach (var initialItem in startingInventoryPreset.startingItems)
        {
            // 템플릿 정보를 바탕으로 실제 UserItem 인스턴스를 생성합니다.
            UserItem newItem = new UserItem
            {
                itemDataId = initialItem.itemDataId,
                quantity = initialItem.quantity,
                enhancementLevel = initialItem.enhancementLevel
            };
            userInventory.Add(newItem);
        }
        
        Debug.Log($"{userInventory.Count}개의 아이템으로 인벤토리를 초기화했습니다.");
    }

    /// <summary>
    /// 저장된 데이터로 인벤토리를 복원합니다. SaveManager가 호출합니다.
    /// </summary>
    public void RestoreInventory(List<UserItem> loadedItems)
    {
        userInventory = loadedItems;
    }

    /// <summary>
    /// UI에 표시하기 위해, 플레이어가 소유한 모든 차 목록을 반환합니다.
    /// </summary>
    public List<UserItem> GetPlayerCars()
    {
        // LINQ를 사용하여 ID가 "CAR_"로 시작하는 모든 아이템을 찾아 리스트로 반환
        return userInventory.Where(item => item.itemDataId.StartsWith("CAR_")).ToList();
    }
    
    public List<UserItem> GetPlayerDrivers()
    {
        // LINQ를 사용하여 ID가 "CAR_"로 시작하는 모든 아이템을 찾아 리스트로 반환
        return userInventory.Where(item => item.itemDataId.StartsWith("DRV_")).ToList();
    }
    
    /// <summary>
    /// 새로운 아이템을 인벤토리에 추가합니다. (예: 뽑기 성공 시)
    /// </summary>
    public void AddItem(string itemId, int quantity = 1, int level = 1)
    {
        // (실제로는 중첩 가능한 아이템인지 등 복잡한 로직이 필요)
        UserItem newItem = new UserItem
        {
            itemDataId = itemId,
            quantity = quantity,
            enhancementLevel = level
        };
        userInventory.Add(newItem);
    }
    
    /// <summary>
    /// 아이템 정보를 반환합니다
    /// </summary>
    public UserItem GetItem(string itemId)
    {
        // (실제로는 중첩 가능한 아이템인지 등 복잡한 로직이 필요)
        UserItem newItem = new UserItem
        {
            itemDataId = itemId,
            quantity = 1,
            enhancementLevel = 1
        };

        return newItem;
    }
    
    // 현재 인벤토리 상태를 저장하기 위해 SaveManager에게 전달하는 함수
    public List<UserItem> GetCurrentInventoryData()
    {
        return userInventory;
    }
}