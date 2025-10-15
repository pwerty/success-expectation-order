using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private ItemSlot upgradeTargetSlot;
    [SerializeField] private ItemSlot upgradeIngredientSlot;
    [SerializeField] private ItemSlot upgradeResult;
    private int upgradeSuccessPercent;
    private UserItem upgradeTarget;
    private UserItem upgradeIngredient;
    [SerializeField] private InventoryPopup inventoryPopup;

    [SerializeField] private TMP_Text expectationPercentTxt;
    public void OpenMaterialSelection()
    {
        // "배달 기사님, 차 목록을 보여주시고, 차가 선택되면 OnMaterialSelected 함수를 실행해주세요."
        inventoryPopup.Show(ItemCategory.Car, OnMaterialSelected);
        
   
    }

    private void OnMaterialSelected(UserItem selectedMaterial)
    {
        upgradeIngredient = selectedMaterial;
        upgradeIngredientSlot.Setup(upgradeIngredient, null);
        upgradeSuccessPercent = UpgradeSuccessCalculate();
        expectationPercentTxt.text = upgradeSuccessPercent.ToString();
    }

    void OnEnable()
    {
        inventoryPopup.gameObject.SetActive(true);
        // 선택 차량으로 정정
        upgradeTarget = InventoryManager.Instance.GetPlayerCars()[0];
        OpenMaterialSelection();
        
        upgradeTargetSlot.Setup(upgradeTarget, null);
        
    }

    int UpgradeSuccessCalculate()
    {
        if (upgradeTarget.enhancementLevel <= upgradeIngredient.enhancementLevel)
            return 100;
        return 0;
    }

    public void TryUpgrade()
    {
        print("UPGRADE START!");
        GetComponent<Animator>().SetTrigger("TryUpgrade");
        string itemidToAddItem = upgradeTarget.itemDataId;
        int itemLv = upgradeTarget.enhancementLevel;
        InventoryManager.Instance.RemoveItemByInstanceId(upgradeIngredient.instanceId);
        InventoryManager.Instance.RemoveItemByInstanceId(upgradeTarget.instanceId);

        if (upgradeSuccessPercent != 0)
        { 
            print(" 업그레이드에 성공했습니다 ::");
            InventoryManager.Instance.AddItem(itemidToAddItem, 1, itemLv + 1);
        }
        else
        {
            print(" 업그레이드에 실패했습니다 ::");
            InventoryManager.Instance.AddItem(itemidToAddItem, 1, 1);
        }
        
        upgradeResult.Setup(InventoryManager.Instance.GetPlayerCars()[0], null);
        
    }
}
