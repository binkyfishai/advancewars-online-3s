# Unity Advance Wars Setup Guide

## Initial Setup Steps

### 1. Open Unity Project
1. Open Unity Hub
2. Click "Open" and select the `AWO` folder
3. Unity should load the project with SampleScene open

### 2. Create Basic Tile Prefabs

**First, create basic cube prefabs for testing:**

1. **Create Plain Tile Prefab:**
   - In the Hierarchy, right-click → 3D Object → Cube
   - Name it "PlainTile"
   - Set Transform Scale to (0.9, 0.2, 0.9)
   - In Inspector, add component "Grid Tile"
   - Set the layer to "Tiles" (Layer 8)
   - Drag from Hierarchy to Assets/Prefabs folder to create prefab
   - Delete the cube from Hierarchy

2. **Create Mountain Tile Prefab:**
   - Create another Cube, name it "MountainTile"
   - Set Transform Scale to (0.9, 0.5, 0.9)
   - Add "Grid Tile" component
   - Set layer to "Tiles" (Layer 8)
   - Make it gray colored (create a gray material)
   - Drag to Assets/Prefabs folder to create prefab
   - Delete from Hierarchy

### 3. Create Materials

**Create these materials in Assets/Materials:**

1. **Highlight Material:**
   - Right-click in Materials folder → Create → Material
   - Name it "HighlightMaterial"
   - Set Albedo color to bright yellow
   - Enable Emission and set emission color to yellow

2. **Movement Range Material:**
   - Create → Material, name "MovementRangeMaterial"
   - Set Albedo color to light blue
   - Enable Emission and set emission color to blue

3. **Attack Range Material:**
   - Create → Material, name "AttackRangeMaterial"
   - Set Albedo color to light red
   - Enable Emission and set emission color to red

### 4. Setup Layer

1. Go to Edit → Project Settings → Tags and Layers
2. Make sure Layer 8 is named "Tiles"
3. If not, click on "User Layer 8" and type "Tiles"

### 5. Configure Grid Manager

1. Select "GridManager" in the Hierarchy
2. In the Inspector, drag your prefabs to the corresponding slots:
   - **Plain Tile Prefab**: Drag PlainTile prefab
   - **Mountain Tile Prefab**: Drag MountainTile prefab
   - **Highlight Material**: Drag HighlightMaterial
   - **Movement Range Material**: Drag MovementRangeMaterial
   - **Attack Range Material**: Drag AttackRangeMaterial

### 6. Test Basic Functionality

**Press Play to test:**

1. **Grid Generation:** A 20x20 grid should appear (green plains with some gray mountains)
2. **Camera Controls:**
   - Right-click + drag: Orbit around
   - Scroll wheel: Zoom in/out
   - Middle-click + drag: Pan camera
3. **Tile Selection:**
   - Left-click on tiles: Should highlight them
   - Hover over tiles: Should show hover effect

## Expected Behavior

- **Grid appears:** 20x20 tiles with mostly green plains and some gray mountains
- **Camera moves smoothly** with mouse controls
- **Tiles highlight** when clicked
- **Console shows** initialization messages

## Troubleshooting

### Grid doesn't appear:
- Check that GridManager has tile prefabs assigned
- Make sure prefabs have GridTile component
- Check that GridManager.InitializeGrid() is being called

### Camera doesn't move:
- Make sure Main Camera has CameraController script
- Check that the camera is not restricted by colliders

### Tiles don't highlight:
- Verify materials are assigned to GridManager
- Check that tiles have Collider components
- Make sure tiles are on "Tiles" layer (Layer 8)

### Console errors:
- Check that all script references are properly assigned
- Verify scene file doesn't have duplicate identifiers

## Next Steps

Once basic functionality works:

1. **Import your GLB models** to replace cube prefabs
2. **Create unit prefabs** using your unit GLB files
3. **Set up building prefabs** using building GLB files
4. **Configure UnitData ScriptableObjects** for different unit types
5. **Test unit placement and movement**

## Controls Summary

- **Left Click**: Select tile/unit
- **Right Click**: Cancel selection
- **Mouse Hover**: Highlight tiles
- **Right-Click + Drag**: Orbit camera
- **Scroll Wheel**: Zoom camera
- **Middle-Click + Drag**: Pan camera
- **Spacebar**: End turn (when implemented)

The game framework is now complete and ready for content! 