using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 가챠(뽑기) 시스템의 로직을 담당합니다.
/// 현재는 테스트를 위한 임시 확률 로직을 사용합니다.
/// </summary>
public class GachaManager : MonoBehaviour
{
    public static GachaManager Instance { get; private set; }

    [Header("테스트용 아이템 목록")]
    [Tooltip("뽑기에 나올 R 등급 차량 ID 목록입니다.")]
    [SerializeField] private List<string> r_CarPool;
    [Tooltip("뽑기에 나올 SR 등급 차량 ID 목록입니다.")]
    [SerializeField] private List<string> sr_CarPool;
    [Tooltip("뽑기에 나올 SSR 등급 차량 ID 목록입니다.")]
    [SerializeField] private List<string> ssr_CarPool;
    
    // 이 확률은 나중에 ScriptableObject로 옮겨갈 것입니다.
    private const int SSR_CHANCE = 5;  // 5%
    private const int SR_CHANCE = 10; // 10%

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지되도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// UI 버튼의 OnClick() 이벤트에 연결될 공개 함수입니다.
    /// 이 함수는 아무것도 반환하지 않습니다(void).
    /// </summary>
    public void OnPressPullButton()
    {
        // 1. 내부적으로 진짜 가챠 함수를 호출합니다.
        List<UserItem> pulledItems = PerformTenCarPull();

        // 2. 받아온 답장(결과 아이템 리스트)을 처리합니다.
        //    (예: 결과창 UI를 띄우는 코드를 여기에 넣습니다.)
        if (pulledItems != null)
        {
            // 2. UIManager에게 '결과창'을 열어달라고 요청합니다. (뒤로가기 기록이 쌓입니다)
            UIManager.Instance.OpenPanel("GachaResult");
            
            // 3. 방금 열린 결과창에 뽑기 결과 데이터를 전달합니다.
            GachaResultPanel.Instance.ShowResults(pulledItems);

            for (int i = 0; i < pulledItems.Count; i++)
            {
                InventoryManager.Instance.AddItem(pulledItems[i].itemDataId);
                print("ITEM 추가 : " +  pulledItems[i].itemDataId);
            }
            
            print("PULL COMPLETE");
        }
    }
    

    /// <summary>
    /// 차량 10회 뽑기를 수행하고, 결과를 인벤토리에 추가한 뒤, 뽑힌 아이템 목록을 반환합니다.
    /// </summary>
    public List<UserItem> PerformTenCarPull()
    {
        // 1. 재화 소모 로직 (지금은 주석 처리)
        // if (!CurrencyManager.Instance.TrySpendGems(3000)) { return null; }

        List<UserItem> resultItems = new List<UserItem>();

        for (int i = 0; i < 10; i++)
        {
            // 2. 확률에 따라 등급 결정
            int roll = Random.Range(0, 100);
            string pulledItemId;

            if (roll < SSR_CHANCE) // SSR (0~4)
            {
                pulledItemId = ssr_CarPool[Random.Range(0, ssr_CarPool.Count)];
            }
            else if (roll < SSR_CHANCE + SR_CHANCE) // SR (5~14)
            {
                pulledItemId = sr_CarPool[Random.Range(0, sr_CarPool.Count)];
            }
            else // R (15~99)
            {
                pulledItemId = r_CarPool[Random.Range(0, r_CarPool.Count)];
            }

            // 3. 결정된 아이템을 인벤토리에 추가하고, 결과 목록에도 추가합니다.
            UserItem newItem = InventoryManager.Instance.GetItem(pulledItemId);
            resultItems.Add(newItem);
        }

        Debug.Log("10회 뽑기 완료! 결과: " + resultItems.Count + "개의 아이템 획득.");
        // 4. 결과창 UI에 보여줄 수 있도록, 뽑힌 아이템 목록을 반환합니다.
        return resultItems;
    }
}