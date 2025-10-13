using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GachaResultPanel : MonoBehaviour
{
    // 결과창은 보통 단 하나이므로, 쉽게 접근할 수 있도록 싱글턴으로 만듭니다.
    public static GachaResultPanel Instance { get; private set; }

    [SerializeField] private Transform contentParent;
    [SerializeField] private GachaResultSlot resultSlotPrefab;
    [SerializeField] private Button confirmButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 확인 버튼을 누르면 UIManager의 뒤로가기 기능이 호출되도록 연결
      //  confirmButton.onClick.AddListener(() => UIManager.Instance.GoBack());
    }

    /// <summary>
    /// 가챠 결과를 받아와 화면에 슬롯들을 생성하고 표시합니다.
    /// </summary>
    public void ShowResults(List<UserItem> resultItems)
    {
        // 이전 슬롯 삭제
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 새 슬롯 생성
        foreach (UserItem item in resultItems)
        {
            GachaResultSlot newSlot = Instantiate(resultSlotPrefab, contentParent);
            newSlot.Setup(item);
        }
    }
}