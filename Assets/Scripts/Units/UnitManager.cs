using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [Header("Unit Prefabs")]
    public GameObject unitPrefab; // Base unit prefab
    
    [Header("Starting Units")]
    public List<StartingUnit> startingUnits = new List<StartingUnit>();
    
    // All units in the game
    private List<Unit> allUnits = new List<Unit>();
    
    // Singleton
    public static UnitManager Instance { get; private set; }
    
    // References
    private GridManager gridManager;
    private GameManager gameManager;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        gridManager = GridManager.Instance;
        gameManager = GameManager.Instance;
    }
    
    public void InitializeUnits()
    {
        // Create starting units
        foreach (StartingUnit startingUnit in startingUnits)
        {
            CreateUnit(startingUnit.unitData, startingUnit.owner, startingUnit.gridPosition);
        }
        
        Debug.Log($"Initialized {allUnits.Count} units");
    }
    
    public Unit CreateUnit(UnitData unitData, int owner, Vector2Int gridPosition)
    {
        if (unitData == null)
        {
            Debug.LogError("Cannot create unit: UnitData is null");
            return null;
        }
        
        if (gridManager == null)
        {
            Debug.LogError("Cannot create unit: GridManager not found");
            return null;
        }
        
        // Check if position is valid and empty
        GridTile tile = gridManager.GetTile(gridPosition);
        if (tile == null)
        {
            Debug.LogError($"Cannot create unit: Invalid grid position {gridPosition}");
            return null;
        }
        
        if (tile.occupyingUnit != null)
        {
            Debug.LogError($"Cannot create unit: Tile {gridPosition} is already occupied");
            return null;
        }
        
                // Create unit GameObject
        GameObject unitObj;
        Debug.Log($"Creating unit GameObject...");
        Debug.Log($"UnitPrefab: {(unitPrefab != null ? unitPrefab.name : "NULL")}");
        
        if (unitPrefab != null)
        {
            unitObj = Instantiate(unitPrefab, transform);
            Debug.Log($"✓ Instantiated from prefab: {unitObj.name}");
        }
        else
        {
            unitObj = new GameObject($"Unit_{unitData.unitName}_{owner}");
            unitObj.transform.SetParent(transform);
            Debug.Log($"✓ Created new GameObject: {unitObj.name}");
        }

        Debug.Log($"Checking for existing Unit component...");
        
        // Add Unit component
        Unit unit = unitObj.GetComponent<Unit>();
        if (unit == null)
        {
            Debug.Log($"No Unit component found - adding one...");
            unit = unitObj.AddComponent<Unit>();
            Debug.Log($"✓ Unit component added: {(unit != null ? "SUCCESS" : "FAILED")}");
        }
        else
        {
            Debug.Log($"✓ Unit component already exists");
        }

                Debug.Log($"Final unit object: {unit.name} with Unit component: {(unit != null ? "YES" : "NO")}");

        // Configure unit
        Debug.Log($"Configuring unit...");
        unit.unitData = unitData;
        unit.owner = owner;
        Debug.Log($"Set unitData: {unitData.unitName}, owner: {owner}");
        
        // Load GLB model if available
        if (unitData.modelPrefab != null)
        {
            Debug.Log($"Loading GLB model: {unitData.modelPrefab.name}");
            GameObject model = Instantiate(unitData.modelPrefab, unitObj.transform);
            Debug.Log($"✓ GLB model loaded: {model.name}");
        }
        else
        {
            Debug.Log($"⚠️ No GLB model found for {unitData.unitName}");
            // Create a basic cube as placeholder
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(unitObj.transform);
            cube.transform.localPosition = Vector3.zero;
            Debug.Log($"Created placeholder cube for {unitData.unitName}");
        }
        
        unit.SetGridPosition(gridPosition);
        Debug.Log($"Set grid position: {gridPosition}");

        // Position in world
        Vector3 worldPos = gridManager.GridToWorld(gridPosition);
        unitObj.transform.position = worldPos;
        Debug.Log($"Set world position: {worldPos}");
        
        // Add to units list
        allUnits.Add(unit);
        
        // Subscribe to events
        unit.OnUnitDestroyed += HandleUnitDestroyed;
        unit.OnUnitDamaged += HandleUnitDamaged;
        unit.OnUnitMoved += HandleUnitMoved;
        
        Debug.Log($"Created unit: {unit.name} at {gridPosition}");
        
        return unit;
    }
    
    public void RemoveUnit(Unit unit)
    {
        if (unit == null) return;
        
        // Unsubscribe from events
        unit.OnUnitDestroyed -= HandleUnitDestroyed;
        unit.OnUnitDamaged -= HandleUnitDamaged;
        unit.OnUnitMoved -= HandleUnitMoved;
        
        // Remove from list
        allUnits.Remove(unit);
        
        Debug.Log($"Removed unit: {unit.name}");
    }
    
    public bool CanMoveUnit(Unit unit, GridTile targetTile)
    {
        if (unit == null || targetTile == null) return false;
        
        // Unit must be able to move
        if (!unit.CanMove()) return false;
        
        // Tile must be able to be moved to
        if (!targetTile.CanStopOn()) return false;
        
        // Check if tile is within movement range
        List<GridTile> movementRange = GetMovementRange(unit);
        return movementRange.Contains(targetTile);
    }
    
        public void MoveUnit(Unit unit, GridTile targetTile)
    {
        Debug.Log($"=== UNIT MANAGER MOVE UNIT ===");
        Debug.Log($"Unit: {unit.name}");
        Debug.Log($"Target: {targetTile.GetGridPosition()}");
        
        if (!CanMoveUnit(unit, targetTile))
        {
            Debug.LogWarning($"✗ Cannot move {unit.name} to {targetTile.GetGridPosition()}");
            return;
        }

        Debug.Log($"✓ Starting movement coroutine for {unit.name}");
        
        // Start movement coroutine
        StartCoroutine(unit.MoveTo(targetTile.GetGridPosition()));
        
        Debug.Log($"✓ Movement coroutine started");
    }
    
    public List<GridTile> GetMovementRange(Unit unit)
    {
        if (unit == null || gridManager == null) return new List<GridTile>();
        
        return gridManager.GetTilesInRange(unit.currentTile, unit.unitData.movement);
    }
    
    public void ShowMovementRange(Unit unit)
    {
        if (unit == null || gridManager == null) return;
        
        List<GridTile> movementRange = GetMovementRange(unit);
        gridManager.ShowMovementRange(movementRange);
    }
    
    public void ClearMovementRange()
    {
        if (gridManager != null)
        {
            gridManager.ClearMovementRange();
        }
    }
    
    public List<Unit> GetUnitsForPlayer(int playerId)
    {
        List<Unit> playerUnits = new List<Unit>();
        
        foreach (Unit unit in allUnits)
        {
            if (unit.owner == playerId)
            {
                playerUnits.Add(unit);
            }
        }
        
        return playerUnits;
    }
    
    public void ResetPlayerUnits(int playerId)
    {
        List<Unit> playerUnits = GetUnitsForPlayer(playerId);
        
        foreach (Unit unit in playerUnits)
        {
            unit.ResetTurn();
        }
        
        Debug.Log($"Reset {playerUnits.Count} units for player {playerId}");
    }
    
    public void ResetAllUnits()
    {
        foreach (Unit unit in allUnits)
        {
            unit.ResetTurn();
        }
        
        Debug.Log($"Reset all {allUnits.Count} units");
    }
    
    public Unit GetUnitAtPosition(Vector2Int gridPosition)
    {
        if (gridManager == null) return null;
        
        GridTile tile = gridManager.GetTile(gridPosition);
        return tile?.occupyingUnit;
    }
    
    public List<Unit> GetEnemyUnitsInRange(Unit unit, int range)
    {
        List<Unit> enemyUnits = new List<Unit>();
        
        if (unit == null || gridManager == null) return enemyUnits;
        
        List<GridTile> tilesInRange = gridManager.GetTilesInRange(unit.currentTile, range);
        
        foreach (GridTile tile in tilesInRange)
        {
            if (tile.occupyingUnit != null && tile.occupyingUnit.owner != unit.owner)
            {
                enemyUnits.Add(tile.occupyingUnit);
            }
        }
        
        return enemyUnits;
    }
    
    public List<Unit> GetAllUnits()
    {
        return new List<Unit>(allUnits);
    }
    
    public int GetUnitCount()
    {
        return allUnits.Count;
    }
    
    public int GetUnitCountForPlayer(int playerId)
    {
        return GetUnitsForPlayer(playerId).Count;
    }
    
    // Event handlers
    private void HandleUnitDestroyed(Unit unit)
    {
        Debug.Log($"Unit destroyed: {unit.name}");
        
        // TODO: Add destruction effects, sounds, etc.
        // TODO: Check victory conditions
    }
    
    private void HandleUnitDamaged(Unit unit)
    {
        Debug.Log($"Unit damaged: {unit.name} - HP: {unit.currentHP}/{unit.unitData.maxHP}");
        
        // TODO: Add damage effects, sounds, etc.
    }
    
    private void HandleUnitMoved(Unit unit)
    {
        Debug.Log($"Unit moved: {unit.name} to {unit.gridPosition}");
        
        // TODO: Add movement effects, check for capture opportunities, etc.
    }
}

[System.Serializable]
public class StartingUnit
{
    public UnitData unitData;
    public int owner;
    public Vector2Int gridPosition;
} 