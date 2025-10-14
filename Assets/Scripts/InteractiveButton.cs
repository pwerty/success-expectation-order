using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

// TabGroup과 호환되도록 기능이 확장되었습니다.
public class InteractiveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("연결할 오브젝트")]
    [SerializeField]
    private Image mainButtonImage; // 버튼의 기본 배경 이미지
    [SerializeField]
    private TextMeshProUGUI buttonText; // 버튼 텍스트
    [SerializeField]
    private Image spreadImage; // 중앙에서 퍼지는 효과를 위한 이미지

    [Header("효과 설정")]
    [SerializeField]
    private Color normalColor = Color.white; // 비선택 상태의 버튼 배경색
    [SerializeField]
    private Color selectedColor = Color.grey; // 선택 상태의 버튼 배경색 (비활성화 암시)
    [SerializeField]
    private Color spreadEffectColor = Color.cyan; // 퍼지는 효과의 색상
    [SerializeField]
    private float animationDuration = 0.3f; // 애니메이션 지속 시간
    [SerializeField]
    private float textScaleMultiplier = 1.1f; // 호버 시 텍스트 확대 배율

    private Vector3 initialTextScale;
    private Coroutine currentSpreadAnimation;
    private Coroutine currentTextAnimation;
    private Button buttonComponent;

    private bool isSelected = false;

    void Awake()
    {
        // 컴포넌트들을 미리 찾아둡니다.
        if (mainButtonImage == null)
        {
            mainButtonImage = GetComponent<Image>();
        }
        buttonComponent = GetComponent<Button>();

        if (buttonText != null)
        {
            initialTextScale = buttonText.transform.localScale;
        }

        // 초기 상태 설정
        if (spreadImage != null)
        {
            spreadImage.rectTransform.localScale = new Vector3(0, 1, 1);
            spreadImage.color = new Color(spreadEffectColor.r, spreadEffectColor.g, spreadEffectColor.b, 0);
        }
        mainButtonImage.color = normalColor;
    }

    // 마우스를 올렸을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return;

        if (buttonText != null)
        {
            if (currentTextAnimation != null) StopCoroutine(currentTextAnimation);
            currentTextAnimation = StartCoroutine(AnimateTextScale(textScaleMultiplier));
        }

        if (spreadImage != null)
        {
            if (currentSpreadAnimation != null) StopCoroutine(currentSpreadAnimation);
            currentSpreadAnimation = StartCoroutine(AnimateSpread(true));
        }
    }

    // 마우스가 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return;

        if (buttonText != null)
        {
            if (currentTextAnimation != null) StopCoroutine(currentTextAnimation);
            currentTextAnimation = StartCoroutine(AnimateTextScale(1f));
        }
        
        if (spreadImage != null)
        {
            if (currentSpreadAnimation != null) StopCoroutine(currentSpreadAnimation);
            spreadImage.rectTransform.localScale = new Vector3(0, 1, 1);
            spreadImage.color = new Color(spreadEffectColor.r, spreadEffectColor.g, spreadEffectColor.b, 0);
        }
    }

    // '선택' 상태가 되었을 때 TabGroup에 의해 호출됩니다.
    public void Select()
    {
        isSelected = true;
        buttonComponent.interactable = false; // 버튼 상호작용 비활성화
        mainButtonImage.color = selectedColor; // 배경색을 '선택됨' 색상으로 변경

        // 호버 효과가 남아있었다면 중지
        if (currentTextAnimation != null) StopCoroutine(currentTextAnimation);
        if (currentSpreadAnimation != null) StopCoroutine(currentSpreadAnimation);

        // 텍스트 크기 고정
        if (buttonText != null)
        {
            buttonText.transform.localScale = initialTextScale * textScaleMultiplier;
        }
        
        // 퍼지는 효과 (사라지지 않음)
        if (spreadImage != null)
        {
            currentSpreadAnimation = StartCoroutine(AnimateSpread(false));
        }
    }

    // '비선택' 상태가 되었을 때 TabGroup에 의해 호출됩니다.
    public void Deselect()
    {
        isSelected = false;
        buttonComponent.interactable = true; // 버튼 상호작용 활성화
        mainButtonImage.color = normalColor; // 배경색을 기본 색상으로 복원

        // 모든 애니메이션 효과 즉시 초기화
        if (currentTextAnimation != null) StopCoroutine(currentTextAnimation);
        if (buttonText != null) buttonText.transform.localScale = initialTextScale;

        if (currentSpreadAnimation != null) StopCoroutine(currentSpreadAnimation);
        if (spreadImage != null)
        {
            spreadImage.rectTransform.localScale = new Vector3(0, 1, 1);
            spreadImage.color = new Color(spreadEffectColor.r, spreadEffectColor.g, spreadEffectColor.b, 0);
        }
    }

    private IEnumerator AnimateTextScale(float targetMultiplier)
    {
        float timer = 0f;
        Vector3 startScale = buttonText.transform.localScale;
        Vector3 targetScale = initialTextScale * targetMultiplier;
        
        while (timer < animationDuration)
        {
            buttonText.transform.localScale = Vector3.Lerp(startScale, targetScale, timer / animationDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        buttonText.transform.localScale = targetScale;
    }
    
    private IEnumerator AnimateSpread(bool fadeOut)
    {
        float timer = 0f;
        Color c = spreadEffectColor;
        spreadImage.color = c;

        while (timer < animationDuration)
        {
            float progress = timer / animationDuration;
            float scaleX = Mathf.Lerp(0, 1, 1 - Mathf.Pow(1 - progress, 3));
            spreadImage.rectTransform.localScale = new Vector3(scaleX, 1, 1);

            if (fadeOut)
            {
                float alpha = Mathf.Lerp(1, 0, progress);
                spreadImage.color = new Color(c.r, c.g, c.b, alpha);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        if (fadeOut)
        {
            spreadImage.rectTransform.localScale = new Vector3(0, 1, 1);
        }
        else
        {
            spreadImage.rectTransform.localScale = new Vector3(1, 1, 1);
            spreadImage.color = c;
        }
    }
}
