# Building a Retro 80s Spaceship Bedroom in Blender

Goal: Long bedroom corridor with retro-futuristic 80s vibe. Dark walls, glowing panels, pipes, vents, windows to space.

---

## Room Layout & Dimensions

- **Length**: 20 units
- **Width**: 6 units
- **Height**: 3 units
- Robot spawn point: center of one end

```
[Window/Door] ← ENTRY

  [Bunk L]    [Bunk R]
    ↓           ↓
[Vent]  [Panel] [Vent]

    [Pipes running along top]

              [Window to space]

    EXIT →
```

---

## Build Process (Step-by-Step)

### 1. Base Room (Walls, Floor, Ceiling)

1. **Delete the default cube** (X → Delete)
2. **Add Floor**:
   - `Shift+A` → Mesh → Cube
   - Scale: `S` → `Z 0.1` (thin floor)
   - Scale XY: `S Shift+Z` → type `10` (make it 20×6)
   - Move down: `G Z` → `-0.5`
   - Name it "Floor"

3. **Add Walls** (4 walls):
   - **Front Wall** (entry):
     - `Shift+A` → Mesh → Cube
     - Scale: `SXY` to (6, 1.5, 3) — tall, thin
     - Position: `G Y` → `-10`
     - Name: "Wall_Front"

   - **Back Wall** (exit):
     - Duplicate front wall: `Shift+D`
     - Move: `G Y` → `10`
     - Name: "Wall_Back"

   - **Left Wall**:
     - Add cube, scale to (0.5, 20, 3)
     - Move: `G X` → `-3`
     - Name: "Wall_Left"

   - **Right Wall**:
     - Duplicate left, move: `G X` → `3`
     - Name: "Wall_Right"

4. **Add Ceiling**:
   - Duplicate floor: `Shift+D`
   - Move up: `G Z` → `3`
   - Name: "Ceiling"

---

### 2. Spaceship Details

#### Bunks (left and right sides)

- **Left Bunk**:
  - Add cube, scale to (2.5, 1.2, 0.5)
  - Position: `G X -2, Y -3, Z 1`
  - Name: "Bunk_L"

- **Right Bunk** (duplicate):
  - Duplicate, move: `G X 2`
  - Name: "Bunk_R"

#### Pipes (along the top)

- **Pipe 1** (left side):
  - Add cylinder: `Shift+A` → Mesh → Cylinder
  - Rotate to horizontal: `R Y 90`
  - Scale: `S X 10, S Y 0.15, S Z 0.15` (long, thin)
  - Position: `G X -1, Z 2.7`
  - Name: "Pipe_L"

- **Pipe 2** (right side):
  - Duplicate, move: `G X 2`
  - Name: "Pipe_R"

#### Vents (small square grilles)

- **Vent 1** (left wall):
  - Add cube, scale to (0.3, 0.8, 0.5)
  - Position: `G X -3.2, Y -5, Z 1.5`
  - Name: "Vent_L1"

- **Vent 2** (right wall):
  - Duplicate, move: `G X 6.4`
  - Name: "Vent_R1"

#### Control Panels (glowing rectangles on walls)

- **Panel 1** (left wall, mid-room):
  - Add plane: `Shift+A` → Mesh → Plane
  - Scale: `S 0.6 1` (1.2 × 1.2)
  - Position: `G X -3.2, Y 2, Z 1.2`
  - Name: "Panel_L"

- **Panel 2** (right wall):
  - Duplicate, move: `G X 6.4`
  - Name: "Panel_R"

#### Windows (to space outside)

- **Window 1** (long window on right side):
  - Add plane, scale to (0.3, 5, 1.5)
  - Position: `G X 3.2, Y 0, Z 1.5`
  - Name: "Window_Space"

- **Window 2** (small window near exit):
  - Add plane, scale to (0.3, 1.2, 0.8)
  - Position: `G X -3.2, Y 8, Z 1.5`
  - Name: "Window_Exit"

---

### 3. Materials & Colors (Shading)

Set up the spaceship vibe:

1. **Dark walls** — material: dark gray (#222222)
2. **Glowing panels** — material: emit bright cyan/orange (#00FFFF or #FF8800)
3. **Pipes** — material: metallic gray
4. **Windows** — material: emit dark blue (#001155) to simulate starfield

---

### 4. Lighting

- **Ambient light** (very low, dark room)
- **Key light**: one area light (soft, blue-tinted) from the window
- **Panel glow**: small emissive plane lights on control panels

---

### 5. Export as FBX

1. **Select all objects** (`A`)
2. **File → Export → FBX (.fbx)**
3. Name: `SpaceshipBedroom.fbx`
4. Save to: `Assets/_Blind/Art/Models/`

---

## Importing into Unity

1. **Drag SpaceshipBedroom.fbx** into `Assets/_Blind/Art/Models/`
2. In Inspector, set:
   - Model → Geometry → Bake Axis Conversion: OFF
   - Materials → Location: Embed in FBX
3. **Prefab the model**: Drag into `Assets/_Blind/Prefabs/Environment/`

---

## Updating SceneSetup.cs

Replace the `CreateRoom()` function to spawn the Blender model instead of primitives:

```csharp
static GameObject CreateRoom()
{
    var roomPrefab = Resources.Load<GameObject>("Models/SpaceshipBedroom");
    return Instantiate(roomPrefab);
}
```

Then add colliders to the walls in the model.

---

## Texturing (Optional, for Polish)

- **Walls**: dark metal texture with slight wear
- **Panels**: cyan/orange glow with grid pattern
- **Pipes**: brushed steel
- **Floor**: industrial grating or metal plate

---

## Next: Materials & Glow

Once the geometry is built, we'll:
1. Add cyan/orange **glowing panels** that flicker subtly
2. Add **warning lights** that pulse
3. Create **starfield backdrop** for the windows
4. Add **ambient hum sound** effect

---

## Blender Shortcuts Cheat Sheet

| Action | Shortcut |
|--------|----------|
| Add object | `Shift+A` |
| Duplicate | `Shift+D` |
| Scale | `S` (then X/Y/Z for axis) |
| Rotate | `R` (then X/Y/Z for axis) |
| Move | `G` (then X/Y/Z for axis) |
| Toggle Edit/Object | `Tab` |
| Select All | `A` |
| Deselect All | `Alt+A` |
| Delete | `X` → Delete |
| Frame Selected | Numpad `.` |
