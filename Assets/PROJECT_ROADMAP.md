# Project Roadmap / Development Notes

Current project direction

This Unity project is becoming a reusable 2D action-platformer / metroidvania starter template while still being developed as an actual game.

The goal is not to stop development and build an over-engineered framework.

The goal is:

Build the game, but make core systems reusable enough to become a personal starter template for future metroidvania/platformer projects.

Target template level for now:

Personal reusable starter template.

Not yet:

Marketplace-ready public template.

Current Unity / tech context

Unity version: 6000.3.8f1
Render pipeline: Built-in Render Pipeline
C# language version: C# 9.0
Target framework: net471
Game type: 2D metroidvania/action-platformer

What was done recently

1. Ground detection improvement

Ground detection was improved from a very simple ground check into a more stable BoxCast-based system.

Current direction:

Use a dedicated GroundCheck transform.
Use BoxCast for better ground detection.
Avoid relying on a single raycast from the center.
Better support for platform edges and more stable grounded checks.

Important note:

GroundCheck should be a separate child object under Player/Enemy.
Do not reuse BottomWallCheck as GroundCheck.
BottomWallCheck is for wall detection.
GroundCheck is for ground detection.

Recommended Player values:

Ground Check Distance = 0.08 - 0.15
Ground Check Size = X 0.65 - 0.8, Y 0.08 - 0.1

For debugging, temporarily use:

Ground Check Distance = 0.2 - 0.3
Ground Check Size = X 0.8 - 1.0, Y 0.1 - 0.15

2. Coyote time added

Player now has coyote time support.

Purpose:

Allow the player to jump shortly after walking off a platform.

Current behavior:

While grounded, coyote timer is refreshed.
After leaving ground, timer counts down.
Fall state can consume jump buffer if coyote timer is active.

Recommended value:

Coyote Time = 0.08 - 0.12

Current approximate value:

Coyote Time = 0.1

3. Jump buffer already exists

Player has jump buffer logic.

Purpose:

If player presses jump slightly before landing, jump triggers when possible.

Current approximate value:

Jump Buffer Time = 0.1

Jump buffer and coyote time work together.

4. Collision detection extracted

Collision logic was moved out of Entity into:

Entity_Collision.cs

Current responsibility of Entity_Collision:

Ground detection.
Wall detection.
Wall slide detection support.
Debug gizmos for collision sensors.

Entity now exposes proxy properties:

groundDetected
wallDetected
canWallSlide

These are still used by states, but their values come from Entity_Collision.

Important setup:

Entity_Collision must be on the same GameObject as Player/Enemy, because Entity uses GetComponent<Entity_Collision>().
It should not be placed only on a child object.
GroundCheck, TopWallCheck, BottomWallCheck, and WhatIsGround must be assigned in Inspector.

5. Enemy ground ahead check was kept separate

Enemy has its own ground-ahead detection for patrol edge checks.

Current enemy-specific fields include:

Ground Ahead Check
Ground Ahead Check Distance
What Is Ground Ahead

Purpose:

Prevent enemy from walking off platforms.
Allow enemy to flip on ledges.

Important note:

What Is Ground Ahead is currently also used as an obstacle layer for enemy player detection raycast.
This works, but naming could be improved later.

Possible future improvement:

whatIsGroundAhead
whatBlocksPlayerDetection

as separate LayerMasks.

6. PlayerInputReader added

Input was moved toward a cleaner architecture via:

PlayerInputReader.cs

Current responsibility:

Read raw Unity input.
Expose simple input values:

moveInput
jumpPressed
jumpHeld
attackPressed
dashPressed

States should not directly use:

input.Player.Jump.WasPressedThisFrame()
input.Player.Attack.WasPressedThisFrame()
input.Player.Dash.WasPressedThisFrame()

Instead, states should use:

player.inputReader.jumpPressed
player.inputReader.jumpHeld
player.inputReader.attackPressed
player.inputReader.dashPressed

Player keeps compatibility property:

public Vector2 moveInput => inputReader.moveInput;

Reason:

Existing states can still use player.moveInput.
The value always reflects the current value from PlayerInputReader.
Do not use a copied field like public Vector2 moveInput = inputReader.moveInput.

7. Cleanup / safety improvements

Some cleanup was done:

Safer knockback checks.
Safer health/knockback interaction.
Combat detection avoids duplicate overlap calls.
Enemy player detection raycast reduced to one result per frame.
Enemy collision debug spam removed.
Basic attack combo length now depends on attackVelocity.Length.

Current architecture snapshot

Current core structure:

Entity:
common Rigidbody2D / Animator / state machine access
facing / flip
proxy collision properties

Entity_Collision:
ground / wall sensors
collision gizmos

StateMachine
EntityState
PlayerState
EnemyState

Player:
owns player states
owns input reader
owns movement helper
jump buffer / coyote timers
player-specific stats/settings for now

PlayerInputReader:
reads Unity Input System values
exposes simplified input state

PlayerMovement:
jump
jump cut
air movement
apex movement
fall gravity

Entity_Combat
Entity_Health
Entity_Knockback
Entity_VFX
IDamagable

Enemy:
simple patrol/battle/attack logic
ground ahead check
player detection

Current project quality assessment

Current project status:

Good gameplay prototype foundation.
Good base for a personal metroidvania/action-platformer starter.
Not yet a full reusable production template.

Approximate readiness:

For current single-player metroidvania: 8/10
For personal reusable starter template: 6.5/10
For two-player platformer template: 5.5-6/10
For public/marketplace template: not yet

Work estimate toward template level

Recent work included important foundation:

Better ground detection.
Coyote time.
Collision extraction.
Input reader.
Safety cleanup.

This is roughly:

20-25% of the path from prototype to personal reusable template.

But in terms of foundation importance, it covered about:

35-40% of the most important architectural foundation.

Remaining work to personal starter template level:

Approximately 2-3 more sessions of similar size.

Remaining work to serious reusable framework level:

Approximately 5-8 more sessions of similar size.

Remaining work to public/marketplace-ready template:

10+ sessions and documentation/polish work.

Recommended next steps

Priority 1: ScriptableObject configs

Move player tuning values out of Player into data assets.

Create something like:

PlayerMovementData
PlayerCombatData
EnemyData

Player movement config should include:

moveSpeed
jumpForce
jumpBufferTime
coyoteTime
jumpCutMultiplier
jumpCutMinVelocity
wallJumpForce
airMoveMultiplier
airMoveDeceleration
apexThreshold
apexMoveMultiplier
fallGravityMultiplier
maxFallSpeed
wallSlideSlowMultiplier
dashSpeed
dashDuration

Combat config could include:

attackVelocity
attackVelocityDuration
comboResetTime
jumpAttackVelocity

Purpose:

Make the project more reusable.
Avoid rewriting code for new characters/projects.
Tune gameplay through assets instead of changing code.

Priority 2: Input lock / input interface

Current PlayerInputReader is a class, but not yet interface-based.

Future idea:

public interface IPlayerInputReader
{
Vector2 moveInput { get; }
bool jumpPressed { get; }
bool jumpHeld { get; }
bool attackPressed { get; }
bool dashPressed { get; }

    void Enable();
    void Disable();
    void ReadInput();
}

Possible future readers:

UnityPlayerInputReader
AIInputReader
ReplayInputReader
DisabledInputReader

Input lock support needed for:

cutscenes
death
knockback
dialogue
menus
ability locks

Priority 3: StateMachine safety

Improve StateMachine.

Potential improvements:

null check when changing state
ignore transition to same state if not needed
OnStateChanged event
optional current state debug name
safer shutdown/death behavior

Example goals:

Do not crash if newState is null.
Do not call Exit/Enter if changing to current state unless forced.

Priority 4: Ability flags / unlocks

Metroidvania needs ability gating.

Create a simple system for:

canDash
canWallSlide
canWallJump
canAirAttack
canDoubleJump

Could start as simple booleans on Player or separate component:

PlayerAbilities

Purpose:

Upgrades.
Ability unlocks.
Template flexibility.
Easier two-player variation.

Priority 5: Multiplayer readiness

Do not implement full two-player mode yet, but avoid blocking it.

Needed eventually:

playerId / ownerId
per-player input source
non-static or payload-based player death events
enemy target selection for multiple players
camera support for multiple targets

Current issue:

public static event Action OnPlayerDeath;

This is okay for one player, but not enough for two players.

Future direction:

public static event Action<Player> OnAnyPlayerDeath;

or instance-based:

public event Action<Player> OnDeath;

Suggested immediate next session

Recommended next work:

Start ScriptableObject configs.

First target:

PlayerMovementData.cs

Move movement-related fields from Player into a ScriptableObject.

Then update states to use:

player.movementData.jumpForce
player.movementData.coyoteTime
player.movementData.dashSpeed

or expose proxy properties if we want a softer transition.

Soft transition recommended:

Add PlayerMovementData.
Assign it to Player.
Keep player properties if needed as proxies.
Move gradually, not all at once.

Development philosophy

Important:

Do not over-abstract too early.
Do not pause game development just to build a framework.
Do build every core system in a reusable way.

Recommended strategy:

Game-first, template-aware.

Meaning:

Keep building the current metroidvania.
When a system becomes core/reusable, extract it.
Avoid making interfaces before there are real use cases.
Prefer simple components and ScriptableObject configs over deep inheritance.

Current mental checkpoint

At the end of the last session:

Player works.
Enemy works.
Entity_Collision works.
PlayerInputReader works.
Ground detection works after creating a dedicated GroundCheck child.
Coyote time works.
Jump buffer works.
Input refactor works.
Latest suggested next step: ScriptableObject configs.

If continuing from here, first inspect:

Player.cs
PlayerInputReader.cs
Entity_Collision.cs
StateMachine.cs

Then continue with:

PlayerMovementData ScriptableObject.