# RepHack

C# 콘솔 기반 ASCII 로그라이크. NetHack에서 영감을 받아 제작.

![gameplay](./screenshots/gameplay.gif)

## 핵심 시스템

### 던전 생성

초기에는 랜덤 좌표 배치 + AABB 충돌 검사 방식을 사용했다. 랜덤 위치에 방을 놓고, 기존 방과 겹치면 재시도하는 단순한 구조. 작동은 하지만 방 배치가 한쪽에 몰리거나 시도 횟수 상한(100회)에 걸려 방이 적게 생성되는 문제가 있었다.

이를 해결하기 위해 BSP(Binary Space Partitioning) 방식으로 교체했으며, Dungeon의 ConnectionRoom 메서드로 연결했다. 이유는 두 가지다.

- **공간 활용도 보장** — 분할 자체가 전체 맵을 커버하기 때문에, 방이 한쪽에 몰리거나 빈 공간이 과도하게 생기는 문제가 구조적으로 발생하지 않는다.
- **복도 연결의 자연스러운 구조** — 트리의 형제 노드끼리 복도를 연결하면, 별도의 연결 알고리즘 없이 모든 방이 도달 가능하다는 것이 보장된다.


### 적 AI — BFS 경로 탐색

적은 매 턴마다 BFS를 실행하여 플레이어까지의 최단 경로를 계산하고, 한 칸 이동한다.

A*를 쓰지 않은 이유는 맵이 균일 비용 그리드(모든 타일의 이동 비용이 동일)이기 때문이다. 이 조건에서 BFS는 A*와 동일한 최적 경로를 보장하면서, 휴리스틱 계산과 우선순위 큐 오버헤드가 없다. 타일 비용이 다양해지는 시점(늪 지형, 속도 디버프 등)이 오면 그때 A*로 교체하는 것이 합리적이라고 판단했다.

```csharp
// 경로 역추적: BFS로 플레이어까지의 경로를 찾은 뒤,
// cameFrom 배열을 따라 시작점까지 역추적하여 "다음 한 칸"을 결정한다.
var lastPos = pos;
while (pos != (enemyX, enemyY))
{
    lastPos = pos;
    pos = cameFrom[lastPos.y, lastPos.x];
}
```

### 게임 루프

`Program.Main()`에서 `Start() → Update() → Render()` 순서로 실행되는 표준 게임 루프 패턴을 사용한다.

입력 처리는 `Dictionary<ConsoleKey, Actions>` 키맵을 통해 키 입력을 액션으로 변환하고, `Dictionary<Actions, Action>` 으로 액션을 실행 함수에 바인딩한다. switch문 없이 `TryGetValue → Invoke` 한 줄로 입력-실행이 완료되는 구조다.

```csharp
if (keyMap.TryGetValue(control.GetInput(), out Action? act))
{
    act.Invoke();
}
```

### 클래스 구조

| 클래스 | 역할 |
|--------|------|
| `Game` | 게임 루프, 시스템 간 조율 |
| `Player` | 플레이어 상태, 이동, 인벤토리 |
| `Enemy` | 적 베이스 클래스. `Slime`, `Goblin` 등이 상속 |
| `Item` | 아이템 베이스 클래스. `PotionItem`, `WeaponItem` 등이 상속 |
| `Dungeon` | 맵 생성, 방/복도 배치 |
| `Renderer` | 버퍼 기반 렌더링, UI 출력 |
| `Control` | 입력 처리, 키 바인딩 |
| `Pathfinding` | BFS, 길찾기 알고리즘. 매 BFS마다 배열을 새로 만드는 메모리 낭비를 줄이기 위해 빼놓음 |
| `FieldOfView` | Shadowcasting을 이용한 시야처리 |

## 구현 완료

- 랜덤 던전 생성 (방 배치 + 복도 연결)
- BFS 기반 적 AI
- 층 전환 시스템
- 아이템 습득 및 사용
- 인벤토리 시스템
- 버퍼 기반 렌더링
- 게임오버 처리

## 향후 계획

- **적 행동 다양화** — `Enemy` 클래스에 `virtual void Act()` 추가. 적 타입별 고유 행동 패턴 (추적, 순찰, 도주 등)
- **장비 시스템** — 무기/방어구 장착, 스탯 반영
- **스크롤/포션 효과 다양화** — 미확인 아이템 식별 시스템 (NetHack의 핵심 시스템)

## 자잘한 고칠 사항들
- minRoomNumber

## 조작

| 키 | 동작 |
|----|------|
| `↑↓←→` | 이동 / 인접한 적 공격 |
| `,` | 아이템 줍기 |
| `I` | 인벤토리 열기 |
| `123456789` | 포션 사용 |

## 빌드

```bash
dotnet run --project RepHack
```

.NET 8.0 이상 필요.
