using UnityEngine;
using System.Collections;

/// <summary>
/// UI 이벤트에 반응하여 메인 카메라를 제어하는 독립적인 관리자입니다.
/// </summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Tooltip("제어할 메인 카메라의 Transform입니다.")]
    [SerializeField] private Transform mainCameraTransform;
    [Tooltip("카메라가 이동하는 데 걸리는 시간(초)입니다.")]
    [SerializeField] private float moveDuration = 1.0f;
    [Tooltip("카메라가 회전하는 데 걸리는 시간(초)입니다.")]
    [SerializeField] private float rotateDuration = 1.0f;
    
    // 💡 아래 두 줄을 추가합니다.
    [Header("가속도 설정")]
    [Tooltip("카메라 이동에 적용될 가속도 커브입니다. S자 모양을 추천합니다.")]
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Tooltip("카메라 회전에 적용될 가속도 커브입니다.")]
   // [SerializeField] private AnimationCurve rotateCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    // 이전에 카메라가 있던 위치와 회전값을 저장해 둡니다. (예: 원래 위치로 돌아오기 기능용)
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private Coroutine activeMoveCoroutine;

    void Awake()
    {
        rotateDuration = moveDuration;
        print("Camera Manager");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (mainCameraTransform != null)
        {
            defaultPosition = mainCameraTransform.position;
            defaultRotation = mainCameraTransform.rotation;
        }
    }
    
    void OnEnable()
    {
        // "라디오를 켜고, 패널이 열렸다는 방송이 나오는지 귀를 기울입니다."
        UIEvents.OnPanelShown += HandlePanelChange;
    }

    void OnDisable()
    {
        // "라디오를 끕니다." (메모리 누수 방지)
        UIEvents.OnPanelShown -= HandlePanelChange;
    }

    /// <summary>
    /// UIEvents 방송을 수신했을 때 실행될 함수입니다.
    /// </summary>
    private void HandlePanelChange(UIPanel panel)
    {
        if (panel.isLogicalPanel) return;
        print(panel.panelId + "의 활성화로 카메라 이동 작동 시작");
        // 만약 이전에 실행 중이던 카메라 이동이 있었다면, 즉시 중단합니다.
        if (activeMoveCoroutine != null)
        {
            StopCoroutine(activeMoveCoroutine);
        }

        // 방송으로 전달된 패널이 "카메라를 움직여라"라고 설정되어 있는지 확인합니다.
        if (panel.moveCameraOnShow)
        {
            // 설정되어 있다면, 해당 위치로 카메라를 움직이는 코루틴을 시작합니다.
            activeMoveCoroutine = StartCoroutine(MoveCameraCoroutine(panel.cameraTargetPosition, Quaternion.Euler(panel.cameraTargetRotation)));
        }
        else
        {
            // 설정되어 있지 않다면, 기본 위치로 카메라를 되돌립니다.
           // activeMoveCoroutine = StartCoroutine(MoveCameraCoroutine(defaultPosition, defaultRotation));
        }
    }

    /// <summary>
    /// 지정된 위치와 회전값으로 카메라를 부드럽게 움직이는 코루틴입니다.
    /// </summary>
    private IEnumerator MoveCameraCoroutine(Vector3 targetPosition, Quaternion targetRotation)
    {
        Vector3 startPosition = mainCameraTransform.position;
        Quaternion startRotation = mainCameraTransform.rotation;
        float elapsedTime = 0f;

        float duration = Mathf.Max(moveDuration, rotateDuration);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            // 💡 여기가 핵심: 선형적인 진행률 대신, AnimationCurve로 계산된 '가속도가 적용된 진행률'을 사용합니다.
            
            // 이동 진행률 계산
            float moveProgress = Mathf.Clamp01(elapsedTime / moveDuration);
            float easedMoveProgress = moveCurve.Evaluate(moveProgress); // 커브 그래프에 따른 현재 값 계산

            // 회전 진행률 계산
            float rotateProgress = Mathf.Clamp01(elapsedTime / rotateDuration);
            float easedRotateProgress = moveCurve.Evaluate(rotateProgress);

            // 위치 이동
            if (moveDuration > 0)
            {
                mainCameraTransform.position = Vector3.LerpUnclamped(startPosition, targetPosition, easedMoveProgress);
            }

            // 회전
            if (rotateDuration > 0)
            {
                mainCameraTransform.rotation = Quaternion.SlerpUnclamped(startRotation, targetRotation, easedRotateProgress);
            }
            
            yield return null;
        }

        mainCameraTransform.position = targetPosition;
        mainCameraTransform.rotation = targetRotation;
    }
}