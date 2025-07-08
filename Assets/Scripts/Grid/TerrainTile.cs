using UnityEngine;

[System.Serializable]
public class TerrainTile : MonoBehaviour
{
    [Header("Terrain Properties")]
    public TerrainType terrainType;
    public int defenseBonus = 0;
    public int movementCost = 1;
    public bool blockMovement = false;
    public bool blockLineOfSight = false;
    
    [Header("Visual")]
    public GameObject terrainModel;
    public Material terrainMaterial;
    
    [Header("Audio")]
    public AudioClip stepSound;
    
    // Components
    private Collider terrainCollider;
    private MeshRenderer meshRenderer;
    
    void Awake()
    {
        terrainCollider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
        
        // Initialize terrain properties based on type
        InitializeTerrainProperties();
    }
    
    void InitializeTerrainProperties()
    {
        switch (terrainType)
        {
            case TerrainType.Plain:
                defenseBonus = 1;
                movementCost = 1;
                blockMovement = false;
                blockLineOfSight = false;
                break;
                
            case TerrainType.Forest:
                defenseBonus = 2;
                movementCost = 2;
                blockMovement = false;
                blockLineOfSight = true;
                break;
                
            case TerrainType.Mountain:
                defenseBonus = 4;
                movementCost = 3;
                blockMovement = false;
                blockLineOfSight = true;
                break;
                
            case TerrainType.River:
                defenseBonus = 0;
                movementCost = 2;
                blockMovement = false;
                blockLineOfSight = false;
                break;
                
            case TerrainType.Sea:
                defenseBonus = 0;
                movementCost = 1;
                blockMovement = true; // Only naval units can move here
                blockLineOfSight = false;
                break;
                
            case TerrainType.Road:
                defenseBonus = 0;
                movementCost = 1;
                blockMovement = false;
                blockLineOfSight = false;
                break;
                
            case TerrainType.Bridge:
                defenseBonus = 0;
                movementCost = 1;
                blockMovement = false;
                blockLineOfSight = false;
                break;
                
            case TerrainType.Reef:
                defenseBonus = 1;
                movementCost = 2;
                blockMovement = false;
                blockLineOfSight = false;
                break;
                
            case TerrainType.Pipe:
                defenseBonus = 0;
                movementCost = 1;
                blockMovement = false;
                blockLineOfSight = false;
                break;
                
            case TerrainType.Waterfall:
                defenseBonus = 0;
                movementCost = 3;
                blockMovement = true;
                blockLineOfSight = true;
                break;
        }
    }
    
    public bool CanUnitEnter(Unit unit)
    {
        if (blockMovement)
        {
            // Check if unit can move on this terrain
            switch (terrainType)
            {
                case TerrainType.Sea:
                    return unit.unitData.canMoveOnWater;
                case TerrainType.Waterfall:
                    return false; // No unit can move on waterfall
                default:
                    return true;
            }
        }
        
        return true;
    }
    
    public int GetMovementCost(Unit unit)
    {
        // Some units have different movement costs on different terrain
        switch (unit.unitData.unitType)
        {
            case UnitType.Infantry:
            case UnitType.Mech:
                // Infantry moves normally on most terrain
                return movementCost;
                
            case UnitType.Recon:
            case UnitType.Tank:
            case UnitType.MediumTank:
            case UnitType.NeoTank:
            case UnitType.MegaTank:
                // Vehicles have different costs
                switch (terrainType)
                {
                    case TerrainType.Forest:
                        return 3;
                    case TerrainType.Mountain:
                        return 99; // Effectively impassable
                    default:
                        return movementCost;
                }
                
            case UnitType.Artillery:
            case UnitType.AntiAir:
            case UnitType.Missile:
                // Artillery units
                switch (terrainType)
                {
                    case TerrainType.Forest:
                        return 2;
                    case TerrainType.Mountain:
                        return 99; // Effectively impassable
                    default:
                        return movementCost;
                }
                
            default:
                return movementCost;
        }
    }
    
    public int GetDefenseBonus(Unit unit)
    {
        // Some units get different defense bonuses
        switch (unit.unitData.unitType)
        {
            case UnitType.Infantry:
            case UnitType.Mech:
                // Infantry gets full defense bonus
                return defenseBonus;
                
            default:
                // Other units get reduced defense bonus
                return Mathf.Max(0, defenseBonus - 1);
        }
    }
    
    public void SetTerrainModel(GameObject model)
    {
        if (terrainModel != null)
        {
            DestroyImmediate(terrainModel);
        }
        
        terrainModel = model;
        
        if (terrainModel != null)
        {
            terrainModel.transform.SetParent(transform);
            terrainModel.transform.localPosition = Vector3.zero;
            terrainModel.transform.localRotation = Quaternion.identity;
            
            // Update collider if needed
            if (terrainCollider == null)
            {
                terrainCollider = GetComponent<Collider>();
                if (terrainCollider == null)
                {
                    // Add a collider if one doesn't exist
                    terrainCollider = gameObject.AddComponent<BoxCollider>();
                }
            }
        }
    }
    
    public void SetTerrainType(TerrainType type)
    {
        terrainType = type;
        InitializeTerrainProperties();
    }
    
    public void PlayStepSound()
    {
        if (stepSound != null)
        {
            AudioSource.PlayClipAtPoint(stepSound, transform.position);
        }
    }
    
    public string GetTerrainName()
    {
        return terrainType.ToString();
    }
    
    public string GetTerrainDescription()
    {
        return $"{GetTerrainName()}\nDefense: +{defenseBonus}\nMovement Cost: {movementCost}";
    }
}

[System.Serializable]
public enum TerrainType
{
    Plain,
    Forest,
    Mountain,
    River,
    Sea,
    Road,
    Bridge,
    Reef,
    Pipe,
    Waterfall,
    Shore
} 