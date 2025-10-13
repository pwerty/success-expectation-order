using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GachaResultSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image rarityBackgroundImage; // 등급별 배경
    [SerializeField] private TextMeshProUGUI nameText;

    // 등급별 배경색을 미리 지정해둡니다.
    [SerializeField] private Color r_Color = Color.gray;
    [SerializeField] private Color sr_Color = Color.blue;
    [SerializeField] private Color ssr_Color = Color.yellow;

    public void Setup(UserItem item)
    {
        ItemData data = DataManager.Instance.GetItemData(item.itemDataId);
        iconImage.sprite = data.icon;
        nameText.text = data.name;

        // 아이템이 '차'일 경우, 등급에 따라 배경색을 바꿉니다.
        if (data is CarData carData)
        {
            switch (carData.rarity)
            {
                case CarRarity.R:
                    rarityBackgroundImage.color = r_Color;
                    break;
                case CarRarity.SR:
                    rarityBackgroundImage.color = sr_Color;
                    break;
                case CarRarity.SSR:
                    rarityBackgroundImage.color = ssr_Color;
                    break;
            }
        }

    }
}