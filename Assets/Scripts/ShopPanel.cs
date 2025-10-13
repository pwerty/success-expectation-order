using UnityEngine;
using System.Collections.Generic;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private ShopItemSlot shopItemSlotPrefab;
    
    // 💡 Inspector 창에서 상점에 진열할 아이템들을 미리 등록합니다.
    [SerializeField] private List<ItemData> itemsForSale;

    void Start()
    {
        PopulateShop();
    }

    /// <summary>
    /// 등록된 아이템들로 상점을 채웁니다.
    /// </summary>
    private void PopulateShop()
    {
        foreach (var itemData in itemsForSale)
        {
            ShopItemSlot newSlot = Instantiate(shopItemSlotPrefab, contentParent);
            
            // 💡 슬롯의 구매 버튼이 눌리면, ShopManager의 PurchaseItem 함수가 호출되도록 연결합니다.
            newSlot.Setup(itemData, ShopManager.Instance.PurchaseItem);
        }
    }
}