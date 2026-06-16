---
apply: always
---


ProjectFox AI Architecture Rules
This is a Unity 6000.3.8f1 project using the Built-in Render Pipeline, C# 9.0, and target framework net471.
The project is a 2D metroidvania / action-platformer.
The development philosophy is game-first, template-aware.
The goal is to keep building the actual game while making core systems reusable enough to become a personal starter template for future 2D metroidvania/platformer projects.
Do not over-engineer the project into a generic framework too early. Prefer simple, Unity-friendly, Inspector-friendly solutions.
The current intended architecture is Entity with shared entity components, Player with player-specific input, movement, abilities, data and states, and Enemy with enemy-specific states, patrol, battle and detection logic.
Entity is the common base for Player and Enemy.
Entity may own or expose Animator, Rigidbody2D, StateMachine, facing direction, flip logic, velocity setting, and collision proxy properties.
Do not duplicate Entity logic in Player or Enemy if it already belongs to Entity.
Do not replace the existing architecture with a new unrelated architecture.
Do not introduce a parallel state machine, parallel health system, parallel damage system, parallel input system, or parallel collision system unless explicitly requested.
Use regular Unity patterns.
Use MonoBehaviour components for runtime scene behavior.
Use ScriptableObject assets for reusable tuning/configuration data.
Use serialized private fields for Inspector assignment.
Use public read-only properties for external access when needed.
Prefer serialized private fields with public getters over public mutable fields for new code.
Do not over-abstract too early.
Do not add interfaces, factories, service locators, dependency injection containers, event buses, or generic framework layers unless there is a concrete current need.
Extract repeated gameplay data into ScriptableObjects.
Extract reusable runtime logic into components.
Keep states simple and readable.
Do not create a large generic framework before there are multiple real use cases.
Do not replace simple Unity references with complex service systems.
Do not build marketplace-level architecture prematurely.
Preserve the current naming style.
The project currently uses names like Entity_Collision, Entity_Health, Entity_Knockback, Entity_Combat, Player_IdleState, Enemy_BattleState.
Use the same naming style for new related classes unless explicitly asked to rename or refactor the whole project.
Do not introduce random alternative naming styles like CharacterMotorSystem, PlayerLocomotionController, StateBehaviourBase, CombatRuntimeService unless specifically requested.
Collision detection has been extracted into Entity_Collision.
Do not move ground, wall, raycast or boxcast detection back into Entity, Player, Enemy, or generic states.
Use existing collision proxy properties from Entity such as groundDetected, wallDetected, and wallSlideSurfaceDetected.
Entity_Collision owns common ground and wall detection.
Entity_Collision is responsible for ground detection, wall detection, wall slide surface detection, and collision gizmos.
Entity_Collision should remain a reusable component shared by Player and Enemy.
Entity_Collision must be placed on the same GameObject as Entity, Player, or Enemy, because Entity uses GetComponent<Entity_Collision>().
Do not place Entity_Collision only on a child object.
Ground detection should use a dedicated GroundCheck child transform.
Do not reuse BottomWallCheck as GroundCheck.
Use GroundCheck for ground detection.
Use TopWallCheck and BottomWallCheck for wall detection.
Recommended Player Ground Check Distance is 0.08 to 0.15.
Recommended Player Ground Check Size is X 0.65 to 0.8 and Y 0.08 to 0.1.
For temporary debugging, Ground Check Distance can be 0.2 to 0.3 and Ground Check Size can be X 0.8 to 1.0 and Y 0.1 to 0.15.
Enemy patrol edge detection is enemy-specific.
Keep enemy ground-ahead checks in Enemy or in a future enemy-specific component.
Do not merge enemy ground-ahead detection into generic Entity_Collision unless there is a clear need.
If improving enemy detection later, consider separating whatIsGroundAhead and whatBlocksPlayerDetection into separate LayerMasks.
Use the existing StateMachine, EntityState, PlayerState, and EnemyState.
Do not introduce Unity Animator-only state logic as a replacement for gameplay states.
Do not create a second state machine implementation unless explicitly requested.
When modifying StateMachine, preserve its safety behavior.
Do not initialize StateMachine with a null state.
Do not change StateMachine to a null state.
Do not re-enter the same state unless force is true.
Always call Exit on the previous state before Enter on the new state.
Keep OnStateChanged event behavior.
Keep CurrentStateName debug helper or an equivalent replacement.
State transitions should use stateMachine.ChangeState(targetState).
Use force true only when a real re-enter is needed.
Gameplay transitions should usually live inside states.
Do not move all gameplay decision logic into Player.Update or Enemy.Update.
Player.Update should remain responsible mainly for reading input, handling player-wide timers like jump buffer and coyote time, and calling base entity update.
Player is the orchestrator, not a god object.
Player may own player states, input reader, movement helper, abilities, movement/combat data references, jump buffer timer, coyote timer, and player death event.
Do not move all state logic into Player.
Do not add large combat, movement, or collision algorithms directly into Player if they belong in PlayerMovement, PlayerState, Entity_Collision, Entity_Combat, PlayerAbilities, or ScriptableObject data.
States and gameplay code should not directly read Unity Input System actions.
Use PlayerInputReader for input.
Do not call Unity Input System actions directly from states.
Use player.inputReader.jumpPressed, player.inputReader.jumpHeld, player.inputReader.attackPressed, player.inputReader.dashPressed, and player.moveInput.
PlayerInputReader is the abstraction point for player input.
A future interface like IPlayerInputReader may be added later, but do not add it unless it is part of the requested task.
Jump buffer and coyote time are core platformer feel features.
Do not remove jump buffer or coyote time when changing jump or fall logic.
Keep jump buffer timer, buffered jump release, and coyote timer as Player-level timing logic unless explicitly refactoring them.
States should consume jump and coyote timing through Player methods and properties.
Movement tuning values should come from PlayerMovementData.
Do not add new movement tuning fields directly to Player if they belong in PlayerMovementData.
Values that belong in PlayerMovementData include move speed, jump force, jump buffer time, coyote time, jump cut values, wall jump force, air movement values, apex movement values, fall gravity multiplier, max fall speed, wall slide slow multiplier, dash speed, and dash duration.
Combat tuning values should come from PlayerCombatData.
Do not add new player combat tuning fields directly to Player if they belong in PlayerCombatData.
Values that belong in PlayerCombatData include basic attack velocity, attack velocity duration, combo reset time, jump attack velocity, and future player combat tuning values.
PlayerAbilities is runtime state.
PlayerAbilities is a MonoBehaviour because abilities are runtime state of a specific player.
Keep ability unlocks and locks in PlayerAbilities.
Use PlayerAbilities for CanDash, CanWallSlide, CanWallJump, CanDoubleJump, and CanAirAttack.
Do not move runtime ability flags into a shared ScriptableObject.
ScriptableObjects are shared config.
Abilities are per-player runtime state.
Avoid adding new single-player assumptions where possible.
Current known single-player assumptions include static player death events, Enemy storing one player target, and detection by Player tag.
They are acceptable for now, but avoid making them worse.
When touching player death events, prefer future-ready forms such as events with Player payload or instance-based player death events.
Only change the current player death event if the task requires it.
PlayerMovement owns reusable player movement operations.
PlayerMovement should contain reusable movement operations such as jump, jump cut, air movement, apex control, and fall gravity.
Do not duplicate these calculations inside multiple states.
States should call methods on player.movement when possible.
Enemy gameplay should continue using Enemy, EnemyState, Enemy_GroundedState, and concrete enemy states.
Do not replace enemy state logic with a completely separate AI system unless explicitly requested.
Enemy currently still has many tuning fields directly on Enemy.
When adding or refactoring enemy tuning values, prefer creating or extending EnemyData as a ScriptableObject.
Good candidates for EnemyData are move speed, idle time, move animation speed multiplier, ground ahead check distance, attack distance, battle move speed, battle time duration, minimum retreat distance, retreat velocity, and player check distance.
Keep scene references and object-specific references on Enemy, such as groundAheadCheck, playerCheck, LayerMasks unless explicitly moved to data, and current target/player transform.
Do not create multiple enemy data assets too early unless there is a real need.
Start simple with one EnemyData.
Enemy detection should stay simple for now.
Do not introduce a complex targeting system unless requested.
If improving enemy detection, prefer small steps.
Separate obstacle layer mask from ground-ahead mask if needed.
Avoid duplicate raycasts.
Avoid hard failure if target is missing.
Keep future multiplayer in mind.
Damage should be applied through IDamagable.
Do not hard-code damage only to Player, Enemy, or Entity_Health.
Damage systems should work with any object implementing IDamagable.
This keeps hazards, attacks, enemies, destructibles, and future objects reusable.
Entity_Combat is responsible for attack target detection and applying damage.
Do not duplicate overlap attack logic inside individual animation triggers or states unless there is a specific reason.
Animation events should call a small trigger method, which delegates to combat logic.
Hazards should stay generic.
Hazards should damage any IDamagable target allowed by LayerMask.
Do not make hazards depend directly on Player.
Keep support for trigger damage, collision damage, damage cooldown, instant kill, and LayerMask filtering.
Entity.SetVelocity already avoids overriding velocity while Entity_Knockback is active.
When changing movement or knockback, preserve this behavior.
Do not allow regular state movement to immediately cancel knockback unless explicitly requested.
Animation trigger components should stay thin.
Good responsibilities for animation trigger components are notifying the current state that an animation trigger happened and calling Entity_Combat.PerformAttack.
Avoid putting gameplay decision trees or state transition logic directly inside animation trigger components.
Use ScriptableObjects for reusable configuration.
Do not store per-run mutable state in ScriptableObjects unless intentionally designing persistent or shared state.
Good ScriptableObject config values include speeds, jump force, attack velocity, durations, distances, and multipliers.
Runtime state should not be stored in ScriptableObjects.
Runtime state includes current health, current target, current unlocked abilities, current input values, current cooldown timers, and current state.
Keep ScriptableObjects Inspector-friendly.
Use Unity attributes like Header, Range, Min, and CreateAssetMenu where helpful.
Organize fields into readable Inspector sections.
Use the existing folder organization.
Data ScriptableObjects should go into Assets/Scripts/Data.
Player states should go into Assets/Scripts/PlayerStates.
Enemy states should go into Assets/Scripts/EnemyStates.
State machine core should go into Assets/Scripts/StateMachine.
Hazards and environment scripts should go into Assets/Scripts/Environments.
Shared entity components should go into Assets/Scripts.
Do not create random new folders unless they serve a clear purpose.
Prefer small safe refactors.
When refactoring, preserve working gameplay.
Prefer incremental steps.
Add the new data or component first.
Wire it into existing code.
Keep compatibility properties if useful.
Move usages gradually.
Remove old fields only after all usages are migrated.
Avoid large rewrites that change multiple systems at once.
Before adding a new system, inspect existing related systems.
Before adding damage logic, inspect IDamagable, Entity_Combat, Entity_Health, and Hazards.
Before adding movement logic, inspect PlayerMovement, Entity.SetVelocity, and Player states.
Before adding input logic, inspect PlayerInputReader.
Before adding collision logic, inspect Entity_Collision.
Before adding state behavior, inspect StateMachine, EntityState, PlayerState, and EnemyState.
Extend existing systems when reasonable.
Do not invent a parallel architecture.
Null checks are preferred for Inspector references.
When adding required components or data assets, validate them in Awake, Start, or OnValidate when useful.
Avoid silent null reference crashes where the reference is expected to be assigned in the Inspector.
Use Debug.Log sparingly.
Debug logs are acceptable for important events or temporary debugging, but avoid logs every frame or every physics check.
Remove or guard temporary debug spam.
Current architectural priority order is: create or integrate EnemyData ScriptableObject, add input lock or disabled input support, improve player death event for future multiplayer, separate enemy target detection concerns if needed, and add namespaces, documentation, or sample prefabs only when moving closer to template or public readiness.
Do not jump directly to large framework work unless specifically requested.
Always follow this principle: build the current game first, but extract reusable systems when they clearly become core platformer/metroidvania building blocks.
Prefer simple components, states for gameplay flow, ScriptableObjects for tuning, interfaces only when there is a real use case, Inspector-friendly setup, and reusable but not over-generic code.
Avoid god objects, duplicated systems, hard-coded Player-only damage, direct Input System calls inside states, putting collision back into Entity, storing runtime player state in ScriptableObjects, and rewriting the architecture without a clear reason.
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