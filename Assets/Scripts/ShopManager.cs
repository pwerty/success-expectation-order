using UnityEngine;

/// <summary>
/// 상점의 구매 로직을 담당하는 싱글턴입니다.
/// </summary>
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

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
    }

    /// <summary>
    /// 지정된 ID의 아이템 구매를 시도합니다.
    /// </summary>
    public void PurchaseItem(string itemId)
    {
        Debug.Log($"아이템 구매 시도: {itemId}");

        // TODO 1: 재화 확인 로직
        // ItemData itemToBuy = DataManager.Instance.GetItemData(itemId);
        // int price = GetPrice(itemId); // 별도의 가격표에서 가격 조회
        // if (PlayerData.gold < price)
        // {
        //     Debug.Log("골드가 부족합니다!");
        //     // TODO: "골드 부족" 팝업 표시
        //     return;
        // }

        // TODO 2: 재화 차감
        // PlayerData.gold -= price;

        // 3. 인벤토리에 아이템 추가
        InventoryManager.Instance.AddItem(itemId);

        // TODO 4: "구매 완료!" 시각 피드백 (팝업 등)
        Debug.Log($"{itemId} 구매 완료! 인벤토리에 추가되었습니다.");
    }
}