# Unity Unit System Setup Guide

## 📋 Overview
This guide will get your units working with movement, selection, and basic combat.

## Step 1: Create Unit Data (ScriptableObjects)

### In Unity Editor:

1. **Right-click in Assets/Data folder → Create → Unit Data**
2. **Create these 4 basic units:**

#### Infantry.asset
- **Unit Type**: Infantry
- **Max HP**: 10
- **Max Fuel**: 99
- **Max Ammo**: 99 
- **Movement**: 3
- **Cost**: 1000
- **Attack Damage Array**: [55, 45, 12, 10, 5, 1, 1, 1]
- **Movement Cost Array**: [1, 1, 2, 1, 2, 1, 1, 2]

#### Tank.asset
- **Unit Type**: Tank
- **Max HP**: 10
- **Max Fuel**: 70
- **Max Ammo**: 9
- **Movement**: 2
- **Cost**: 3000
- **Attack Damage Array**: [75, 55, 85, 70, 40, 10, 5, 1]
- **Movement Cost Array**: [2, 1, 3, 1, 3, 1, 1, 3]

#### Artillery.asset
- **Unit Type**: Artillery
- **Max HP**: 10
- **Max Fuel**: 50
- **Max Ammo**: 9
- **Movement**: 1
- **Cost**: 6000
- **Attack Damage Array**: [90, 70, 80, 75, 45, 55, 65, 50]
- **Movement Cost Array**: [1, 1, 2, 1, 2, 1, 1, 2]

#### Recon.asset
- **Unit Type**: Recon
- **Max HP**: 10
- **Max Fuel**: 80
- **Max Ammo**: 99
- **Movement**: 8
- **Cost**: 4000
- **Attack Damage Array**: [70, 35, 85, 28, 40, 12, 10, 6]
- **Movement Cost Array**: [2, 1, 3, 1, 3, 1, 1, 3]

## Step 2: Create Unit Prefabs

### Option A: Basic Cubes (Quick Testing)

1. **Create Infantry Prefab:**
   - Hierarchy → Right-click → 3D Object → Cube
   - Name: "InfantryUnit"
   - Scale: (0.8, 1.2, 0.8)
   - Add **Unit** component
   - Add **Collider** (for selection)
   - Set Layer to "Units" (Layer 9)
   - Make it blue colored (create blue material)
   - Drag to Assets/Prefabs/Units/ folder

2. **Create Tank Prefab:**
   - Create Cube, name "TankUnit"
   - Scale: (1.2, 0.8, 1.8) 
   - Add **Unit** component
   - Set Layer to "Units" (Layer 9)
   - Make it dark green colored
   - Drag to Prefabs folder

3. **Create Artillery Prefab:**
   - Create Cube, name "ArtilleryUnit"
   - Scale: (1.0, 1.0, 1.5)
   - Add **Unit** component  
   - Set Layer to "Units" (Layer 9)
   - Make it brown colored
   - Drag to Prefabs folder

4. **Create Recon Prefab:**
   - Create Cube, name "ReconUnit"
   - Scale: (1.0, 0.7, 1.3)
   - Add **Unit** component
   - Set Layer to "Units" (Layer 9) 
   - Make it yellow colored
   - Drag to Prefabs folder

### Option B: Using Your GLB Models

1. **Locate your faction models** in Assets/models/units/[FactionName]/
2. **For each GLB model:**
   - Drag GLB into scene
   - Add **Unit** component
   - Add **Collider** (Box Collider works fine)
   - Set Layer to "Units" (Layer 9)
   - Position at (0,0,0) and scale appropriately
   - Drag to Assets/Prefabs/Units/ folder
   - Delete from scene

## Step 3: Setup Layers

1. **Edit → Project Settings → Tags and Layers**
2. **Set Layer 9 to "Units"**

## Step 4: Configure Unit Manager

1. **Select UnitManager in Hierarchy**
2. **In Inspector, expand "Unit Prefabs" array**
3. **Set Size to 4** (or however many unit types you have)
4. **Drag your unit prefabs into the slots:**
   - Element 0: Infantry prefab
   - Element 1: Tank prefab  
   - Element 2: Artillery prefab
   - Element 3: Recon prefab

## Step 5: Test Unit Spawning

### Add Test Units to Scene:

1. **Select GameManager in Hierarchy**
2. **Add this simple test script temporarily:**

```csharp
// Add to GameManager.cs in Start() method:
void Start()
{
    // ... existing code ...
    
    // Test spawn some units
    StartCoroutine(TestSpawnUnits());
}

IEnumerator TestSpawnUnits()
{
    yield return new WaitForSeconds(1f); // Wait for grid to initialize
    
    // Spawn test units
    if (unitManager != null && gridManager != null)
    {
        // Spawn Infantry at (2,2)
        GridTile tile1 = gridManager.GetTile(2, 2);
        if (tile1 != null)
        {
            unitManager.CreateUnit(0, tile1, 0); // Infantry, player 0
        }
        
        // Spawn Tank at (5,5)  
        GridTile tile2 = gridManager.GetTile(5, 5);
        if (tile2 != null)
        {
            unitManager.CreateUnit(1, tile2, 0); // Tank, player 0
        }
        
        Debug.Log("Test units spawned!");
    }
}
```

## Step 6: Test Basic Functionality

**Press Play and test:**

1. ✅ **Units appear** on the grid at specified positions
2. ✅ **Click units** - they should get selected (highlight)
3. ✅ **Movement range** should show when unit selected
4. ✅ **Click empty tiles** to move selected unit
5. ✅ **Camera** still works (orbit, zoom, pan)

## Expected Behavior

- **Unit spawns** on grid tiles properly positioned
- **Unit selection** highlights the unit
- **Movement range** shows blue highlighted tiles
- **Click to move** unit smoothly animates to new position
- **Turn system** works (units can move once per turn)

## Troubleshooting

### Units don't appear:
- Check UnitManager has prefabs assigned
- Verify GridManager is initialized first
- Make sure tile positions are valid (within grid bounds)

### Units don't select:
- Check units have Collider components
- Verify units are on "Units" layer (Layer 9)
- Make sure InputManager is active

### Units don't move:
- Check movement range calculation
- Verify tiles are walkable
- Make sure unit has movement points and fuel

### Units appear at wrong position:
- Check GridManager.GridToWorld() conversion
- Verify cell size matches your grid (you said size 3)
- Make sure unit pivot is at center/bottom

## Next Steps

Once basic units work:

1. **Configure unit stats** properly in UnitData assets
2. **Implement combat** (attack between units)
3. **Add player switching** (multiple factions)
4. **Replace cube prefabs** with your actual GLB models
5. **Add unit animations** (movement, attack, idle)

## Quick Test Commands

Add these to GameManager for testing:

- **Spacebar**: End turn 
- **R**: Reset unit turns
- **T**: Spawn test unit at cursor position

The unit system should now be fully functional! 