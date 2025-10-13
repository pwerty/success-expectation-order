using UnityEngine;
using System.Collections.Generic;
using System.Linq; // FirstOrDefault()를 사용하기 위해 필요

/// <summary>
/// 게임의 모든 정적 데이터(ScriptableObject)를 로드하고 관리하는 싱글턴입니다.
/// </summary>
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    // ID(string)를 키로 사용하여 모든 아이템 데이터에 빠르게 접근할 수 있는 '데이터베이스'입니다.
    private Dictionary<string, ItemData> itemDatabase = new Dictionary<string, ItemData>();

    void Awake()
    {
        // --- 싱글턴 패턴 구현 ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllItemData(); // 모든 데이터를 로드합니다.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Resources 폴더에서 모든 ItemData ScriptableObject를 로드하여 데이터베이스를 구축합니다.
    /// </summary>
    private void LoadAllItemData()
    {
        // Resources/GameData/ 폴더 하위의 모든 ItemData를 불러옵니다.
        // DriverData, CarData 등 ItemData를 상속하는 모든 것을 다 가져옵니다.
        var allItems = Resources.LoadAll<ItemData>("GameData");

        foreach (var item in allItems)
        {
            if (!itemDatabase.ContainsKey(item.id))
            {
                itemDatabase.Add(item.id, item);
            }
            else
            {
                Debug.LogWarning($"중복된 아이템 ID가 존재합니다: {item.id}");
            }
        }

        Debug.Log($"{itemDatabase.Count}개의 아이템 데이터를 로드했습니다.");
    }

    /// <summary>
    /// ID를 이용해 해당하는 아이템의 원본 데이터(설계도)를 반환합니다.
    /// </summary>
    public ItemData GetItemData(string id)
    {
        if (itemDatabase.TryGetValue(id, out ItemData data))
        {
            return data;
        }
        
        Debug.LogError($"아이템 ID를 찾을 수 없습니다: {id}");
        return null;
    }
}