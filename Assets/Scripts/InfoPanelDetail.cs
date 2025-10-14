using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GarageDetailPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI driftText;
    [SerializeField] private TextMeshProUGUI accelerationText;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Slider driftSlider;
    [SerializeField] private Slider accelSlider;

    // 이 함수는 아이템 슬롯이 클릭될 때 호출되거나,
    // PreviewManager가 "미리보기 업데이트됨!" 이벤트를 방송할 때 호출될 수 있습니다.
    public void UpdateDisplay()
    {
        UserItem selectedItem = PreviewManager.Instance.CurrentSelectedItem;

        if (selectedItem != null)
        {
            // 1. 이름, 레벨 등 공통 정보를 표시합니다.
            
            nameText.text = selectedItem.Data.name;
            levelText.text = $"+{selectedItem.enhancementLevel}";

            // 2. 현재 레벨에 맞는 최종 능력치를 계산하여 표시합니다.
            var stats = selectedItem.GetCurrentStats();
            speedText.text = stats.stat1.ToString("F0"); // F0: 소수점 없이
            driftText.text = stats.stat2.ToString("F0");
            accelerationText.text = stats.stat3.ToString("F0");
            speedSlider.value = stats.stat1;
            driftSlider.value = stats.stat2;
            accelSlider.value = stats.stat3;
        }
    }
    
    // UI가 처음 활성화될 때 한 번 업데이트 해주는 것이 좋습니다.
    void OnEnable()
    {
        UpdateDisplay();
    }
}