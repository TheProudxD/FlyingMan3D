# FlyingMan3D — подробное описание проекта

Документ составлен по текущему состоянию репозитория после отдельного изучения `AI/UNITY_COMPOSITION_GUIDE.md`.

## 1. Что это за игра

`FlyingMan3D` — мобильный hyper-casual / arcade-проект на Unity, где игрок запускает персонажа из рогатки, управляет им в полёте, проходит через кольца-модификаторы, наращивает или теряет количество бойцов, а в финале сталкивает собранную группу с пачкой врагов у конца платформы.

Краткая идея из `README.md` полностью подтверждается кодом:

- стартовый персонаж закреплён в рогатке;
- сила запуска выбирается по oscillating-индикатору;
- после выстрела игрок управляет полётом по оси `X`;
- по пути появляются кольца, которые изменяют численность толпы;
- у финиша спавнится группа врагов;
- после входа в финишную зону персонажи переходят из режима полёта в режим ближнего боя;
- победа достигается, если враги закончились раньше, чем закончились игроки.

По ощущениям и по коду это игра, ориентированная на mobile-пайплайн:

- вертикальные скриншоты в `README.md`;
- touch/mouse-first управление;
- рекламные сценарии `app open`, `interstitial`, `rewarded`;
- powerup shop перед запуском;
- skip-level за rewarded ad;
- окно win с множителем награды.

## 2. High-Level gameplay loop

Ниже фактический игровой цикл, как он реализован сейчас:

1. Игра стартует через `Bootstrap` сцену.
2. Инициализируются core-сервисы: addressables, configs, audio, localization, graphics, ads.
3. Загружается `Main` сцена.
4. Поднимаются прогресс игрока, UI root и HUD.
5. Для уровня создаются:
   - рогатка;
   - главный персонаж;
   - динамический контент уровня;
   - финиш;
   - кольца;
   - враги;
   - бочки.
6. HUD показывает состояние “tap to throw” и powerup shop.
7. Игрок кликом останавливает индикатор силы и запускает персонажа.
8. Во время полёта удержанием/движением мыши влево-вправо корректируется траектория.
9. Персонажи проходят через кольца:
   - `+N`;
   - `xN`;
   - `-N`;
   - `/N`.
10. При первом входе в `Finish` включается боевая фаза:
   - камера переключается на финишную;
   - враги активируют AI;
   - игроки переходят в autonomous movement / melee mode.
11. Победа и поражение вычисляются по счётчикам живых `players` и `enemies`.
12. После победы выдаётся награда, доступен ad-множитель и переход на следующий уровень.
13. После поражения показывается lose window с рестартом.

## 3. Сцены и runtime scopes

В build settings включены только две сцены:

- `Assets/_Project/Scenes/Bootstrap.unity`
- `Assets/_Project/Scenes/Main.unity`

### Bootstrap

Bootstrap сцена отвечает за старт приложения.

Наблюдения:

- содержит `GameBootstraper`;
- содержит `BootstrapInstaller`;
- содержит `SceneScope` для Reflex;
- содержит объект `YGBootrstraper`, что указывает на следы интеграции Yandex Games / похожего bootstrap-слоя.

### Main

Main сцена — основная игровая сцена.

Из YAML видны корневые объекты:

- `EventSystem`
- `Platform`
- `Environment`
- `Spawner`
- `Directional Light`
- `SceneScope`
- декоративные `LeftBuildings` / `RightBuildings`

### Project scope

Глобальные сервисы поднимаются не напрямую на сцене, а через `Assets/Resources/ProjectScope.prefab`.

В этом prefab подтверждён `ProjectInstaller`, который биндингует project-wide зависимости:

- state machine;
- factories;
- config service;
- audio;
- windows;
- persistent progress;
- ads;
- metrics;
- level lifecycle;
- resource services;
- camera;
- loading curtain;
- и др.

Итого в проекте фактически три уровня инициализации:

- `ProjectScope.prefab` — глобальный DI scope;
- `Bootstrap` scene scope — запуск и инъекция bootstrap-объектов;
- `Main` scene scope — сцено-специфичные ссылки (`Indicator`, `Spawner`, `Platform`).

## 4. Архитектурный стиль проекта

Проект лучше всего описывать как гибрид:

- `DI + Service Layer`
- `State Machine`
- `Factories`
- `Addressables`
- `ScriptableObject configs`
- `Observable values`
- `MonoBehaviour gameplay objects`

Это не “чистая компонентная архитектура” из гайда один-в-один, но и не monolithic scene-script project. Скорее это service-oriented mobile-game architecture с локальными попытками композиции на уровне gameplay.

## 5. Основной архитектурный поток

### 5.1. Bootstrap и запуск игры

Ключевые файлы:

- `Assets/_Project/Scripts/Infrastructure/GameBootstraper.cs`
- `Assets/_Project/Scripts/Infrastructure/FSM/States/BootstrapState.cs`

Логика:

- `GameBootstraper.Start()` резолвит `StateMachine` и запускает `Initialize()`;
- `StateMachine.Initialize()` входит в `BootstrapState`;
- `BootstrapState` показывает loading curtain, инициализирует ads, assets, configs, audio, localization и graphics;
- затем грузит `Main` сцену и переводит игру в `LoadGameState`.

### 5.2. State Machine

Состояния зарегистрированы централизованно в `ProjectInstaller`:

- `BootstrapState`
- `LoadGameState`
- `LoadLevelState`
- `GameLoopState`
- `WinLevelState`
- `LoseLevelState`
- `RestartLevelState`
- `ReplayLevelState`
- `ContinueLevelState`

Реальный happy path:

- `BootstrapState`
- `LoadGameState`
- `LoadLevelState`
- `GameLoopState`
- `WinLevelState` или `LoseLevelState`

Дополнительные переходы:

- `RestartLevelState`
- `ReplayLevelState`

`ContinueLevelState` существует, но фактически пустой и сейчас не образует рабочего gameplay branch.

### 5.3. DI через Reflex

Основные installers:

- `BootstrapInstaller`
- `MainSceneInstaller`
- `ProjectInstaller`

Что важно:

- `BootstrapInstaller` после сборки контейнера вручную инжектит все `LocalizedLabel` и активирует `GameBootstraper`;
- `MainSceneInstaller` собирает `LevelSceneReferences` и передаёт их в `GameFactory` и `LevelLifecycleService`;
- `ProjectInstaller` объявляет почти все глобальные сервисы как singletons.

Это хороший слой orchestration, близкий к идеям из `UNITY_COMPOSITION_GUIDE.md`: зависимости централизованы, сервисы явно разделены по ответственности.

## 6. Ключевые архитектурные подсистемы

### 6.1. AssetProvider и Addressables

Ключевой файл:

- `Assets/_Project/Scripts/Infrastructure/Services/AssetManagement/AssetProvider.cs`

`AssetProvider` — центральная точка загрузки и инстанцирования runtime-ассетов.

Через него создаются:

- `Player`
- `Enemy`
- `BigEnemy`
- `LargeEnemy`
- `Finish`
- `Ring`
- `Barrel`
- `Smoke`
- `Slingshot`
- `UIRoot`
- `LoadingCurtain`
- `Audio`
- `CameraSetup`
- window prefabs

Addressable keys вынесены в `AssetPath.cs`.

Отдельно важно:

- level data берётся из `LevelContainer`;
- ring effect component навешивается динамически по `RingType` через `AddComponent`;
- после инстанцирования объекты инжектятся через Reflex container.

### 6.2. ConfigService

Ключевые файлы:

- `Assets/_Project/Scripts/Infrastructure/Services/Config/ConfigService.cs`
- `Assets/_Project/Configs/ConfigContainer.asset`
- `Assets/_Project/Configs/UI/WindowsData.asset`

`ConfigService` держит типизированный доступ к `ScriptableObject`-конфигам.

По факту в `ConfigContainer` подключены:

- `AudioConfig`
- `CoinRewardAnimationConfig`
- `WinWindowAnimationsConfig`
- `LevelContainer`
- `WinImagesConfig`

Отдельно через addressables грузятся:

- `DesktopGraphics`
- `MobileGraphics`
- locale assets

### 6.3. Factories

Ключевые factory-сервисы:

- `GameFactory`
- `PlayerFactory`
- `EnemyFactory`
- `UIFactory`
- `FxFactory`

Роли:

- `GameFactory` — high-level facade над level content и scene refs;
- `PlayerFactory` — создание и удаление игроков;
- `EnemyFactory` — создание и удаление врагов;
- `UIFactory` — создание UI root и регистрация window factories;
- `FxFactory` — дым и ragdoll VFX.

### 6.4. LevelLifecycleService

Ключевой файл:

- `Assets/_Project/Scripts/Infrastructure/Services/Level/LevelLifecycleService.cs`

Это один из самых важных сервисов проекта.

Он:

- хранит runtime-списки игроков и врагов;
- держит `ObservableVariable<int>` для `PlayersCounter` и `EnemiesCounter`;
- создаёт главного игрока;
- создаёт клонов игроков;
- создаёт врагов;
- создаёт финиш;
- создаёт кольца;
- создаёт бочки;
- создаёт рогатку;
- очищает level holder при рестарте/перезагрузке.

По сути это runtime lifecycle manager всей игровой сцены.

### 6.5. ObservableVariable

Ключевой файл:

- `Assets/_Project/Scripts/Infrastructure/Observable/ObservableVariable.cs`

На этой простой observable-модели держатся:

- counters в HUD;
- отображение денег;
- уровень;
- таймер;
- стрелка reward wheel;
- другие view/reactive-сценарии.

Это облегчённый event-driven слой.

### 6.6. Window system

Ключевые файлы:

- `WindowService`
- `WindowRegistry`
- `WindowResourceManager`
- `WindowBase`
- `UIContainer`

Текущий зарегистрированный набор окон:

- `Pause`
- `Lose`
- `Tutorial`
- `Leaderboard`
- `Win`
- `HUD`

Окна создаются через `UIFactory`, а не лежат постоянно в сцене.

При закрытии:

- prefab instance уничтожается;
- addressable asset освобождается через `WindowResourceManager`.

### 6.7. Camera and Audio

Камера:

- `CameraSetup` persistent;
- `CameraService` переключает setup между режимом полёта и финишным видом;
- все world-space canvases получают камеру через `CameraBinder`.

Аудио:

- `AudioService` — фасад;
- `AudioServiceView` — persistent view с отдельными `AudioSource` для SFX и music;
- конфиг звуков лежит в `AudioConfig`.

### 6.8. Сохранения и ресурсы

Ключевые файлы:

- `SaveLoadService`
- `PersistentProgressService`
- `PlayerProgress`
- `PowerupProgress`
- `LevelResourceService`
- `MoneyResourceService`

Что хранится:

- текущий уровень;
- деньги;
- лучший достигнутый уровень;
- powerup progression:
  - `health`
  - `movingSpeed`
  - `flyingControl`

Бэкенд сохранения сейчас простой:

- `PlayerPrefs`

Стартовые значения:

- `CurrentLevel = 1`
- `Money = 250`
- `RichestLevel = -1`
- `health = 1`
- `movingSpeed = 3.5`
- `flyingControl = 65`

## 7. Gameplay-системы

### 7.1. Игрок как набор специализированных контроллеров

Ключевые файлы:

- `PlayerController`
- `PlayerInitializer`
- `PlayerMovementController`
- `PlayerLaunchController`
- `PlayerDeathHandler`
- `PlayerFinishMover`

Это одна из самых сильных точек проекта в контексте `UNITY_COMPOSITION_GUIDE.md`.

Вместо одного большого `Player` проект уже разложен на отдельные responsibility blocks:

- `PlayerInitializer` собирает ссылки и готовит объект;
- `PlayerMovementController` отвечает за боковое управление;
- `PlayerLaunchController` отвечает за анимацию рогатки и стартовый импульс;
- `PlayerDeathHandler` отвечает за смерть и ragdoll;
- `PlayerFinishMover` отвечает за поведение в финальной боевой фазе;
- `PlayerController` выступает orchestration/facade-слоем.

Дополнительная особенность:

- в `Awake()` `PlayerController` умеет сам добавить недостающие саб-компоненты через `AddComponent`, чтобы старый prefab не ломался после рефакторинга.

### 7.2. Launch mechanic

Связка файлов:

- `Indicator`
- `Needle`
- `PlayerLaunchController`
- `Slingshot`

Как это работает:

- `Indicator` вращает стрелку туда-сюда;
- по клику рассчитывается launch factor;
- фактор дискретный, не непрерывный;
- `PlayerLaunchController` сначала оттягивает capsule рогатки назад;
- затем резко возвращает её;
- после этого всем rigidbody игрока задаётся линейная скорость и torque.

Это не просто “один rigidbody-выстрел”, а запуск полноценного ragdoll-персонажа.

### 7.3. Полёт и управление

Во время полёта:

- игрок удерживает `Mouse0`;
- `PlayerMovementController` берёт `Input.GetAxis("Mouse X")`;
- всем rigidbody персонажа добавляется боковая скорость по `X`;
- при выходе за границы `±40` по `X` включается correction velocity.

Это означает, что crowd управляется физически, а не character controller-ом или navmesh-персонажем.

### 7.4. Спавн уровня по баллистике

Ключевой файл:

- `Spawner.cs`

Это один из самых характерных gameplay-сценариев проекта.

`Spawner` не берёт готовую layout-сцену уровня. Он рассчитывает level content динамически, исходя из параметров полёта:

- по стартовой скорости считает время подъёма;
- по баллистике вычисляет позиции будущих колец;
- по траектории вычисляет дальность и положение финиша;
- спавнит вокруг финиша врагов;
- создаёт баррели рядом с финишем;
- окрашивает платформу и кольца в palette текущего level index.

То есть уровень представляет собой не заранее расставленную сцену, а комбинацию:

- статической main scene;
- `Level` ScriptableObject;
- runtime spawning.

### 7.5. Level data

Ключевые файлы:

- `Level.cs`
- `LevelContainer.cs`
- `Assets/_Project/Configs/Levels/*.asset`

Каждый `Level` содержит:

- набор `UpperRing[]`
- `TimeDif`
- `MaxLaunchSpeed`
- `StopDistance`
- `MoneyReward`
- `BarrelAmount`
- список `EnemyData`

`LevelContainer` использует modulo-indexing:

- после прохождения 40-го уровня конфиги переиспользуются циклически;
- номер уровня в прогрессе продолжает расти, но контент берётся по `(currentLevel - 1) % LevelsCount`.

### 7.6. Кольца и модификация толпы

Ключевые файлы:

- `RingBase`
- `AdditiveRing`
- `MultiplierRing`
- `ReducerRing`
- `DividerRing`
- `RingData`
- `RingHolder`

Эффекты:

- `AdditiveRing` добавляет фиксированное количество игроков;
- `MultiplierRing` размножает каждого проходящего игрока;
- `ReducerRing` удаляет фиксированное число игроков;
- `DividerRing` сокращает толпу делением.

Особенности реализации:

- ring logic не хранится в prefab заранее;
- нужный подкласс навешивается runtime через `AssetProvider.GetRingByType(...)`;
- кольца умеют двигаться по `MovementAxis` с заданной `Speed`;
- при старте кольцо само пишет на `TMP_Text` свой символ и значение.

### 7.7. Клонирование игроков

Ключевой файл:

- `LevelLifecycleService.GetNewPlayer()`

Клоны создаются не абстрактно и не как счётчик, а как реальные новые `PlayerController`.

Что важно:

- новый игрок спавнится рядом с главным в случайной точке;
- копируется transform-hierarchy движение;
- для дочерних rigidbody копируется `linearVelocity`;
- clone сразу инициализируется как полноценный участник толпы.

Это значит, что увеличение численности реализовано физическими сущностями, а не одной цифрой на HUD.

### 7.8. Враги

Ключевые файлы:

- `EnemyBase`
- `Enemy`
- `BigEnemy`
- `LargeEnemy`
- `EnemyData`
- `EnemyType`

Рабочие типы врагов:

- `Simple`
- `Big`
- `Large`

Фактические параметры:

- `Simple`: скорость 5, здоровье 1;
- `Big`: скорость 1.2, здоровье 2;
- `Large`: скорость 0.95, здоровье 3.

Поведение:

- враги пассивны до входа в `Finish`;
- после `Initialize()` начинают искать ближайшую цель;
- бегут к ней напрямую;
- разворачиваются в сторону цели;
- умирают от падения, выхода за границы или потери здоровья;
- при смерти создают ragdoll.

Важно:

- `EnemyType.WithGun` и `EnemyType.WithGunAndShield` существуют в enum;
- но в `LevelLifecycleService` оба сейчас мапятся на обычный `Simple` enemy prefab;
- отдельных геймплейных различий у них нет.

### 7.9. Финиш и переход в боевую фазу

Ключевые файлы:

- `Finish.cs`
- `PlayerFinishMover.cs`
- `EnemyBase` subclasses
- `GameLoopState`

Первый игрок, вошедший в `Finish`, запускает battle transition:

- играет SFX;
- камера переключается на finish-camera;
- у всех врагов вызывается `Initialize()`;
- время кратко замедляется до `0.5`;
- игрок теряет launch-режим и переходит в combat-mode.

Каждый игрок, проходящий в финишную зону:

- переводится в специальный режим движения;
- получает `PlayerFinishMover`;
- перестаёт быть ragdoll-целью запуска;
- начинает искать ближайшего врага и бежать к нему.

### 7.10. Бой игрока в финале

`PlayerFinishMover` делает следующее:

- получает `StopDistance` из текущего уровня;
- получает `movingSpeed` из powerup progression;
- получает случайное здоровье в диапазоне `1..health powerup`;
- включает world-space canvas с цифрой HP;
- ищет ближайшего врага;
- двигается к нему;
- при столкновении обменивается уроном.

Фактически у каждого добежавшего до финиша бойца есть индивидуальное HP и melee state.

### 7.11. Условия победы и поражения

Условия живут в `GameLoopState`.

Поражение:

- если список игроков стал пустым.

Победа:

- если список врагов пуст;
- игроки ещё живы;
- объект `Finish` существует;
- индикатор запуска уже отключён.

То есть win/loss считаются через счётчики сущностей, а не через отдельный game manager с большим количеством flags.

### 7.12. Бочки

Ключевой файл:

- `ExplosionBarrel.cs`

Бочки:

- стоят рядом с финишем на части уровней;
- реагируют на столкновение с игроком/врагом;
- проигрывают скейл-анимацию;
- толкают nearby rigidbody импульсом;
- воспроизводят hit sound и particle splash;
- после этого исчезают.

### 7.13. Ragdoll и FX

Используются:

- player ragdoll;
- enemy ragdoll;
- smoke effect;
- win particle.

Рэгдоллы появляются при смерти персонажей и врагов и добавляют “физический” feel проекту.

### 7.14. Powerup shop

Ключевые файлы:

- `PowerupsManager`
- `PowerupView`

Доступные постоянные апгрейды:

- `Health`
- `MovingSpeed`
- `FlyingControl`

Влияние на геймплей:

- `Health` повышает запас здоровья бойца в финальной фазе;
- `MovingSpeed` ускоряет movement в финальной фазе;
- `FlyingControl` повышает lateral control во время полёта.

Покупка работает двумя путями:

- за soft currency;
- за rewarded ad, если денег не хватает.

Цена powerup:

- рассчитывается по `AnimationCurve`;
- progression сохраняется в `PlayerPrefs`.

## 8. UI и UX слой

### HUD

Ключевой файл:

- `Hud.cs`

HUD умеет переключаться между двумя фазами:

- pre-launch:
  - `TapToThrow`
  - `PowerupShop`
  - деньги
- in-run / combat:
  - счётчик игроков
  - счётчик врагов
  - pause
  - skip level при нужных условиях

### Tutorial

В проекте есть два tutorial-слоя:

- простая legacy-версия `UI/Tutorial/Tutorial.cs`;
- полноценное окно `TutorialWindow`.

`TutorialWindow`:

- скрывает HUD;
- показывает анимированную руку / курсор;
- умеет демонстрировать “tap to throw”;
- умеет демонстрировать “hold and drag”;
- закрывается по вводу или по таймеру.

### Pause / Lose / Win / Leaderboard

Что реализовано:

- `PauseWindow`
- `LoseWindow`
- `WinWindow`
- `LeaderboardWindow`

Особенности:

- `PauseWindow` останавливает `Time.timeScale`;
- `LoseWindow` даёт рестарт;
- `WinWindow` начисляет награду и предлагает ad multiplier;
- `LeaderboardWindow` существует как окно, но реальный remote leaderboard сейчас отключён.

### Win reward multiplier

Ключевые файлы:

- `WinWindow`
- `WheelMultiplierButton`
- `TargetArrowAnimation`

Это отдельная мини-система:

- на win-экране запускается oscillating arrow;
- кнопка пересчитывает reward по текущему углу;
- multiplier сейчас дискретный: `x2`, `x4`, `x6`;
- по rewarded ad можно заменить базовую награду на увеличенную.

### Audio toggles и локализуемые тексты

UI также поддерживает:

- `SoundSwitcher`
- `MusicSwitcher`
- `LocalizedLabel`

То есть в проекте заложен слой локализуемого интерфейса и настраиваемого audio UX.

## 9. Контент и численные наблюдения

На текущем snapshot в проекте:

- 40 authored level assets;
- 2 build scenes;
- 6 зарегистрированных window-prefabs;
- 4 locale assets: `ru`, `en`, `es`, `de`.

### Параметры уровней

По фактическим `.asset`-конфига:

- `MaxLaunchSpeed`: от `55` до `160`;
- `MoneyReward`: от `150` до `450`;
- суммарное число врагов на уровне: от `4` до `22`;
- уровни с движущимися кольцами: `37` из `40`;
- уровни с бочками: `10` из `40`.

Уровни с бочками:

- 7
- 9
- 14
- 18
- 19
- 20
- 24
- 28
- 33
- 36

### Суммарное использование типов врагов по всей кампании

- `Simple`: `281`
- `Big`: `115`
- `Large`: `64`

### Суммарное использование типов колец по всей кампании

- `Additive`: `153`
- `Multiplier`: `75`
- `Reducer`: `76`
- `Divider`: `51`

### Примеры прогрессии

Ранние уровни:

- 1-й уровень: 4 простых врага, статичные кольца, награда 150;
- 2-й уровень: 5 простых врагов, всё ещё простая конфигурация.

Поздние уровни:

- появляются moving rings по `MovementAxis`;
- растёт launch speed;
- растёт количество mixed enemy packs;
- включаются `Big` и `Large`;
- в поздних конфигурациях встречаются сложные комбинации `x`, `+`, `-`, `/`.

## 10. Технологический стек

Engine:

- Unity `6000.1.4f1`

Ключевые библиотеки и пакеты:

- `Reflex` для DI
- `UniTask`
- `Addressables`
- `LitMotion`
- `NaughtyAttributes`
- `TNRD.SerializableInterface`
- `YandexMobileAds`
- `ProBuilder`

В проекте также лежат сторонние плагины/ассеты:

- `Joystick Pack`
- `Obi`
- `FastScriptReload`
- `Better Hierarchy`
- `MeshCombiner`
- assorted editor utilities

Но не все из них активно используются gameplay-кодом.

## 11. Насколько проект соответствует UNITY_COMPOSITION_GUIDE.md

### Что совпадает с философией гайда

- есть чёткий orchestration layer через `ProjectInstaller`, `GameFactory`, `LevelLifecycleService`, `PlayerController`;
- player logic разложена на несколько специализированных контроллеров;
- UI отделён от сервисов и опирается на DI;
- counters и UI-реакции опираются на observable/events;
- assets, windows, configs и scene refs разделены по слоям;
- многие системы действительно маленькие и переиспользуемые.

### Где проект остаётся гибридным, а не полностью “Unity Way”

- много gameplay-классов живут в global namespace;
- часто используется прямой `Input.*` вместо контроллеров ввода;
- много runtime `GetComponent`, `FindObjectOfType`, `FindObjectsByType`;
- ring behavior задаётся через наследование и runtime `AddComponent`;
- enemy logic построена на классическом inheritance tree;
- есть service-oriented orchestration, но не так много явной event-driven коммуникации между игровыми объектами.

### Итог по стилю

Проект уже движется в сторону compositional thinking, особенно в инфраструктуре и после рефакторинга игрока, но общий код всё ещё гибридный:

- инфраструктура — достаточно зрелая;
- gameplay — частично композиционный, частично наследуемый, частично legacy.

## 12. Важные технические нюансы и незавершённые части

Это очень полезный раздел для будущих доработок.

### 12.1. Системы, которые выглядят незавершёнными или частично выключенными

- `ContinueLevelState` пустой.
- В `LoseWindow` код для continue/skip-компенсации закомментирован, кнопка skip скрыта.
- `Timer` и `TimerView` существуют, но таймер не запускается gameplay-потоком.
- `HeartTracker` зарегистрирован в DI, но фактически не подключён к HUD.
- `StatisticsWindow` существует как класс, но не зарегистрирован в `WindowId`/`UIFactory`/`WindowsData`.
- `ReviewThanksWindow` тоже существует, но не зарегистрирован как runtime window.
- `ReviewData` есть как config-класс, но не входит в `ConfigContainer`, значит `ConfigService.Get<ReviewData>()` сейчас не сможет корректно сработать.
- `CoinRewardAnimation` реализован, но его использование в `WinWindow` закомментировано.
- `ScoreView` содержит закомментированную инициализацию `_score` и выглядит как сломанный/inactive код.
- `InputReader` зарегистрирован как сервис, но core gameplay продолжает читать `Input` напрямую.
- `GameData` config-класс существует, но по текущему коду не используется.

### 12.2. Реклама и live-сервисы

- `MetricService` целиком состоит из точек расширения, но реальные `YG2.MetricaSend(...)` закомментированы.
- `LeaderboardService` хранит рекорд локально, но отправка в Yandex leaderboard закомментирована.
- `ReviewShowService.Show()` не вызывает реальный SDK, потому вызов из `WinLevelState` сейчас ничего не показывает.
- `LocalizationService` может работать с несколькими языками, но `DefineLanguage()` принудительно включает `ru`.

### 12.3. Потенциально опасные детали реализации

- `DeviceSpecificGraphics.IsMobileDevice()` сейчас всегда возвращает `true`, так что фактически всегда применяется mobile graphics preset.
- `RewardedAdController.Show()` вызывает reward callback сразу, если реклама не готова.
- Это значит, что skip level / powerup / reward multiplier могут выдать награду даже без успешного показа ad.
- `InterstitialAdController` после показа уничтожает interstitial, но не запрашивает новый, поэтому interstitial-flow выглядит одноразовым до следующей инициализации.
- `EnemyType.WithGun` и `EnemyType.WithGunAndShield` не имеют отдельных prefab/behavior и сводятся к `Simple`.
- `SoundSwitcher` и `MusicSwitcher` умеют читать `PlayerPrefs`, но `LoadSettings()` не вызывается и toggle по сути всегда стартует из `true`.

### 12.4. Тесты

- В проекте есть папки `Assets/_Project/Scripts/Tests/Editor` и `Runtime`.
- Реальных test scripts в этих папках сейчас нет.

## 13. Что открыть в первую очередь, если нужно быстро понять проект

Если нужен короткий маршрут по коду, я бы открывал в таком порядке:

1. `Assets/_Project/Scripts/Infrastructure/DI/ProjectInstaller.cs`
2. `Assets/_Project/Scripts/Infrastructure/FSM/States/BootstrapState.cs`
3. `Assets/_Project/Scripts/Infrastructure/FSM/States/LoadGameState.cs`
4. `Assets/_Project/Scripts/Infrastructure/FSM/States/LoadLevelState.cs`
5. `Assets/_Project/Scripts/Infrastructure/Services/Factories/GameFactory.cs`
6. `Assets/_Project/Scripts/Infrastructure/Services/Level/LevelLifecycleService.cs`
7. `Assets/_Project/Scripts/Gameplay/Spawner.cs`
8. `Assets/_Project/Scripts/Gameplay/PlayerController.cs`
9. `Assets/_Project/Scripts/Gameplay/PlayerFinishMover.cs`
10. `Assets/_Project/Scripts/Gameplay/Rings/*.cs`
11. `Assets/_Project/Scripts/Gameplay/Enemies/*.cs`
12. `Assets/_Project/Scripts/UI/Hud.cs`
13. `Assets/_Project/Scripts/UI/Windows/WinWindow.cs`

## 14. Краткий вывод

`FlyingMan3D` — это не просто “одна сцена с персонажем и кольцами”, а уже довольно плотный mobile-game проект со следующими слоями:

- bootstrap и DI;
- state machine;
- factory-based runtime spawning;
- level configs на `ScriptableObject`;
- addressable content;
- separate UI window system;
- persistent progress;
- monetization hooks;
- локализация;
- combat-at-finish gameplay loop;
- powerup progression.

Самая сильная сторона проекта — комбинация:

- динамической генерации run-сегмента по параметрам запуска;
- физического ragdoll-полёта;
- crowd-size modification через кольца;
- переключения в финальную боевую фазу у конца платформы.
