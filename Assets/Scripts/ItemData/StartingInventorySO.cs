using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 게임 시작 시 초기 인벤토리를 정의하는 ScriptableObject입니다.
/// </summary>
[CreateAssetMenu(fileName = "StartingInventory", menuName = "Game Data/Starting Inventory Preset")]
public class StartingInventorySO : ScriptableObject
{
    public List<InitialItemEntry> startingItems;
}

/// <summary>
/// 초기 아이템의 정보를 담는 데이터 구조입니다.
/// </summary>
[System.Serializable]
public class InitialItemEntry
{
    public string itemDataId;
    [Min(1)] public int quantity = 1;
    [Min(1)] public int enhancementLevel = 1;
}