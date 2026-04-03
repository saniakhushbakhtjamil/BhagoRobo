# CLAUDE.md — BlindRobo

This is the continuation of the `blind` project, rebuilt as a fresh **3D URP** Unity project for better atmosphere and depth. The 2D scripts are all reusable — the main work is re-setting up the scene in 3D.

## Collaboration Style
- Sania is a software engineer, new to game dev and Unity
- Keep code explanations **concise** — explain the "why", not every line
- For Unity navigation: give **step-by-step instructions** (where to click, what to look for)
- When there are multiple approaches: **give options with a recommendation**
- Pace: **understand before moving on** — don't rush ahead
- Focus on helping navigate Unity as a tool, not just writing code
- Always add `[Tooltip("...")]` to serialized fields in scripts — keeps the Inspector self-documenting

## Game Overview
A top-down survival/exploration game. A robot navigates a pitch-black room searching for batteries to survive or an exit to escape.

- The room is pitch black — the robot's flashlight is the only light source
- Flashlight radius and brightness scale with battery level (dim = low battery, bright = full)
- Battery drains continuously over time
- Picking up batteries recharges the robot
- At 0% battery: 15-second grace period (flickering light, tense atmosphere)
- Grace period ends → Game Over
- Reaching the exit → Win

## Unity Setup
- Unity version: **6000.4.0f1**
- Render pipeline: **3D URP** (fresh project — switching from 2D URP for better atmosphere)
- All scripts use `namespace Blind`
- Input System: **New Input System** (InputSystem_Actions asset) — do NOT use old `Input.GetAxis`

## Folder Structure
```
Assets/
  _Blind/                     # All project files (underscore keeps it at top)
    Art/
      Sprites/
        Player/               # Robot sprites
        Environment/          # Walls, floors, exit
        UI/                   # HUD elements
      Animations/
        Player/               # Robot animation clips
    Audio/
      Music/                  # Background tracks
      SFX/                    # Pickup sounds, game over, win
    Data/                     # ScriptableObjects (tunable config values)
    Prefabs/
      Player/                 # Robot prefab
      Pickups/                # Battery pickup prefab
      Environment/            # Walls, floor, exit prefabs
      UI/                     # HUD, game over screen prefabs
    Scenes/
      Levels/                 # Level scenes (Level_01, Level_02...)
      UI/                     # MainMenu, GameOver scenes
    Scripts/
      Core/                   # GameManager, camera follow
      Player/                 # Movement, battery, flashlight
      Pickups/                # Battery pickup logic
      Environment/            # Exit trigger
      UI/                     # HUD, game over screen
      Data/                   # ScriptableObject definitions
      Utils/                  # Shared helpers/constants
  ThirdParty/                 # Asset Store packages — never edit
  Settings/                   # URP renderer config — never edit
```

## Scripts Reference
All scripts written. Robot uses wheeled physics + mouse-aimed turret flashlight.

| Script | Location | Purpose |
|---|---|---|
| `GameManager.cs` | `Core/` | Owns game state: Playing → GracePeriod → GameOver / Win |
| `CameraFollow.cs` | `Core/` | Follows + rotates with player; offset (0,5,-5), pitch 45° |
| `BatteryConfig.cs` | `Data/` | ScriptableObject — tunable values (drain rate, grace period, pickup amount) |
| `PlayerMovement.cs` | `Player/` | Wheeled physics: momentum, turning radius, lateral friction, braking drag |
| `BatterySystem.cs` | `Player/` | Battery % drain, grace period timer, fires events |
| `FlashlightController.cs` | `Player/` | Subscribes to battery events, updates light radius + intensity |
| `FlashlightAimer.cs` | `Player/` | Rotates turret toward mouse cursor, clamped ±90° front hemisphere |
| `BatteryPickup.cs` | `Pickups/` | Recharges battery on collision; works during grace period |
| `ExitTrigger.cs` | `Environment/` | Triggers win when player reaches the exit |
| `HUDController.cs` | `UI/` | Subscribes to battery events, updates battery bar color and fill |

## Architecture
- **Event-driven**: `BatterySystem` fires events → other scripts react. No polling.
- **GameManager** is the single source of truth for game state (singleton, `GameManager.Instance`)
- **ScriptableObjects** (`Data/`) hold all tunable values — tweak without touching code
- No magic numbers in scripts — all config lives in `BatteryConfig`
- Battery drain: `Time.deltaTime / config.fullDrainDuration`
- Flickering during grace period: Perlin noise — `Mathf.PerlinNoise(Time.time * flickerSpeed, 0f)`

## 3D Scene Setup Notes
Key differences from the 2D project when setting up the scene in 3D URP:

- Use **Light2D (Spot)** for flashlight — still available in 3D URP via the 2D Renderer
  - OR use a standard 3D **Spot Light** component set to top-down angle for similar effect
- Sprites must use **Sprite-Lit-Default** material to be affected by Light2D
- Light2D blend style: **Additive** for the flashlight effect
- For room walls in 3D: use **3D cube meshes** or keep Tilemap with 3D colliders
- Camera: orthographic, looking straight down (top-down view), positioned above the scene on Y axis
- Rigidbody2D → **Rigidbody** with constraints (freeze Y position, freeze X/Z rotation)
- CapsuleCollider2D → **CapsuleCollider** (or BoxCollider)
- `OnTriggerEnter2D` → `OnTriggerEnter` in 3D scripts

## Design Rules
- Flashlight must feel tense as battery drops — not just darker, but smaller radius too
- Grace period should feel desperate: flickering light, not a safe buffer
- **Get the feel right before adding complexity**
- No enemies until core mechanics are solid
- No multiple levels until single room feels right

## Game Design (finalised this session)
- **Story**: Cute robot is making a bed on a spaceship. Unknown event kills all humans + cuts power. Robot must navigate dark rooms to reach the tech room (restore emergency power) then the control room. Game over = spaceship never reaches Jupiter.
- **Controls**: WASD move, mouse aims flashlight independently (robot can stand still and look around)
- **Camera**: Follows AND rotates with robot — screen forward always matches robot facing. Angle ~60° (between top-down and isometric). Adjust by feel in Unity.
- **Exit**: Hidden until player is within ~5-7m — dim light appears around it to guide player in
- **Enemies (post-MVP)**: "Moromogons" — drain battery on contact, don't kill directly
- **Battery spawns**: Random each run

## Robot Visual (built from primitives in-scene)
- Box body + two cylinder wheels + small cube turret on top
- Flashlight (Spot Light) mounted on turret — rotates with mouse

## Current Session Progress
- [x] Unity 3D URP project created at `My project/`
- [x] `_Blind/` folder structure created in Assets
- [x] All scripts written (see Scripts Reference)
- [x] Scene rough setup — robot, flashlight, camera, battery pickups placed
- [x] Wheeled vehicle physics (momentum, turn radius, lateral friction, braking)
- [x] Mouse-aimed flashlight turret (independent of move direction, ±90° clamp)
- [x] Battery system working — pickups work in grace period, restore to Playing
- [x] Camera follows + rotates with robot (offset 0,5,-5; pitch 45°)
- [ ] **Uncommitted changes** — `PlayerMovement.cs` + `SampleScene.unity` modified, `FlashlightAimer.cs.meta` untracked. Commit when happy with feel.
- [ ] **Next: Tune feel in Play Mode** — adjust speed, turn radius, flashlight radius/intensity vs battery, grace period flicker. Then commit.
- [ ] Game Over & Win screens (UI canvas)
- [ ] Proper room layout — walls, floor, exit trigger with proximity reveal (~5-7m)
- [ ] Create reusable prefabs (battery pickup, exit, robot)
- [ ] Juice: camera shake, particle effects, SFX, music
- [ ] Level design (multiple rooms)
- [ ] Enemies (Moromogons — drain battery on contact)

## Development Phases
- [x] Game design finalised (MVP scope locked)
- [x] 3D URP Unity project created
- [x] Folder structure set up
- [x] All scripts written
- [x] Scene rough setup (robot, flashlight, camera, battery pickups)
- [ ] **Current: Tune game feel → commit scene → UI screens**
- [ ] Proper room layout with walls and exit
- [ ] Prefabs for pickups and environment
- [ ] Juice pass (shake, particles, audio)
- [ ] Level design
- [ ] Enemies
