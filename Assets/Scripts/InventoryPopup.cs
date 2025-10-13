using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum ItemCategory { Driver, Car }

public class InventoryPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupWindowObject; // 팝업창 본체
    [SerializeField] private Button backgroundButton; // 외부 클릭 감지용 배경
    [SerializeField] private Transform contentParent; // 아이템 슬롯이 생성될 부모 (Scroll View의 Content)
    [SerializeField] private ItemSlot itemSlotPrefab; // 아이템 슬롯 프리팹

    void Start()
    {
        // 배경 버튼을 누르면 팝업이 닫히도록 설정
        backgroundButton.onClick.AddListener(Hide);
        // 시작 시에는 숨겨둡니다.
        popupWindowObject.SetActive(false);
    }
    
    /// <summary>
    /// '드라이버' 버튼이 OnClick()에서 호출할 전용 함수입니다.
    /// </summary>
    public void ShowDriverInventory()
    {
        Show(ItemCategory.Driver);
    }

    /// <summary>
    /// '차량' 버튼이 OnClick()에서 호출할 전용 함수입니다.
    /// </summary>
    public void ShowCarInventory()
    {
        Show(ItemCategory.Car);
    }


    /// <summary>
    /// 지정된 카테고리의 아이템들로 팝업을 채우고 보여줍니다.
    /// </summary>
    public void Show(ItemCategory category)
    {
        // 1. 이전에 있던 슬롯들을 모두 삭제합니다.
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 2. 요청된 카테고리에 맞는 아이템 목록을 InventoryManager로부터 받아옵니다.
        List<UserItem> itemsToShow;
        if (category == ItemCategory.Driver)
        {
            itemsToShow = InventoryManager.Instance.GetPlayerDrivers();
        }
        else // category == ItemCategory.Car
        {
            itemsToShow = InventoryManager.Instance.GetPlayerCars();
        }

        // 3. 받아온 목록을 바탕으로 아이템 슬롯을 생성하고 설정합니다.
        foreach (UserItem item in itemsToShow)
        {
            ItemSlot newSlot = Instantiate(itemSlotPrefab, contentParent);
            // 슬롯을 클릭하면 OnItemSelected 함수가 호출되도록 연결
            newSlot.Setup(item, OnItemSelected);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
        // 4. 팝업창을 보여줍니다.
        popupWindowObject.SetActive(true);
    }

    /// <summary>
    /// 아이템 슬롯이 클릭되었을 때 호출될 함수입니다.
    /// </summary>
    private void OnItemSelected(UserItem selectedItem)
    {
        Debug.Log($"{selectedItem.Data.name} (+{selectedItem.enhancementLevel}) 이(가) 선택되었습니다.");
        
        // 아이템을 선택했으니 팝업을 닫습니다.
        Hide();
        
        // TODO: 여기서 선택된 아이템 정보를 다른 시스템(예: 장착 시스템)에 전달하는 로직을 추가할 수 있습니다.
    }

    /// <summary>
    /// 팝업창을 숨깁니다.
    /// </summary>
    public void Hide()
    {
        popupWindowObject.SetActive(false);
    }
}