using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // Action을 사용하기 위해 필요

public class ShopItemSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button purchaseButton;

    private ItemData currentItemData;

    /// <summary>
    /// 이 슬롯이 어떤 아이템을 표시할지 설정하고, 구매 버튼의 이벤트를 연결합니다.
    /// </summary>
    public void Setup(ItemData data, Action<string> onPurchaseCallback)
    {
        currentItemData = data;

        // 1. 데이터 원본에서 정보를 가져와 UI에 표시합니다.
        iconImage.sprite = data.icon;
        nameText.text = data.name;

        // 2. 구매 버튼이 눌렸을 때, 어떤 행동을 할지 외부에서 주입받습니다.
        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(() => onPurchaseCallback?.Invoke(currentItemData.id));
    }
}