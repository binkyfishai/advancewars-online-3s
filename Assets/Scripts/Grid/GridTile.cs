using UnityEngine;

public class GridTile : MonoBehaviour
{
    [Header("Tile Properties")]
    public TileType tileType;
    public int gridX;
    public int gridZ;
    public int movementCost = 1;
    public int defenseBonus = 0;
    
    // Terrain system integration
    public Vector3 worldPosition => GetWorldPosition();
    
    [Header("Visual")]
    public Material originalMaterial;
    public Material highlightMaterial;
    public Material movementRangeMaterial;
    
    // References
    private GridManager gridManager;
    private MeshRenderer meshRenderer;
    private Collider tileCollider;
    
    // Occupancy
    public Unit occupyingUnit { get; private set; }
    public Building building { get; private set; }
    
    // Visual state
    private bool isHighlighted = false;
    private bool isInMovementRange = false;
    private bool isInAttackRange = false;
    private bool isHovered = false;
    
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        tileCollider = GetComponent<Collider>();
        
        // Store original material
        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.material;
        }
    }
    
    public void Initialize(int x, int z, TileType type, GridManager manager)
    {
        gridX = x;
        gridZ = z;
        tileType = type;
        gridManager = manager;
        
        // Set terrain properties based on type
        SetTerrainProperties();
        
        // Set name for debugging
        name = $"Tile_{x}_{z}_{type}";
    }
    
    void SetTerrainProperties()
    {
        switch (tileType)
        {
            case TileType.Plain:
                movementCost = 1;
                defenseBonus = 0;
                break;
            case TileType.Mountain:
                movementCost = 2;
                defenseBonus = 30; // 30% defense bonus
                break;
            case TileType.Forest:
                movementCost = 1;
                defenseBonus = 20; // 20% defense bonus
                break;
            case TileType.Road:
                movementCost = 1;
                defenseBonus = 0;
                break;
            case TileType.River:
                movementCost = 2;
                defenseBonus = 0;
                break;
            case TileType.Bridge:
                movementCost = 1;
                defenseBonus = 0;
                break;
            case TileType.Reef:
                movementCost = 1;
                defenseBonus = 10; // 10% defense bonus
                break;
            case TileType.Building:
                movementCost = 1;
                defenseBonus = 30; // 30% defense bonus
                break;
        }
    }
    
    // Mouse interaction
    void OnMouseEnter()
    {
        if (GameManager.Instance != null && GameManager.Instance.CanSelectTile(this))
        {
            if (gridManager != null)
                gridManager.HighlightTile(this);
        }
    }
    
    void OnMouseExit()
    {
        if (gridManager != null)
            gridManager.ClearHighlight(this);
    }
    
    // Disabled OnMouseDown to avoid conflict with InputManager
    // The InputManager handles all tile selection via OnLeftClick
    /*
    void OnMouseDown()
    {
        Debug.Log($"=== GRID TILE MOUSE DOWN: {GetGridPosition()} ===");
        Debug.Log($"Tile occupying unit: {(occupyingUnit != null ? occupyingUnit.name : "NULL")}");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectTile(this);
        }
    }
    */
    
    // Unit occupancy
    public void SetOccupyingUnit(Unit unit)
    {
        Debug.Log($"=== SET OCCUPYING UNIT ON TILE {GetGridPosition()} ===");
        Debug.Log($"Previous occupying unit: {(occupyingUnit != null ? occupyingUnit.name : "NULL")}");
        Debug.Log($"New occupying unit: {(unit != null ? unit.name : "NULL")}");
        
        occupyingUnit = unit;
        
        if (unit != null)
        {
            // Position unit on this tile
            Vector3 worldPos = gridManager.GridToWorld(new Vector2Int(gridX, gridZ));
            unit.transform.position = worldPos;
            Debug.Log($"✓ Unit {unit.name} positioned at world pos: {worldPos}");
        }
        
        Debug.Log($"✓ Final occupying unit: {(occupyingUnit != null ? occupyingUnit.name : "NULL")}");
    }
    
    public void ClearOccupyingUnit()
    {
        Debug.Log($"=== CLEAR OCCUPYING UNIT ON TILE {GetGridPosition()} ===");
        Debug.Log($"Previous occupying unit: {(occupyingUnit != null ? occupyingUnit.name : "NULL")}");
        occupyingUnit = null;
        Debug.Log($"✓ Occupying unit cleared");
    }
    
    // Building
    public void SetBuilding(Building buildingComponent)
    {
        building = buildingComponent;
    }
    
    public void ClearBuilding()
    {
        building = null;
    }
    
    // Visual highlighting
    public void SetSelected(bool selected)
    {
        if (meshRenderer == null) return;
        
        // You can add a special selection material here
        // For now, use the highlight material
        if (selected && highlightMaterial != null)
        {
            meshRenderer.material = highlightMaterial;
        }
        else if (!selected && !isHighlighted && !isInMovementRange && !isInAttackRange)
        {
            meshRenderer.material = originalMaterial;
        }
    }
    
    public void SetHighlight(bool highlight, Material material)
    {
        if (meshRenderer == null) return;
        
        isHighlighted = highlight;
        
        if (highlight && material != null)
        {
            meshRenderer.material = material;
        }
        else if (!highlight && !isInMovementRange && !isInAttackRange)
        {
            meshRenderer.material = originalMaterial;
        }
    }
    
    public void SetMovementRange(bool inRange, Material material)
    {
        if (meshRenderer == null) return;
        
        isInMovementRange = inRange;
        
        if (inRange && material != null)
        {
            meshRenderer.material = material;
        }
        else if (!inRange && !isHighlighted && !isInAttackRange)
        {
            meshRenderer.material = originalMaterial;
        }
    }
    
    public void SetAttackRange(bool inRange, Material material)
    {
        if (meshRenderer == null) return;
        
        isInAttackRange = inRange;
        
        if (inRange && material != null)
        {
            meshRenderer.material = material;
        }
        else if (!inRange && !isHighlighted && !isInMovementRange)
        {
            meshRenderer.material = originalMaterial;
        }
    }
    
    public void SetHovered(bool hovered)
    {
        if (meshRenderer == null) return;
        
        isHovered = hovered;
        
        if (hovered && highlightMaterial != null)
        {
            meshRenderer.material = highlightMaterial;
        }
        else if (!hovered && !isHighlighted && !isInMovementRange && !isInAttackRange)
        {
            meshRenderer.material = originalMaterial;
        }
    }
    
    // Utility methods
    public bool IsEmpty()
    {
        return occupyingUnit == null && building == null;
    }
    
    public bool CanMoveThrough()
    {
        // Can move through if empty or only has a building
        return occupyingUnit == null;
    }
    
    public bool CanStopOn()
    {
        // Can stop on if completely empty
        return occupyingUnit == null;
    }
    
    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(gridX, gridZ);
    }
    
    public Vector3 GetWorldPosition()
    {
        if (gridManager != null)
            return gridManager.GridToWorld(new Vector2Int(gridX, gridZ));
        
        return transform.position;
    }
    
    // Calculate movement cost for a unit
    public int GetMovementCost(Unit unit)
    {
        if (unit == null) return movementCost;
        
        // TODO: Different unit types may have different movement costs on different terrains
        // For now, return base movement cost
        return movementCost;
    }
    
    // Calculate defense bonus for a unit
    public int GetDefenseBonus(Unit unit)
    {
        if (unit == null) return defenseBonus;
        
        // TODO: Different unit types may get different defense bonuses
        // For now, return base defense bonus
        return defenseBonus;
    }
    
    // Debug info
    public override string ToString()
    {
        return $"GridTile({gridX}, {gridZ}) - {tileType} - Cost: {movementCost} - Defense: {defenseBonus}%";
    }
    
    // Gizmos for debugging
    void OnDrawGizmos()
    {
        if (gridManager == null) return;
        
        // Draw grid position
        Gizmos.color = Color.white;
        Vector3 pos = GetWorldPosition();
        Gizmos.DrawWireCube(pos, Vector3.one * 0.1f);
        
        // Draw different colors based on tile type
        switch (tileType)
        {
            case TileType.Plain:
                Gizmos.color = Color.green;
                break;
            case TileType.Mountain:
                Gizmos.color = Color.gray;
                break;
            case TileType.Forest:
                Gizmos.color = new Color(0f, 0.5f, 0f); // Dark green
                break;
            case TileType.Road:
                Gizmos.color = Color.yellow;
                break;
            case TileType.River:
                Gizmos.color = Color.blue;
                break;
            case TileType.Bridge:
                Gizmos.color = Color.cyan;
                break;
            case TileType.Reef:
                Gizmos.color = Color.magenta;
                break;
            case TileType.Building:
                Gizmos.color = Color.red;
                break;
        }
        
        Gizmos.DrawWireCube(pos + Vector3.up * 0.1f, Vector3.one * 0.05f);
    }
} 