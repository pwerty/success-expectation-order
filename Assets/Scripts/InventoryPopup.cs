using System;
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
    private Action<UserItem> currentOnSelectCallback;

    void Start()
    {
        // 배경 버튼을 누르면 팝업이 닫히도록 설정
        backgroundButton.onClick.AddListener(Hide);
        // 시작 시에는 숨겨둡니다.
        popupWindowObject.SetActive(false);
    }
    
   

    /// <summary>
    /// 지정된 카테고리의 아이템으로 팝업을 채우고, 아이템 선택 시 실행할 행동을 지정합니다.
    /// </summary>
    public void Show(ItemCategory category, Action<UserItem> onSelectCallback)
    {
        // 💡 전달받은 '배달 요청사항(콜백)'을 저장합니다.
        currentOnSelectCallback = onSelectCallback;

        // --- 이하 슬롯 생성 로직은 동일 ---
        foreach (Transform child in contentParent) { Destroy(child.gameObject); }

        List<UserItem> itemsToShow;
        if (category == ItemCategory.Driver)
            itemsToShow = InventoryManager.Instance.GetPlayerDrivers();
        else
            itemsToShow = InventoryManager.Instance.GetPlayerCars();

        foreach (UserItem item in itemsToShow)
        {
            ItemSlot newSlot = Instantiate(itemSlotPrefab, contentParent);
            // 💡 중요: 이제 하드코딩된 OnItemSelected가 아닌, '슬롯이 클릭됨'을 알리는 범용 함수를 연결합니다.
            newSlot.Setup(item, OnSlotClicked);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
        popupWindowObject.SetActive(true);
    }

    /// <summary>
    /// 어떤 슬롯이든 클릭되면 호출되는 중간 다리 역할의 함수입니다.
    /// </summary>
    private void OnSlotClicked(UserItem selectedItem)
    {
        // 💡 저장해 두었던 '배달 요청사항(콜백)'을 실행하고, 선택된 아이템을 전달합니다.
        currentOnSelectCallback?.Invoke(selectedItem);

        // 약속을 이행했으니 팝업을 닫습니다.
        Hide();
    }


    /// <summary>
    /// 팝업창을 숨깁니다.
    /// </summary>
    public void Hide()
    {
        popupWindowObject.SetActive(false);
    }
}