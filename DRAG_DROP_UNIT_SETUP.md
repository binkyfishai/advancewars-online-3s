# 🎮 Drag & Drop Unit Setup Guide

## 🚀 Super Easy Unit Creation with Your GLB Models

### Step 1: Quick Setup (30 seconds)
1. **Add QuickUnitSetup script to any GameObject** (like GameManager)
2. **Right-click the component → "Setup Unit System"**
3. This creates a **UnitDataSetup** GameObject for you

### Step 2: Drag & Drop Your Models (2 minutes)
1. **Select the UnitDataSetup GameObject** in Hierarchy
2. **In Inspector, expand "Unit Models" array**
3. **Drag GLB models** from `Assets/models/units/[FactionName]/` into the **Model Prefab** slots:

   - **Infantry** → `InfantryMesh.YourFaction.glb`
   - **Tank** → `TankMesh.YourFaction.glb` (or `MediumTankMesh`)
   - **Artillery** → `ArtilleryMesh.YourFaction.glb`
   - **Recon** → `ReconMesh.YourFaction.glb`
   - **AntiAir** → `AntiAirMesh.YourFaction.glb`
   - **APC** → `APCMesh.YourFaction.glb`
   - **MediumTank** → `MediumTankMesh.YourFaction.glb`
   - **MegaTank** → `MegaTankMesh.YourFaction.glb`

### Step 3: Create All Unit Data (5 seconds)
1. **Right-click the UnitDataSetup component**
2. Choose **"Create All Unit Data"**
3. ✅ **Done!** All UnitData assets created in `Assets/Data/`

### Step 4: Connect to GameManager (30 seconds)
1. **Select GameManager** in Hierarchy
2. **Drag the created UnitData assets** from `Assets/Data/` into **"Test Unit Types"** array
3. Or just drag the **"Created Unit Data"** array from UnitDataSetup

### Step 5: Test It! (Press Play)
- ✅ **Units spawn** with your actual GLB models
- ✅ **All stats** automatically configured (HP, movement, attack, etc.)
- ✅ **Full Advance Wars gameplay** ready to go!

## 🎯 Available Unit Types

The system comes pre-configured with stats for:

| Unit Type | Movement | Cost | Special |
|-----------|----------|------|---------|
| **Infantry** | 3 | 1,000 | Can capture |
| **Tank** | 2 | 3,000 | Good all-around |
| **Artillery** | 1 | 6,000 | Indirect attack |
| **Recon** | 8 | 4,000 | High movement |
| **AntiAir** | 2 | 8,000 | Anti-aircraft |
| **APC** | 2 | 5,000 | Transport unit |
| **MediumTank** | 1 | 16,000 | Heavy armor |
| **MegaTank** | 1 | 28,000 | Ultimate unit |

## 🔄 Update Models Later

**To change models later:**
1. **Update the Model Prefab slots** in UnitDataSetup
2. **Right-click component → "Update Model Assignments"**
3. ✅ All UnitData assets updated instantly!

## 🎨 Use Different Factions

**Want OrangeStar units? BlueMoon? Any faction?**
- Just drag models from **any faction folder**:
  - `Assets/models/units/OrangeStar/`
  - `Assets/models/units/BlueMoon/`
  - `Assets/models/units/GreenEarth/`
  - etc.

## ⚡ Super Fast Workflow

**Total time: ~3 minutes from GLB models to playable units!**

1. QuickSetup (30s)
2. Drag 8 models (2m)
3. Create data (5s)
4. Connect to GameManager (30s)
5. **Press Play = Working Advance Wars game!**

## 🛠️ Advanced Features

- **Automatic stats** based on Advance Wars balance
- **Smart model assignment** (updates existing data)
- **Multiple faction support** (just swap model prefabs)
- **All combat stats** pre-configured (attack tables, movement costs)
- **Ready for expansion** (easy to add new unit types)

Perfect for quickly prototyping with your existing GLB models! 