using UnityEngine;
using System.Collections.Generic;
using System; // Action을 사용하기 위해 필요
using System.Linq;

/// <summary>
/// UI 내비게이션, 패널 관리, 뒤로가기 기능을 총괄하는 중앙 관리자입니다. (싱글턴)
/// </summary>
public class UIManager : MonoBehaviour
{
    // CAUTION : Unity에서 다루는 Panel과 1:1 대응하는 것이 아닙니다!
    public static UIManager Instance { get; private set; }

    [Tooltip("애플리케이션 시작 시 기본으로 표시될 UIPanel의 ID입니다.")]
    [SerializeField] private string homePanelId;

    // '주소록' 역할을 하는 Dictionary: <패널 ID, 해당 UIPanel 컴포넌트>
     private Dictionary<string, UIPanel> panelRegistry = new Dictionary<string, UIPanel>();

    // 뒤로가기 기능을 위한 '행동'을 저장하는 스택
    private Stack<Action> historyStack = new Stack<Action>();

    private UIPanel currentActivePanel;

    void Awake()
    {
        homePanelId = "Home";
        // --- 싱글턴 패턴 구현 ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지되도록 설정
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // --- 여기가 핵심: '주소록' 자동 생성 ---
        // 씬에 존재하는 모든 UIPanel 컴포넌트를 찾아(비활성화된 것도 포함) 주소록에 자동 등록합니다.
        UIPanel[] allPanelsInScene = FindObjectsByType<UIPanel>(FindObjectsSortMode.None);

        // 2. LINQ의 Where() 함수를 사용해 isManagedByUIManager가 true인 패널만 걸러냅니다.
        var managedPanels = allPanelsInScene.Where(panel => panel.isControlledByManager);
        
        // 3. 오직 걸러낸 패널들('최상위 패널')만 가지고 주소록(Dictionary)을 만듭니다.
        foreach (UIPanel panel in managedPanels)
        {
            if (!panelRegistry.ContainsKey(panel.panelId))
            {
                panelRegistry.Add(panel.panelId, panel);
            }
            else
            {
                Debug.LogWarning($"중복된 Panel ID가 존재합니다: {panel.panelId}. GameObject: {panel.gameObject.name}");
            }
        }
        
        print("등록 완료");
        
        // 모든 패널을 숨기고 홈 패널만 표시하며 시작합니다.
        foreach (var panel in panelRegistry.Values)
        {
            panel.Hide();
            print(panel.name + "숨었다");
        }
        ShowPanelInternal(homePanelId);
        print("홈으로 화면 초기화 완료!");
    }
    
    /// <summary>
    /// ID를 이용해 특정 패널을 엽니다. 같은 계층의 패널을 교체할 때 사용됩니다.
    /// </summary>
    /// <param name="panelId">열고 싶은 UIPanel의 ID</param>
    public void SwitchPanel(string panelId)
    {
        if (currentActivePanel != null)
        {
            // 뒤로가기를 위해 "이전 패널을 켜는 행동"을 저장하지 않고 현재 패널만 닫습니다.
            currentActivePanel.Hide();
        }
        ShowPanelInternal(panelId);
    }
    
    /// <summary>
    /// ID를 이용해 새로운 계층의 패널을 엽니다. 뒤로가기 스택에 현재 상태를 저장합니다.
    /// </summary>
    /// <param name="panelId">열고 싶은 UIPanel의 ID</param>
    public void OpenPanel(string panelId)
    {
        if (currentActivePanel != null)
        {
            // 뒤로가기 스택에 "현재 활성화된 패널을 다시 켜는 행동"을 저장합니다.
            string previousPanelId = currentActivePanel.panelId;
            historyStack.Push(() => ShowPanelInternal(previousPanelId));
            currentActivePanel.Hide();
        }
        ShowPanelInternal(panelId);
    }

    /// <summary>
    /// 뒤로가기 버튼을 눌렀을 때 호출됩니다.
    /// </summary>
    public void GoBack()
    {
        if (historyStack.Count > 0)
        {
            if (currentActivePanel != null)
            {
                currentActivePanel.Hide();
            }
            // 스택에서 "이전 상태로 돌아가는 행동"을 꺼내서 실행합니다.
            historyStack.Pop().Invoke();
        }
        else
        {
            Debug.Log("더 이상 돌아갈 곳이 없습니다.");
            // 여기서 애플리케이션 종료 팝업을 띄울 수도 있습니다.
        }
    }

    /// <summary>
    /// 패널을 여는 내부 로직입니다.
    /// </summary>
    private void ShowPanelInternal(string panelId)
    {
        if (panelRegistry.TryGetValue(panelId, out UIPanel targetPanel))
        {
            targetPanel.Show();
            currentActivePanel = targetPanel;
            Debug.Log("Switch Complete");
            
            UIEvents.PanelShown(targetPanel);
        }
        else
        {
            Debug.LogError($"요청한 Panel ID를 찾을 수 없습니다: {panelId}");
        }
    }
}