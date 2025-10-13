using UnityEngine;
using System.Collections;

/// <summary>
/// UI 자식 요소에 붙여서 개별 등장 애니메이션을 제어하는 스크립트입니다.
/// </summary>
[RequireComponent(typeof(Animator))]
public class UIComponentAnimator : MonoBehaviour
{
    [Tooltip("부모 패널이 나타날 때, 이 컴포넌트가 얼마나 늦게 등장할지 결정합니다 (초 단위).")]
    public float delay = 0f;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        // 시작 시에는 보이지 않도록 비활성화합니다.
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 부모 UIPanel이 호출하여 등장 애니메이션을 재생시킵니다.
    /// </summary>
    public void PlayShowAnimation()
    {
        // 애니메이션을 재생하기 직전에 활성화합니다.
        gameObject.SetActive(true);
        if(animator != null)
            animator.SetTrigger("Show");
            
    }
}