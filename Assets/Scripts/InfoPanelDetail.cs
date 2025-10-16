using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoPanelDetail : MonoBehaviour
{
    [SerializeField] private InventoryPopup inventoryPopup;

    public void OpenInventoryForCar()
    {
        inventoryPopup.Show(ItemCategory.Car, OnCarSelectedForPreview);
    }
    
    public void OnInventoryForDriver()
    {
        inventoryPopup.Show(ItemCategory.Driver, OnDriverSelectedForPreview);
    }

    // '배달 요청사항'에 해당하는 실제 함수
    private void OnCarSelectedForPreview(UserItem selected)
    {
        // 기존 OnItemSelected에 있던 로직을 이곳으로 옮겨옵니다.
        PreviewManager.Instance.DisplayItem(selected);
        UpdateDisplay();
    }
    
    private void OnDriverSelectedForPreview(UserItem selected)
    {
        // 기존 OnItemSelected에 있던 로직을 이곳으로 옮겨옵니다.
        PreviewManager.Instance.DisplayItem(selected);
        UpdateDisplay();
    }
    
    
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    
    
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI driftText;
    [SerializeField] private TextMeshProUGUI accelerationText;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Slider driftSlider;
    [SerializeField] private Slider accelSlider;

    [Header("애니메이션 설정")]
    [SerializeField] private float animationDuration = 0.5f; // 애니메이션에 걸리는 시간
    private Coroutine activeAnimationCoroutine;

    // 이 함수는 아이템 슬롯이 클릭될 때 호출되거나,
    // PreviewManager가 "미리보기 업데이트됨!" 이벤트를 방송할 때 호출될 수 있습니다.
    public void UpdateDisplay()
    {
        UserItem selectedItem = PreviewManager.Instance.CurrentSelectedItem;

        if (selectedItem != null)
        {
            // 1. 이름, 레벨 등 즉시 변경될 정보는 바로 업데이트합니다.
            nameText.text = selectedItem.Data.name;
            levelText.text = $"+{selectedItem.enhancementLevel}";

            // 2. 이전에 실행 중인 애니메이션이 있다면 즉시 중단합니다.
            if (activeAnimationCoroutine != null)
            {
                StopCoroutine(activeAnimationCoroutine);
            }

            // 3. 새로운 애니메이션 코루틴을 시작하고, 그 참조를 저장합니다.
            var stats = selectedItem.GetCurrentStats();
            activeAnimationCoroutine = StartCoroutine(AnimateSlidersCoroutine(stats.stat1, stats.stat2, stats.stat3));
        }
    }
    
    /// <summary>
    /// 지정된 목표값까지 슬라이더를 부드럽게 움직이는 코루틴입니다.
    /// </summary>
    private IEnumerator AnimateSlidersCoroutine(float targetSpeed, float targetDrift, float targetAccel)
    {
        // --- 애니메이션 시작 ---
        // TODO: 여기에 슬라이더 움직임 시작 사운드를 재생하는 코드를 넣습니다.
        // AudioManager.Instance.PlaySliderStartSound();

        // 시작 시점의 각 슬라이더 값을 저장합니다. (before value)
        float startSpeed = speedSlider.value;
        float startDrift = driftSlider.value;
        float startAccel = accelSlider.value;

        float elapsedTime = 0f;

        // animationDuration 시간 동안 반복하여 값을 서서히 변경합니다.
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / animationDuration);

            // Lerp (선형 보간)를 사용하여 현재 값과 목표 값 사이의 값을 계산합니다.
            speedSlider.value = Mathf.Lerp(startSpeed, targetSpeed, progress);
            driftSlider.value = Mathf.Lerp(startDrift, targetDrift, progress);
            accelSlider.value = Mathf.Lerp(startAccel, targetAccel, progress);
            
            // TODO: 값이 증가할 때마다 '틱' 소리를 내면 더 좋습니다.
            // AudioManager.Instance.PlaySliderTickSound();

            yield return null; // 다음 프레임까지 대기합니다.
        }

        // --- 애니메이션 종료 ---
        // 애니메이션이 끝난 후, 목표 값으로 정확하게 값을 설정합니다. (오차 보정)
        speedSlider.value = targetSpeed;
        driftSlider.value = targetDrift;
        accelSlider.value = targetAccel;

        // 텍스트는 애니메이션이 모두 끝난 뒤 마지막에 한 번만 업데이트합니다.
        speedText.text = targetSpeed.ToString("F0");
        driftText.text = targetDrift.ToString("F0");
        accelerationText.text = targetAccel.ToString("F0");
    }

    // UI가 처음 활성화될 때 한 번 업데이트 해주는 것이 좋습니다.
    void OnEnable()
    {
        print("활성화로 인한 갱신 실시!");
        
        // 💡 1. 자신의 부모 계층에서 UIPanel 컴포넌트를 찾습니다.
        UIPanel parentPanel = GetComponentInParent<UIPanel>();

        if (parentPanel == null)
        {
            Debug.LogWarning("InfoPanelDetail의 부모 UIPanel을 찾을 수 없습니다!");
            return;
        }

        // 💡 2. 부모의 panelId에 따라 다른 행동을 하도록 분기합니다.
        switch (parentPanel.panelId)
        {
            case "CarPanel": // 만약 부모 패널의 ID가 "Garage"라면
                var playerCars = InventoryManager.Instance.GetPlayerCars();
                if (playerCars != null && playerCars.Count > 0)
                {
                    OnCarSelectedForPreview(playerCars[0]);
                }
                break;
            
            case "DriverPanel": // 만약 부모 패널의 ID가 "DriverInfo"라면
                var playerDrivers = InventoryManager.Instance.GetPlayerDrivers();
                if (playerDrivers != null && playerDrivers.Count > 0)
                {
                    OnDriverSelectedForPreview(playerDrivers[0]);
                }
                break;
            
            default:
                Debug.Log($"InfoPanelDetail이 예상치 못한 부모({parentPanel.panelId}) 하위에 있습니다.");
                break;
        }

        // 애니메이션 트리거는 분기 로직 이후에 공통으로 호출
        this.GetComponent<Animator>()?.SetTrigger("Show");
    }
}