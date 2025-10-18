# SEO : Success Expectation Order




Unity UGUI의 고질적인 강한 결합도(Tight Coupling)와 복잡한 의존성 문제를 해결하여, UI가 복잡해져도 스파게티 코드가 되지 않는 견고하고 확장 가능한 UI 프레임워크를 설계하고 구현했습니다.

이 프로젝트는 단순히 UI를 화면에 그리는 것을 넘어, SOLID 원칙과 이벤트 기반 아키텍처를 통해 UI 시스템이 어떻게 게임의 다른 시스템(카메라, 오디오, 게임 로직)과 완벽하게 분리되어 독립적으로 작동할 수 있는지에 대한 해답을 제안합니다.

## 시연 영상
[![Watch the video](https://img.youtube.com/vi/oO4ZeWpcWMA/maxresdefault.jpg)](https://www.youtube.com/watch?v=oO4ZeWpcWMA)


##  프로젝트 목표

느슨한 결합 (Loose Coupling): UI, 카메라, 오디오, 게임 로직 등 각 시스템이 서로를 직접 참조하지 않고, '방송'을 통해 소통하는 구조를 만듭니다. 이를 통해 UI 수정이 다른 시스템에 버그를 유발하는 사이드 이펙트를 원천적으로 차단합니다.

확장성 (Scalability): 새로운 UI 패널이나 기능을 추가할 때, 기존 코드를 수정하지 않고도 시스템에 안전하게 통합할 수 있는 개방-폐쇄 원칙(OCP)을 준수하는 설계를 목표로 합니다.

재사용성 (Reusability): UI의 각 파트를 독립적으로 작동하는 '레고 블록'처럼 설계하여, 어떤 프로젝트나 씬에서도 쉽게 가져다 쓸 수 있는 컴포넌트 기반 아키텍처를 구현합니다.

지능형 사용자 경험 (Intelligent UX): 사용자의 선택 경로와 상태를 기억하여 반복 조작을 최소화하고, 의미 있는 애니메이션으로 시선을 유도하여 사용자의 인지 부하를 줄이는 것을 목표로 합니다.

## 핵심 아키텍처 (Core Architecture)

이 프레임워크는 여러 검증된 디자인 패턴과 원칙의 조합으로 이루어져 있는데요..

1. 이벤트 기반 아키텍처

모든 시스템은 `UIEvents`라는 중앙 방송국을 통해서만 소통합니다. `UIManager`가 "GaragePanel이 열렸다!"라고 방송하면, `CameraManager`, `AudioManager`, `MenuSwitcher` 등 관심 있는 모든 컴포넌트가 이 방송을 듣고 각자 맡은 일을 자율적으로 수행합니다.


2. 재귀적 컴포넌트 구조

복잡한 UI 계층을 "스스로 자기 자식을 책임지는 똑똑한 패널"의 반복으로 해결했습니다.

`UIManager` : `MainMenu`, `InGameUI` 등 최상위 패널의 전환이라는 전략적 결정만 내립니다.

`ContentSwitcher` : 각 패널 내부에 존재하며, 자신의 하위 콘텐츠 패널들을 전환하는 전술적 책임을 가집니다.
이 구조 덕분에 UI 계층이 아무리 깊어져도, 각 컴포넌트는 자신의 직속 부하만 관리하므로 시스템의 복잡도가 선형적으로 유지됩니다.

## 주요 기능 및 구현 상세

1. ID 기반 패널 자동 등록 시스템
Awake() 시점에 `FindObjectsByType`을 호출하여, `isManagedByUIManager` 플래그가 설정된 모든 `UIPanel`을 `Dictionary`에 자동으로 등록합니다. 새로운 UI를 추가할 때 개발자가 직접 `UIManager`에 참조를 연결할 필요가 없어 휴먼 에러를 방지하고, 코드 수정 없이 시스템을 확장할 수 있습니다.

2. 적응형 뒤로가기 구현: `Stack<GameObject>` 대신 
`Stack<Action>`을 사용하여, 이전 패널을 다시 여는 '행동' 자체를 저장합니다. 이를 통해 UI 패널이 다시 나타날 때, 내부 상태까지 복원하여 사용자 경험의 연속성을 보장합니다.

3. 디자이너 친화적인 절차적 애니메이션
부모 `UIPanel`이 자식 `UIComponentAnimator`들을 Delay 값에 따라 순차적으로 활성화시키는 시스템을 구축했습니다. 디자이너가 코드 수정 없이 `AnimationCurve`로 직접 애니메이션의 가속도와 느낌을 조절할 수 있게 하여, 개발과 디자인의 협업 효율을 높이고 의미 있는 UI 연출을 가능하게 합니다.


## Copyright
**Need For Speed** : 영상 일부 사용 (2015 NFS, 2017 NFS:Heat)

**aespa - Rich Man** : Adaptive Audio 시스템 시연 오디오로 사용
