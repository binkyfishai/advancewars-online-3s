using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class UnitModelAssignment
{
    public string unitName;
    public UnitType unitType;
    public GameObject modelPrefab; // Drag GLB model here
    [HideInInspector] public UnitData createdData; // Reference to created asset
}

public class UnitDataSetup : MonoBehaviour
{
    [Header("Automatic Unit Data Creation & Model Assignment")]
    [SerializeField] private bool autoCreateOnStart = true;
    
    [Header("Drag GLB Models Here")]
    public UnitModelAssignment[] unitModels = new UnitModelAssignment[]
    {
        new UnitModelAssignment { unitName = "Infantry", unitType = UnitType.Infantry },
        new UnitModelAssignment { unitName = "Tank", unitType = UnitType.Tank },
        new UnitModelAssignment { unitName = "Artillery", unitType = UnitType.Artillery },
        new UnitModelAssignment { unitName = "Recon", unitType = UnitType.Recon },
        new UnitModelAssignment { unitName = "AntiAir", unitType = UnitType.AntiAir },
        new UnitModelAssignment { unitName = "APC", unitType = UnitType.APC },
        new UnitModelAssignment { unitName = "MediumTank", unitType = UnitType.MediumTank },
        new UnitModelAssignment { unitName = "MegaTank", unitType = UnitType.MegaTank }
    };
    
    [Header("Created Unit Data Assets")]
    public UnitData[] createdUnitData;
    
    void Start()
    {
        if (autoCreateOnStart)
        {
            CreateAllUnitData();
        }
    }
    
    [ContextMenu("Create All Unit Data")]
    public void CreateAllUnitData()
    {
        #if UNITY_EDITOR
        // Create the Data folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
        {
            AssetDatabase.CreateFolder("Assets", "Data");
        }
        
        foreach (var unitModel in unitModels)
        {
            unitModel.createdData = CreateUnitDataAsset(unitModel);
        }
        
        // Update the created unit data array
        createdUnitData = new UnitData[unitModels.Length];
        for (int i = 0; i < unitModels.Length; i++)
        {
            createdUnitData[i] = unitModels[i].createdData;
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Created {unitModels.Length} UnitData assets in Assets/Data/");
        Debug.Log("You can now assign these to your GameManager's 'Test Unit Types' array!");
        #endif
    }
    
    [ContextMenu("Update Model Assignments")]
    public void UpdateModelAssignments()
    {
        #if UNITY_EDITOR
        foreach (var unitModel in unitModels)
        {
            if (unitModel.createdData != null && unitModel.modelPrefab != null)
            {
                unitModel.createdData.modelPrefab = unitModel.modelPrefab;
                EditorUtility.SetDirty(unitModel.createdData);
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("Updated model assignments for all unit data!");
        #endif
    }
    
    #if UNITY_EDITOR
    UnitData CreateUnitDataAsset(UnitModelAssignment unitModel)
    {
        string assetPath = $"Assets/Data/{unitModel.unitName}.asset";
        
        // Check if asset already exists
        UnitData existingData = AssetDatabase.LoadAssetAtPath<UnitData>(assetPath);
        if (existingData != null)
        {
            Debug.Log($"UnitData for {unitModel.unitName} already exists, updating...");
            UpdateExistingUnitData(existingData, unitModel);
            return existingData;
        }
        
        // Create new asset
        UnitData unitData = ScriptableObject.CreateInstance<UnitData>();
        
        // Set basic properties
        unitData.unitName = unitModel.unitName;
        unitData.unitType = unitModel.unitType;
        unitData.modelPrefab = unitModel.modelPrefab;
        
        // Set stats based on unit type
        SetUnitStats(unitData, unitModel.unitType);
        
        AssetDatabase.CreateAsset(unitData, assetPath);
        Debug.Log($"Created {unitModel.unitName} UnitData asset at {assetPath}");
        
        return unitData;
    }
    
    void UpdateExistingUnitData(UnitData unitData, UnitModelAssignment unitModel)
    {
        // Update model if provided
        if (unitModel.modelPrefab != null)
        {
            unitData.modelPrefab = unitModel.modelPrefab;
        }
        
        // Update stats (in case we changed the balance)
        SetUnitStats(unitData, unitModel.unitType);
        
        EditorUtility.SetDirty(unitData);
    }
    
    void SetUnitStats(UnitData unitData, UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.Infantry:
                unitData.maxHP = 10;
                unitData.fuel = 99;
                unitData.ammo = 99;
                unitData.movement = 3;
                unitData.cost = 1000;
                SetAttackTable(unitData, new int[] {55, 45, 12, 10, 5, 1, 1, 1});
                SetMovementTable(unitData, new int[] {1, 1, 2, 1, 2, 1, 1, 2});
                unitData.canCapture = true;
                break;
                
                         case UnitType.Tank:
                 unitData.maxHP = 10;
                 unitData.fuel = 70;
                 unitData.ammo = 9;
                 unitData.movement = 2;
                 unitData.cost = 3000;
                 SetAttackTable(unitData, new int[] {75, 55, 85, 70, 40, 10, 5, 1});
                 SetMovementTable(unitData, new int[] {2, 1, 3, 1, 3, 1, 1, 3});
                 break;
                 
             case UnitType.Artillery:
                 unitData.maxHP = 10;
                 unitData.fuel = 50;
                 unitData.ammo = 9;
                 unitData.movement = 1;
                 unitData.cost = 6000;
                 SetAttackTable(unitData, new int[] {90, 70, 80, 75, 45, 55, 65, 50});
                 SetMovementTable(unitData, new int[] {1, 1, 2, 1, 2, 1, 1, 2});
                 SetIndirectAttack(unitData, 2, 3); // Artillery has indirect attack range 2-3
                 break;
                 
             case UnitType.Recon:
                 unitData.maxHP = 10;
                 unitData.fuel = 80;
                 unitData.ammo = 99;
                 unitData.movement = 8;
                 unitData.cost = 4000;
                 SetAttackTable(unitData, new int[] {70, 35, 85, 28, 40, 12, 10, 6});
                 SetMovementTable(unitData, new int[] {2, 1, 3, 1, 3, 1, 1, 3});
                 break;
                 
             case UnitType.AntiAir:
                 unitData.maxHP = 10;
                 unitData.fuel = 60;
                 unitData.ammo = 9;
                 unitData.movement = 2;
                 unitData.cost = 8000;
                 SetAttackTable(unitData, new int[] {45, 25, 65, 55, 105, 120, 120, 15});
                 SetMovementTable(unitData, new int[] {2, 1, 3, 1, 3, 1, 1, 3});
                 break;
                 
             case UnitType.APC:
                 unitData.maxHP = 10;
                 unitData.fuel = 70;
                 unitData.ammo = 0; // No weapons
                 unitData.movement = 2;
                 unitData.cost = 5000;
                 SetAttackTable(unitData, new int[] {0, 0, 0, 0, 0, 0, 0, 0}); // Cannot attack
                 SetMovementTable(unitData, new int[] {2, 1, 3, 1, 3, 1, 1, 3});
                 unitData.canLoad = true;
                 unitData.loadCapacity = 1;
                 break;
                 
             case UnitType.MediumTank:
                 unitData.maxHP = 10;
                 unitData.fuel = 50;
                 unitData.ammo = 8;
                 unitData.movement = 1;
                 unitData.cost = 16000;
                 SetAttackTable(unitData, new int[] {95, 75, 105, 95, 75, 35, 25, 10});
                 SetMovementTable(unitData, new int[] {2, 1, 3, 1, 3, 1, 1, 3});
                 break;
                 
             case UnitType.MegaTank:
                 unitData.maxHP = 10;
                 unitData.fuel = 50;
                 unitData.ammo = 3;
                 unitData.movement = 1;
                 unitData.cost = 28000;
                 SetAttackTable(unitData, new int[] {125, 105, 125, 125, 105, 65, 55, 35});
                 SetMovementTable(unitData, new int[] {2, 1, 3, 1, 3, 1, 1, 3});
                 break;
                 
             default:
                 // Default stats
                 unitData.maxHP = 10;
                 unitData.fuel = 50;
                 unitData.ammo = 9;
                 unitData.movement = 2;
                 unitData.cost = 3000;
                 SetAttackTable(unitData, new int[] {50, 50, 50, 50, 50, 50, 50, 50});
                 SetMovementTable(unitData, new int[] {2, 1, 3, 1, 3, 1, 1, 3});
                break;
                 }
     }
     
     void SetAttackTable(UnitData unitData, int[] damageValues)
     {
         // Map damage values to common unit types
         UnitType[] unitTypes = new UnitType[]
         {
             UnitType.Infantry, UnitType.Mech, UnitType.Recon, UnitType.Tank, 
             UnitType.MediumTank, UnitType.NeoTank, UnitType.MegaTank, UnitType.APC
         };
         
         var attackList = new System.Collections.Generic.List<AttackData>();
         
         for (int i = 0; i < Mathf.Min(damageValues.Length, unitTypes.Length); i++)
         {
             if (damageValues[i] > 0)
             {
                 attackList.Add(new AttackData
                 {
                     targetType = unitTypes[i],
                     damage = damageValues[i],
                     minRange = 1,
                     maxRange = 1,
                     usesAmmo = true
                 });
             }
         }
         
         unitData.attackTable = attackList.ToArray();
     }
     
     void SetMovementTable(UnitData unitData, int[] movementCosts)
     {
         // Map movement costs to terrain types
         TerrainType[] terrainTypes = new TerrainType[]
         {
             TerrainType.Plain, TerrainType.Road, TerrainType.Forest, TerrainType.Mountain,
             TerrainType.River, TerrainType.Sea, TerrainType.Reef, TerrainType.Bridge
         };
         
         var movementList = new System.Collections.Generic.List<TerrainMovement>();
         
         for (int i = 0; i < Mathf.Min(movementCosts.Length, terrainTypes.Length); i++)
         {
             movementList.Add(new TerrainMovement
             {
                 terrain = terrainTypes[i],
                 movementCost = movementCosts[i]
             });
         }
         
         unitData.movementTable = movementList.ToArray();
     }
     
     void SetIndirectAttack(UnitData unitData, int minRange, int maxRange)
     {
         // Update all attack entries to have the specified range
         for (int i = 0; i < unitData.attackTable.Length; i++)
         {
             unitData.attackTable[i].minRange = minRange;
             unitData.attackTable[i].maxRange = maxRange;
         }
     }
     #endif
} 