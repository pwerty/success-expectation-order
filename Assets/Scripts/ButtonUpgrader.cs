using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class ButtonUpgrader
{
    // Unity 에디터 상단 메뉴에 새로운 항목을 추가합니다.
    [MenuItem("Tools/UI/Upgrade to Interactive Button")]
    private static void UpgradeSelectedButtons()
    {
        // 현재 Hierarchy에서 선택된 모든 게임 오브젝트를 가져옵니다.
        GameObject[] selectedObjects = Selection.gameObjects;
        int upgradedCount = 0;

        foreach (GameObject go in selectedObjects)
        {
            // InteractiveButton_V2 스크립트가 이미 있는지 확인합니다. 이미 있다면 건너뜁니다.
            if (go.GetComponent<InteractiveButton>() != null)
            {
                Debug.LogWarning($"'{go.name}' 에는 이미 InteractiveButton_V2 컴포넌트가 있어 건너뜁니다.", go);
                continue;
            }

            // Button 컴포넌트와 자식으로 Text 또는 TextMeshProUGUI가 있는지 확인합니다.
            Button button = go.GetComponent<Button>();
            TMP_Text tmpText = go.GetComponentInChildren<TMP_Text>();
            Text unityText = go.GetComponentInChildren<Text>();

            if (button != null && (tmpText != null || unityText != null))
            {
                // 변경 사항을 기록하여 Undo(Ctrl+Z)가 가능하도록 합니다.
                Undo.RecordObject(go, "Upgrade Button");

                // 1. InteractiveButton_V2 스크립트를 추가합니다.
                InteractiveButton interactiveButton = Undo.AddComponent<InteractiveButton>(go);

                // 2. 자식 텍스트 오브젝트의 이름을 'ButtonText'로 변경합니다.
                Transform textTransform = (tmpText != null) ? tmpText.transform : unityText.transform;
                Undo.RecordObject(textTransform.gameObject, "Rename Text");
                textTransform.name = "ButtonText";

                // 3. 'SpreadEffect' 이름의 새 Image 자식 오브젝트를 생성합니다.
                GameObject spreadEffectObj = new GameObject("SpreadEffect", typeof(Image));
                Undo.RegisterCreatedObjectUndo(spreadEffectObj, "Create SpreadEffect");
                spreadEffectObj.transform.SetParent(go.transform, false);

                // 4. SpreadEffect 이미지의 RectTransform을 부모에 꽉 차도록 설정합니다.
                RectTransform spreadRect = spreadEffectObj.GetComponent<RectTransform>();
                spreadRect.anchorMin = Vector2.zero;
                spreadRect.anchorMax = Vector2.one;
                spreadRect.sizeDelta = Vector2.zero;
                spreadRect.anchoredPosition = Vector2.zero;
                
                // SpreadEffect가 텍스트 뒤에 렌더링되도록 순서를 맨 앞으로 보냅니다.
                spreadEffectObj.transform.SetAsFirstSibling();

                // 5. 새로 추가한 InteractiveButton_V2 스크립트에 참조를 자동으로 연결합니다.
                // SerializedObject를 사용하면 private 필드에도 접근할 수 있습니다.
                SerializedObject so = new SerializedObject(interactiveButton);
                so.FindProperty("mainButtonImage").objectReferenceValue = go.GetComponent<Image>();
                so.FindProperty("buttonText").objectReferenceValue = textTransform.GetComponent<TextMeshProUGUI>();
                so.FindProperty("spreadImage").objectReferenceValue = spreadEffectObj.GetComponent<Image>();
                so.ApplyModifiedProperties(); // 변경사항 적용

                upgradedCount++;
                Debug.Log($"'{go.name}' 버튼을 성공적으로 업그레이드했습니다.", go);
            }
        }

        if (upgradedCount > 0)
        {
            Debug.Log($"{upgradedCount}개의 버튼이 InteractiveButton으로 업그레이드되었습니다.");
        }
        else
        {
            Debug.LogWarning("업그레이드할 유효한 버튼이 선택되지 않았습니다. Button과 자식 Text가 있는지 확인하세요.");
        }
    }
}
