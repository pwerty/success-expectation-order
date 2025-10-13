// UIEvents.cs
using System;

public static class UIEvents
{
    // 💡 string 대신 UIPanel을 전달하도록 변경합니다.
    public static event Action<UIPanel> OnPanelShown;

    // 이벤트를 외부로 송출하는 함수도 그에 맞게 변경합니다.
    public static void PanelShown(UIPanel panel)
    {
        OnPanelShown?.Invoke(panel);
    }
}