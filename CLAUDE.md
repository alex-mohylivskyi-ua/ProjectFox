# ProjectFox — Claude Code Rules

## Project

Unity 6000.3.8f1, Built-in Render Pipeline, C# 9.0, net471.
2D metroidvania / action-platformer.
Philosophy: **game-first, template-aware** — build the actual game, extract reusable systems only when they clearly become core platformer building blocks. Do not over-engineer prematurely.

## Architecture overview

- `Entity` — base for Player and Enemy. Owns: Animator, Rigidbody2D, StateMachine, facing direction, flip, SetVelocity, collision proxy properties.
- `Player : Entity` — orchestrator, not a god object. Owns: player states, PlayerInputReader, PlayerMovement, PlayerAbilities, PlayerMovementData, PlayerCombatData, jump buffer timer, coyote timer, player death event.
- `Enemy : Entity` — owns enemy states, patrol/battle/detection logic.
- Entity_* components are shared between Player and Enemy.

Do not duplicate Entity logic in Player or Enemy. Do not replace this architecture with a parallel one.
Do not introduce a parallel state machine, health system, damage system, input system, or collision system unless explicitly requested.

## Naming

Follow existing style exactly:
- Components: `Entity_Collision`, `Entity_Health`, `Entity_Knockback`, `Entity_Combat`
- States: `Player_IdleState`, `Player_DashState`, `Enemy_BattleState`

Do not use alternative naming like `CharacterMotorSystem`, `PlayerLocomotionController`, `StateBehaviourBase`, `CombatRuntimeService`.

## Folder structure

```
Assets/Scripts/              — shared entity components (Entity, Entity_Health, etc.)
Assets/Scripts/Data/         — ScriptableObjects (PlayerMovementData, PlayerCombatData, EnemyData)
Assets/Scripts/PlayerStates/ — player states
Assets/Scripts/EnemyStates/  — enemy states
Assets/Scripts/StateMachine/ — StateMachine, EntityState, PlayerState, EnemyState
Assets/Scripts/Environments/ — Hazards and environment scripts
```

Do not create random new folders unless they serve a clear purpose.

## Unity patterns

- `[SerializeField] private` fields for Inspector assignment.
- Public read-only properties for external access.
- MonoBehaviour for runtime scene behavior.
- ScriptableObject for reusable tuning/config (not runtime mutable state).
- No interfaces, factories, service locators, DI containers, event buses, or generic framework layers unless there is a concrete current need.

## Collision (Entity_Collision)

- All ground/wall/raycast/boxcast detection lives in `Entity_Collision`. Do not move it back into Entity, Player, Enemy, or generic states.
- Entity exposes proxy properties: `groundDetected`, `wallDetected`, `wallSlideSurfaceDetected`.
- Entity_Collision must be on the same GameObject as Entity (uses GetComponent).
- Entity_Collision is responsible for: ground detection, wall detection, wall slide surface detection, and collision gizmos.
- Ground detection: dedicated `GroundCheck` child transform. Do not reuse BottomWallCheck as GroundCheck.
- Wall detection: `TopWallCheck` and `BottomWallCheck`.
- Recommended Player Ground Check Distance: 0.08–0.15. Size: X 0.65–0.8, Y 0.08–0.1.
- Temporary debug values: distance 0.2–0.3, size X 0.8–1.0, Y 0.1–0.15.
- Enemy patrol edge detection stays in Enemy or a future enemy-specific component — do not merge into Entity_Collision.

## State machine

- Use existing `StateMachine`, `EntityState`, `PlayerState`, `EnemyState`.
- Do not create a second state machine implementation.
- Do not initialize or transition to a null state.
- Do not re-enter the same state unless force is true. Use force only when a real re-enter is needed.
- Always call Exit on previous state before Enter on new state.
- Keep `OnStateChanged` event behavior.
- Keep `CurrentStateName` debug helper or an equivalent replacement.
- Transitions use `stateMachine.ChangeState(targetState)`.
- Gameplay transitions live inside states, not in Player.Update or Enemy.Update.

## Input

- States must not read Unity Input System actions directly.
- Use `PlayerInputReader`: `player.inputReader.jumpPressed`, `jumpHeld`, `attackPressed`, `dashPressed`, `player.moveInput`.
- Do not add a future `IPlayerInputReader` interface unless it is part of the requested task.

## Jump buffer and coyote time

- Jump buffer and coyote time are core platformer feel features. Do not remove them when changing jump or fall logic.
- Keep jump buffer timer, buffered jump release, and coyote timer as Player-level timing logic unless explicitly refactoring.
- States consume jump and coyote timing through Player methods and properties, not by reading timers directly.

## Tuning data

- Movement tuning → `PlayerMovementData` (ScriptableObject): move speed, jump force, jump buffer time, coyote time, jump cut values, wall jump force, air movement, apex movement, fall gravity multiplier, max fall speed, wall slide slow multiplier, dash speed, dash duration.
- Combat tuning → `PlayerCombatData` (ScriptableObject): basic attack velocity, attack velocity duration, combo reset time, jump attack velocity, and future player combat tuning.
- Do not add new tuning fields directly to Player if they belong in data assets.
- Do not store runtime mutable state in ScriptableObjects.

## PlayerMovement

- Owns reusable movement operations: jump, jump cut, air movement, apex control, fall gravity.
- States call `player.movement.*` methods — do not duplicate calculations in multiple states.

## PlayerAbilities

- MonoBehaviour (runtime state, per-player). Owns: CanDash, CanWallSlide, CanWallJump, CanDoubleJump, CanAirAttack.
- Do not move ability flags into a ScriptableObject. ScriptableObjects are shared config; abilities are per-player runtime state.

## Single-player assumptions

- Avoid adding new single-player assumptions where possible.
- Current known acceptable assumptions: static player death events, Enemy storing one player target, detection by Player tag.
- When touching player death events, prefer future-ready forms: events with Player payload or instance-based player death events.
- Only change the current player death event if the task requires it.

## Enemy

- Continue using `Enemy`, `EnemyState`, `Enemy_GroundedState`, and concrete enemy states. Do not replace with a separate AI system.
- Enemy currently has many tuning fields directly on it. When adding/refactoring, prefer creating or extending `EnemyData` ScriptableObject.
- Good EnemyData candidates: move speed, idle time, move animation speed multiplier, ground ahead check distance, attack distance, battle move speed, battle time duration, minimum retreat distance, retreat velocity, player check distance.
- Keep scene/object-specific references on Enemy: `groundAheadCheck`, `playerCheck`, LayerMasks (unless moved to data), current target/player transform.
- Do not create multiple enemy data assets too early. Start simple with one EnemyData.

## Enemy detection

- Keep it simple for now. Do not introduce a complex targeting system.
- If improving, prefer small steps: separate `whatIsGroundAhead` and `whatBlocksPlayerDetection` LayerMasks if needed.
- Avoid duplicate raycasts. Avoid hard failure if target is missing. Keep future multiplayer in mind.

## Damage and combat

- Damage always goes through `IDamagable.TakeDamage(float damage, Transform damageDealer)`.
- Do not hard-code damage to Player, Enemy, or Entity_Health. Works with any IDamagable (hazards, destructibles, future objects).
- `Entity_Combat` owns overlap-based attack detection and damage application. Do not duplicate this logic in animation triggers or states.
- Animation events call a thin trigger method → delegates to Entity_Combat.PerformAttack.

## Hazards

- Generic — damage any IDamagable allowed by LayerMask. Do not depend on Player directly.
- Keep support for: trigger damage, collision damage, damage cooldown/DPS, instant kill, and LayerMask filtering.
- Live in `Assets/Scripts/Environments/`.

## Knockback

- `Entity.SetVelocity` skips velocity if `Entity_Knockback.isKnocked`. Preserve this.
- Do not allow regular state movement to cancel knockback unless explicitly requested.

## Animation triggers

- Keep thin: notify current state (AnimationTrigger) and call Entity_Combat.PerformAttack.
- No gameplay decision trees or state transition logic inside animation trigger components.

## ScriptableObjects

- Inspector-friendly: use `[Header]`, `[Range]`, `[Min]`, `[CreateAssetMenu]`. Organize fields into readable Inspector sections.
- Config values only: speeds, forces, durations, distances, multipliers.
- No runtime state: current HP, current target, current input, cooldown timers, current state.

## Before adding new systems — inspect first

- Damage logic → inspect `IDamagable`, `Entity_Combat`, `Entity_Health`, `Hazards`.
- Movement logic → inspect `PlayerMovement`, `Entity.SetVelocity`, Player states.
- Input logic → inspect `PlayerInputReader`.
- Collision logic → inspect `Entity_Collision`.
- State behavior → inspect `StateMachine`, `EntityState`, `PlayerState`, `EnemyState`.

Extend existing systems when reasonable. Do not invent a parallel architecture.

## Null safety and validation

- Prefer null checks for Inspector references.
- Validate required components/data assets in Awake, Start, or OnValidate when useful.
- Avoid silent null reference crashes where Inspector assignment is expected.

## Debug.Log

- Use sparingly. Acceptable for important events or temporary debugging.
- Never log every frame or every physics check.
- Remove or guard temporary debug spam before committing.

## Refactoring approach

- Small safe incremental steps: add new data/component → wire in → migrate usages → remove old fields.
- Preserve working gameplay. Avoid large rewrites that change multiple systems at once.

## Current priority order

1. EnemyData ScriptableObject
2. Input lock / disabled input support
3. Improve player death event (instance-based, Player payload) for future multiplayer
4. Separate enemy target detection concerns if needed
5. Namespaces, documentation, sample prefabs — only when approaching template readiness

## What to avoid

God objects, duplicated systems, Player-only hard-coded damage, Input System calls inside states, collision logic back in Entity, runtime state in ScriptableObjects, rewriting the architecture without a clear reason, marketplace-level abstraction before multiple real use cases exist.


## Additional
Local multiplayer awareness rules.
Avoid static player references where possible.
Do not use FindObjectOfType<Player> or singleton Player.instance patterns.
Do not use CompareTag("Player") for damage or detection logic where multiple players could exist.
Use IDamagable and LayerMask filtering instead of Player-specific checks.
Enemy detection should work with a list of potential targets, not a hardcoded single Player reference.
Player death events should carry a Player reference as payload, not be static void events.
Prefer instance-based events on Player over static events where possible.
PlayerInputReader should be per-player, not shared or static.
Each Player instance should own its own PlayerInputReader, PlayerMovementData reference, PlayerCombatData reference, and PlayerAbilities.
Do not store shared mutable player state in ScriptableObjects.
When adding new systems that reference the player, ask whether the system could work with a list of players or an interface instead of a single Player reference.