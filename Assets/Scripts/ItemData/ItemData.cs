using UnityEngine;
using UnityEngine.UI;

public abstract class ItemData : ScriptableObject
{
    public string id;
    public new string name; // new 키워드로 GameObject의 name을 숨기기
    [TextArea] public string description;
    
    [Header("시각적 에셋")]
    public Sprite icon; // UI에 표시될 2D 아이콘
    public GameObject modelPrefab; // 게임 월드에 표시될 3D 모델 프리팹
}