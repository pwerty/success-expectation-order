using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // Action을 사용하기 위해 필요

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button button;

    private UserItem currentItem;

    /// <summary>
    /// 이 슬롯이 어떤 아이템을 표시할지 설정하고, 클릭 이벤트를 연결합니다.
    /// </summary>
    public void Setup(UserItem itemToDisplay, Action<UserItem> onSlotClickedCallback)
    {
        currentItem = itemToDisplay;

        // 1. 데이터 원본(ScriptableObject)에서 시각적 정보를 가져옵니다.
        ItemData data = DataManager.Instance.GetItemData(currentItem.itemDataId); 
        iconImage.sprite = data.icon;
        nameText.text = data.name;

        // 2. 인스턴스 정보(UserItem)에서 동적 데이터를 가져옵니다.
        levelText.text = $"+{currentItem.enhancementLevel}";

        // 3. 클릭되었을 때 어떤 행동을 할지 외부에서 주입받습니다.
        button.onClick.RemoveAllListeners(); // 이전 리스너 제거
        button.onClick.AddListener(() => onSlotClickedCallback?.Invoke(currentItem));
    }
}