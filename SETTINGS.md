# BhagoRobo Game Settings

All tunable values are accessible in the Unity Inspector. This file documents what each setting does.

---

## Player Movement (PlayerMovement.cs)

| Setting | Default | Range | What it does |
|---------|---------|-------|--------------|
| **Acceleration Force** | 18 | 5-50 | How fast the robot builds up speed when pressing W/S. Higher = punchier engine |
| **Max Speed** | 7 m/s | 3-15 | Top speed the robot can reach. Caps forward/reverse acceleration |
| **Turn Speed** | 90°/sec | 30-180 | How sharp the robot turns at full speed. Only applies when moving |
| **Lateral Friction** | 12 | 5-20 | How strongly sideways sliding is prevented. Higher = grippier wheels, no drifting |
| **Driving Drag** | 1.5 | 0.5-3 | Air resistance when coasting. Higher = stops faster naturally |
| **Braking Drag** | 4 | 2-8 | Extra drag when reversing while moving forward. Creates heavier braking feel |

---

## Camera Follow (CameraFollow.cs)

| Setting | Default | Range | What it does |
|---------|---------|-------|--------------|
| **Smooth Speed** | 8 | 2-15 | How fast camera position catches up to player. Higher = snappier, Lower = lazier |
| **Rotation Smooth Speed** | 3 | 1-8 | How fast camera rotates with player facing. Higher = snappier turn follow |
| **Offset** | (0, 5, -5) | — | Position relative to player. Y = height, Z = distance behind |
| **Rotation Delay** | 0.4 sec | 0.1-1.0 | How long robot must hold a direction before camera starts rotating |

---

## Flashlight (FlashlightController.cs)

| Setting | Default | Range | What it does |
|---------|---------|-------|--------------|
| **Max Range** | 12 m | 5-20 | Spotlight range when battery is 100% |
| **Min Range** | 2 m | 1-5 | Spotlight range when battery is 0% (before grace period) |
| **Max Intensity** | 3 | 1-5 | Light brightness at 100% battery |
| **Min Intensity** | 0.3 | 0.1-1 | Light brightness at 0% battery |

---

## Flashlight Aimer (FlashlightAimer.cs)

| Setting | Default | Range | What it does |
|---------|---------|-------|--------------|
| **Downward Tilt** | 20° | 0-45 | Angle the turret tilts down so beam hits the floor |

---

## Battery System (BatteryConfig.asset)

| Setting | Default | Range | What it does |
|---------|---------|-------|--------------|
| **Full Drain Duration** | 60 sec | 30-120 | How long until battery fully drains from 100% to 0% |
| **Pickup Restore Amount** | 0.3 (30%) | 0.1-0.5 | How much battery each pickup restores |
| **Grace Period Duration** | 15 sec | 5-30 | How long light flickers after battery hits 0% before game over |
| **Flicker Speed** | 8 | 3-15 | How fast the light flickers during grace period |
| **Exit Reveal Radius** | 6 m | 3-10 | How close you need to be before the exit light becomes visible |

---

## Camera Setup (SceneSetup.cs)

| Setting | Default | What it does |
|---------|---------|--------------|
| **Camera Position** | (0, 5, -5) | Initial world position of the camera |
| **Camera Pitch** | 45° | X rotation of camera (lean angle) |

---

## Quick Tuning Tips

- **Game feels too easy**: Lower `Full Drain Duration`, lower `Pickup Restore Amount`
- **Game feels too hard**: Raise `Full Drain Duration`, raise `Pickup Restore Amount`
- **Robot movement feels sluggish**: Raise `Acceleration Force`, lower `Driving Drag`
- **Robot movement feels twitchy**: Lower `Acceleration Force`, raise `Driving Drag`
- **Camera feels disconnected**: Raise `Smooth Speed` and `Rotation Smooth Speed`
- **Camera feels too snappy**: Lower `Smooth Speed` and `Rotation Smooth Speed`
- **Turning radius feels too tight**: Lower `Turn Speed`
- **Can't turn sharp enough**: Raise `Turn Speed`
