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
                instanceId = System.Guid.NewGuid().ToString(),
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
    public string AddItem(string itemId, int quantity = 1, int level = 1)
    {
        string instId = System.Guid.NewGuid().ToString();
        // (실제로는 중첩 가능한 아이템인지 등 복잡한 로직이 필요)
        UserItem newItem = new UserItem
        {
            instanceId = instId,
            itemDataId = itemId,
            quantity = quantity,
            enhancementLevel = level
        };
        userInventory.Add(newItem);
        return instId;
    }
    
    /// <summary>
    /// 아이템 정보를 반환합니다
    /// </summary>
    public UserItem GetItem(string itemId)
    {
        // (실제로는 중첩 가능한 아이템인지 등 복잡한 로직이 필요)
        UserItem newItem = new UserItem
        {
            instanceId = System.Guid.NewGuid().ToString(),
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
    
    /// <summary>
    /// 인벤토리에서 특정 아이템 인스턴스를 제거합니다.
    /// 객체 참조가 동일한 첫 번째 아이템을 찾아 제거합니다.
    /// </summary>
    /// <param name="itemToRemove">제거할 UserItem 객체 인스턴스</param>
    /// <returns>제거 성공 여부</returns>
    public bool RemoveItem(UserItem itemToRemove)
    {
        if (itemToRemove == null)
        {
            Debug.LogError("제거하려는 아이템이 null입니다.");
            return false;
        }

        // List.Remove()는 전달된 객체와 '참조'가 동일한 첫 번째 요소를 리스트에서 제거합니다.
        bool success = userInventory.Remove(itemToRemove);

        if (success)
        {
            // DataManager를 통해 아이템의 원본 이름을 가져옵니다.
            string itemName = DataManager.Instance.GetItemData(itemToRemove.itemDataId).name;
            Debug.Log($"{itemName} 아이템이 인벤토리에서 제거되었습니다.");
        }
        else
        {
            Debug.LogWarning("제거하려는 아이템을 인벤토리에서 찾을 수 없습니다. 이미 제거되었거나 다른 인스턴스일 수 있습니다.");
        }

        return success;
    }
    
    /// <summary>
    /// 고유 ID(Instance ID)를 이용해 인벤토리에서 아이템을 제거합니다.
    /// </summary>
    public bool RemoveItemByInstanceId(string instanceId)
    {
        UserItem itemToRemove = userInventory.FirstOrDefault(item => item.instanceId == instanceId);

        if (itemToRemove != null)
        {
            return RemoveItem(itemToRemove); // 이미 만든 참조 기반 제거 함수를 재활용
        }
        
        Debug.LogWarning($"Instance ID '{instanceId}'에 해당하는 아이템을 찾을 수 없습니다.");
        return false;
    }
}