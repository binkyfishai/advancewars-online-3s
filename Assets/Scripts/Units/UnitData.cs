using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Data", menuName = "Advance Wars/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Basic Info")]
    public string unitName;
    public UnitType unitType;
    public UnitClass unitClass;
    
    [Header("Visual")]
    public GameObject modelPrefab;
    public Sprite icon;
    public RuntimeAnimatorController animatorController;
    
    [Header("Combat Stats")]
    public int maxHP = 100;
    public int cost = 1000;
    public int movement = 2;
    public int vision = 2;
    public int fuel = 99;
    public int ammo = 10;
    
    [Header("Attack Data")]
    public AttackData[] attackTable;
    
    [Header("Movement")]
    public TerrainMovement[] movementTable;
    public bool canMoveOnWater = false;
    public bool canMoveOnReef = false;
    public bool isAirUnit = false;
    
    [Header("Special Abilities")]
    public bool canCapture = false;
    public bool canLoad = false;
    public bool canSupply = false;
    public int loadCapacity = 0;
    public UnitType[] canLoadTypes;
    
    [Header("Audio")]
    public AudioClip moveSound;
    public AudioClip attackSound;
    public AudioClip destroySound;
    
    // Get movement cost for specific terrain
    public int GetMovementCost(TerrainType terrain)
    {
        foreach (TerrainMovement tm in movementTable)
        {
            if (tm.terrain == terrain)
                return tm.movementCost;
        }
        return 999; // Impassable if not found
    }
    
    // Check if unit can move on terrain
    public bool CanMoveOnTerrain(TerrainType terrain)
    {
        return GetMovementCost(terrain) < 999;
    }
    
    // Get attack damage against target unit
    public int GetAttackDamage(UnitType targetType)
    {
        foreach (AttackData ad in attackTable)
        {
            if (ad.targetType == targetType)
                return ad.damage;
        }
        return 0; // No damage if not found
    }
    
    // Check if unit can attack target
    public bool CanAttack(UnitType targetType)
    {
        return GetAttackDamage(targetType) > 0;
    }
    
    // Get attack range
    public Vector2Int GetAttackRange()
    {
        if (attackTable.Length > 0)
        {
            // Find min and max ranges
            int minRange = int.MaxValue;
            int maxRange = 0;
            
            foreach (AttackData ad in attackTable)
            {
                if (ad.damage > 0)
                {
                    minRange = Mathf.Min(minRange, ad.minRange);
                    maxRange = Mathf.Max(maxRange, ad.maxRange);
                }
            }
            
            return new Vector2Int(minRange == int.MaxValue ? 1 : minRange, maxRange);
        }
        
        return new Vector2Int(1, 1); // Default to adjacent attack
    }
    
    // Check if unit has indirect attack
    public bool HasIndirectAttack()
    {
        Vector2Int range = GetAttackRange();
        return range.x > 1 || range.y > 1;
    }
}

[System.Serializable]
public class AttackData
{
    public UnitType targetType;
    public int damage;
    public int minRange = 1;
    public int maxRange = 1;
    public bool usesAmmo = true;
}

[System.Serializable]
public class TerrainMovement
{
    public TerrainType terrain;
    public int movementCost;
}

public enum UnitType
{
    // Infantry
    Infantry,
    Mech,
    
    // Vehicles
    Recon,
    Tank,
    MediumTank,
    NeoTank,
    MegaTank,
    APC,
    Artillery,
    Rocket,
    AntiAir,
    Missile,
    
    // Aircraft
    Fighter,
    Bomber,
    BattleCopter,
    TransportCopter,
    
    // Naval
    Battleship,
    Cruiser,
    Submarine,
    Lander,
    
    // Special
    PipeRunner,
    Oozium
}

public enum UnitClass
{
    Infantry,
    Vehicle,
    Aircraft,
    Naval,
    Special
} 