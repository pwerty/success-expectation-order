using System;
using UnityEngine;

[Serializable]
public class UserItem
{
    public string itemDataId; // 어떤 아이템의 원본인지 가리키는 ID
    public int enhancementLevel = 1; // 강화 단계
    public int quantity = 1;

    // --- 편의를 위한 기능들 ---

    // itemDataId를 이용해 실제 데이터 원본(ScriptableObject)을 찾아올 수 있어야 합니다.
    // 이 부분은 DataManager가 담당합니다.
    private ItemData _data;
    public ItemData Data
    {
        get
        {
            if (_data == null)
            {
                 _data = DataManager.Instance.GetItemData(itemDataId);
            }
            return _data;
        }
    }
    
    // 강화 단계에 따른 최종 능력치를 계산하는 함수
    public (float stat1, float stat2, float stat3) GetCurrentStats()
    {
        // 강화 레벨이 1 미만일 경우를 대비한 방어 코드
        int levelFactor = Mathf.Max(0, enhancementLevel - 1);

        if (Data is DriverData driverData)
        {
            float pedalSense = driverData.basePedalSense + (driverData.growthPedalSense * levelFactor);
            float recovery = driverData.baseRecoverySpeed + (driverData.growthRecoverySpeed * levelFactor);
            float solving = driverData.baseProblemSolving + (driverData.growthProblemSolving * levelFactor);
            return (pedalSense, recovery, solving);
        }
        
        if (Data is CarData carData)
        {
            float speed = carData.baseSpeed + (carData.growthSpeed * levelFactor);
            float drift = carData.baseDrift + (carData.growthDrift * levelFactor);
            float accel = carData.baseAcceleration + (carData.growthAcceleration * levelFactor);
            return (speed, drift, accel);
        }

        return (0, 0, 0);
    }
}