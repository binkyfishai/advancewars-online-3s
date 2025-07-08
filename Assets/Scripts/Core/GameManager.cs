using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Managers")]
    public GridManager gridManager;
    public UnitManager unitManager;
    public BuildingManager buildingManager;
    public UIManager uiManager;
    public TerrainManager terrainManager;
    public InputManager inputManager;
    
    [Header("Game State")]
    public int currentPlayer = 0;
    public int totalPlayers = 2;
    public int currentTurn = 1;
    public int currentDay = 1;
    
    [Header("Selection")]
    public GridTile selectedTile;
    public Unit selectedUnit;
    
    [Header("Testing")]
    public bool spawnTestUnits = true;
    
    [Header("Unit Data for Testing")]
    public UnitData[] testUnitTypes; // Array of UnitData for testing
    
    // Simple player system for compatibility
    public List<Player> players = new List<Player>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializePlayers();
        InitializeGame();
        
        // Test spawn some units after grid is ready
        if (spawnTestUnits)
        {
            StartCoroutine(TestSpawnUnits());
        }
    }
    
    void InitializePlayers()
    {
        players.Clear();
        // Add Orange Star (Player 0)
        players.Add(new Player(0, "Orange Star", 10000));
        // Add Blue Moon (Player 1)
        players.Add(new Player(1, "Blue Moon", 10000));
    }
    
    void InitializeGame()
    {
        Debug.Log("Initializing Advance Wars game...");
        
        // Initialize managers in order
        if (gridManager != null)
        {
            gridManager.InitializeGrid();
        }
        
        if (terrainManager != null)
        {
            terrainManager.GenerateTerrain(gridManager.gridSize);
        }
        
        if (uiManager != null)
        {
            uiManager.UpdateUI();
        }
        
        Debug.Log("Game initialized!");
    }
    
    IEnumerator TestSpawnUnits()
    {
        yield return new WaitForSeconds(1f); // Wait for grid to initialize
        
        Debug.Log("=== UNIT SPAWNING DEBUG ===");
        Debug.Log($"unitManager: {(unitManager != null ? "✓" : "✗ NULL")}");
        Debug.Log($"gridManager: {(gridManager != null ? "✓" : "✗ NULL")}");
        Debug.Log($"testUnitTypes: {(testUnitTypes != null ? "✓" : "✗ NULL")}");
        
        if (testUnitTypes != null)
        {
            Debug.Log($"testUnitTypes.Length: {testUnitTypes.Length}");
            for (int i = 0; i < testUnitTypes.Length; i++)
            {
                if (testUnitTypes[i] != null)
                {
                    Debug.Log($"  [{i}] {testUnitTypes[i].unitName} (✓)");
                }
                else
                {
                    Debug.Log($"  [{i}] NULL (✗)");
                }
            }
        }
        
        if (unitManager != null && gridManager != null && testUnitTypes != null && testUnitTypes.Length > 0)
        {
            Debug.Log("All conditions met - starting unit spawning...");
            
            // Spawn Player 0 units (Orange Star)
            if (testUnitTypes.Length > 0 && testUnitTypes[0] != null)
            {
                GridTile tile1 = gridManager.GetTile(2, 2);
                Debug.Log($"Tile at (2,2): {(tile1 != null ? "✓ Found" : "✗ NULL")}");
                if (tile1 != null)
                {
                    try
                    {
                        unitManager.CreateUnit(testUnitTypes[0], 0, new Vector2Int(2, 2));
                        Debug.Log("✓ Infantry spawned at (2,2) for Player 0");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"✗ Failed to spawn Infantry: {e.Message}");
                    }
                }
            }
            
            if (testUnitTypes.Length > 1 && testUnitTypes[1] != null)
            {
                GridTile tile2 = gridManager.GetTile(5, 5);
                if (tile2 != null)
                {
                    try
                    {
                        unitManager.CreateUnit(testUnitTypes[1], 0, new Vector2Int(5, 5));
                        Debug.Log("✓ Tank spawned at (5,5) for Player 0");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"✗ Failed to spawn Tank: {e.Message}");
                    }
                }
            }
            
            // Spawn Player 1 units (Blue Moon) - ENEMY UNITS
            if (testUnitTypes.Length > 0 && testUnitTypes[0] != null)
            {
                GridTile tile3 = gridManager.GetTile(8, 8);
                if (tile3 != null)
                {
                    try
                    {
                        unitManager.CreateUnit(testUnitTypes[0], 1, new Vector2Int(8, 8));
                        Debug.Log("✓ Enemy Infantry spawned at (8,8) for Player 1");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"✗ Failed to spawn Enemy Infantry: {e.Message}");
                    }
                }
            }
            
            if (testUnitTypes.Length > 1 && testUnitTypes[1] != null)
            {
                GridTile tile4 = gridManager.GetTile(10, 10);
                if (tile4 != null)
                {
                    try
                    {
                        unitManager.CreateUnit(testUnitTypes[1], 1, new Vector2Int(10, 10));
                        Debug.Log("✓ Enemy Tank spawned at (10,10) for Player 1");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"✗ Failed to spawn Enemy Tank: {e.Message}");
                    }
                }
            }
            
            Debug.Log("=== Unit spawning completed ===");
        }
        else
        {
            Debug.LogError("✗ Cannot spawn units - missing components:");
            if (unitManager == null) Debug.LogError("  - UnitManager is NULL");
            if (gridManager == null) Debug.LogError("  - GridManager is NULL");
            if (testUnitTypes == null) Debug.LogError("  - testUnitTypes array is NULL");
            if (testUnitTypes != null && testUnitTypes.Length == 0) Debug.LogError("  - testUnitTypes array is empty");
        }
    }
    
    void Update()
    {
        // Test controls
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetAllUnits();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnTestUnitAtCursor();
        }
    }
    
    // Selection methods
    public void SelectTile(GridTile tile)
    {
        if (tile == null) return;

        Debug.Log($"=== TILE CLICKED: {tile.GetGridPosition()} ===");
        Debug.Log($"OLD Selected unit: {(selectedUnit != null ? selectedUnit.name : "None")}");
        Debug.Log($"OLD Selected tile: {(selectedTile != null ? selectedTile.GetGridPosition().ToString() : "None")}");
        Debug.Log($"NEW Tile occupying unit: {(tile.occupyingUnit != null ? tile.occupyingUnit.name : "NULL")}");
        
        // Store the current selection before changing it
        Unit previouslySelectedUnit = selectedUnit;
        GridTile previouslySelectedTile = selectedTile;
        
        // Check if we have a unit selected and clicked on an enemy unit (ATTACK)
        if (previouslySelectedUnit != null && previouslySelectedUnit.owner == currentPlayer && 
            tile.occupyingUnit != null && tile.occupyingUnit.owner != currentPlayer)
        {
            Debug.Log($"=== ATTACK CHECK ===");
            Debug.Log($"Attacker: {previouslySelectedUnit.name} (Player {previouslySelectedUnit.owner})");
            Debug.Log($"Defender: {tile.occupyingUnit.name} (Player {tile.occupyingUnit.owner})");
            
            // Check if the enemy unit is within attack range
            bool canAttack = CombatCalculator.IsInAttackRange(previouslySelectedUnit, tile.occupyingUnit);
            Debug.Log($"Can attack (in range): {canAttack}");
            
            // Check if unit can attack this turn
            bool hasNotAttacked = !previouslySelectedUnit.hasAttacked;
            Debug.Log($"Has not attacked this turn: {hasNotAttacked}");
            
            // Check if unit has ammo
            bool hasAmmo = previouslySelectedUnit.currentAmmo > 0;
            Debug.Log($"Has ammo: {hasAmmo} ({previouslySelectedUnit.currentAmmo})");
            
            if (canAttack && hasNotAttacked && hasAmmo)
            {
                Debug.Log($"✓ ATTACKING! {previouslySelectedUnit.name} attacks {tile.occupyingUnit.name}");
                
                // Temporarily restore the selection for the attack
                selectedUnit = previouslySelectedUnit;
                selectedTile = previouslySelectedTile;
                
                // Perform the attack
                AttackUnit(previouslySelectedUnit, tile.occupyingUnit);
                return;
            }
            else
            {
                Debug.Log($"✗ Cannot attack:");
                if (!canAttack) Debug.Log("  - Target not in attack range");
                if (!hasNotAttacked) Debug.Log("  - Unit has already attacked this turn");
                if (!hasAmmo) Debug.Log("  - Unit has no ammo");
            }
        }
        
        // Check if we already have a unit selected and this tile is a valid movement destination
        Debug.Log($"=== MOVEMENT CHECK ===");
        Debug.Log($"previouslySelectedUnit != null: {previouslySelectedUnit != null}");
        if (previouslySelectedUnit != null)
        {
            Debug.Log($"previouslySelectedUnit.owner: {previouslySelectedUnit.owner}");
            Debug.Log($"currentPlayer: {currentPlayer}");
            Debug.Log($"previouslySelectedUnit.owner == currentPlayer: {previouslySelectedUnit.owner == currentPlayer}");
            Debug.Log($"tile != previouslySelectedTile: {tile != previouslySelectedTile}");
        }
        
        if (previouslySelectedUnit != null && previouslySelectedUnit.owner == currentPlayer && 
            tile != previouslySelectedTile && tile.occupyingUnit == null)
        {
            Debug.Log($"✓ Checking movement range for {previouslySelectedUnit.name}");
            // Check if the clicked tile is within movement range
            var movementRange = unitManager.GetMovementRange(previouslySelectedUnit);
            Debug.Log($"Movement range size: {movementRange.Count}");
            bool isInRange = movementRange.Contains(tile);
            Debug.Log($"Target tile {tile.GetGridPosition()} is in range: {isInRange}");
            if (isInRange)
            {
                // Move the unit to the clicked tile
                Debug.Log($"✓ Moving unit from {previouslySelectedUnit.currentTile.GetGridPosition()} to {tile.GetGridPosition()}");
                // Temporarily restore the selection for the move
                selectedUnit = previouslySelectedUnit;
                selectedTile = previouslySelectedTile;
                MoveSelectedUnit(tile);
                return;
            }
            else
            {
                Debug.Log($"✗ Tile {tile.GetGridPosition()} is not in movement range");
            }
        }
        else
        {
            Debug.Log($"✗ Movement check failed - conditions not met");
        }

        // Clear previous selection
        if (previouslySelectedTile != null)
        {
            previouslySelectedTile.SetSelected(false);
        }

        // Clear movement range
        if (gridManager != null)
        {
            gridManager.ClearMovementRange();
        }

        // Select new tile
        selectedTile = tile;
        selectedUnit = tile.occupyingUnit;

        Debug.Log($"NEW Selected unit: {(selectedUnit != null ? selectedUnit.name : "None")}");
        Debug.Log($"NEW Selected tile: {(selectedTile != null ? selectedTile.GetGridPosition().ToString() : "None")}");

        if (selectedTile != null)
        {
            selectedTile.SetSelected(true);

            // Show movement range if unit is selected
            if (selectedUnit != null && selectedUnit.owner == currentPlayer)
            {
                Debug.Log($"✓ Showing movement range for {selectedUnit.name} (Player {selectedUnit.owner})");
                var movementRange = unitManager.GetMovementRange(selectedUnit);
                Debug.Log($"Movement range size: {movementRange.Count}");
                gridManager.ShowMovementRange(movementRange);
            }
            else if (selectedUnit != null && selectedUnit.owner != currentPlayer)
            {
                Debug.Log($"✗ Cannot select enemy unit {selectedUnit.name} (Player {selectedUnit.owner})");
            }
        }

        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateTileInfo(selectedTile);
        }
    }
    
    public void SelectUnit(Unit unit)
    {
        if (unit == null) return;
        
        // Can only select own units
        if (unit.owner != currentPlayer) return;
        
        // Clear previous selections
        ClearSelections();
        
        // Select the unit
        selectedUnit = unit;
        selectedTile = unit.currentTile;
        
        if (selectedTile != null)
        {
            selectedTile.SetSelected(true);
        }
        
        // Show movement range
        if (unitManager != null)
        {
            unitManager.ShowMovementRange(unit);
        }
        
        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateUI();
        }
    }
    
    public void ClearSelections()
    {
        selectedUnit = null;
        selectedTile = null;
        
        // Clear visual indicators
        if (gridManager != null)
        {
            gridManager.ClearMovementRange();
        }
        
        if (unitManager != null)
        {
            unitManager.ClearMovementRange();
        }
    }
    
    public bool CanSelectTile(GridTile tile)
    {
        if (tile == null) return false;
        
        // Can always select tiles with your units
        if (tile.occupyingUnit != null && tile.occupyingUnit.owner == currentPlayer)
            return true;
        
        // Can select empty tiles if you have a unit selected
        if (selectedUnit != null && tile.occupyingUnit == null)
            return true;
        
        // Can select tiles with enemy units for attack
        if (selectedUnit != null && tile.occupyingUnit != null && tile.occupyingUnit.owner != currentPlayer)
            return true;
        
        return true; // Allow selecting any tile for info
    }
    
    public void MoveSelectedUnit(GridTile targetTile)
    {
        Debug.Log($"=== MOVE SELECTED UNIT CALLED ===");
        Debug.Log($"Selected unit: {(selectedUnit != null ? selectedUnit.name : "NULL")}");
        Debug.Log($"Target tile: {(targetTile != null ? targetTile.GetGridPosition().ToString() : "NULL")}");
        
        if (selectedUnit != null && targetTile != null)
        {
            Debug.Log($"GameManager: Moving unit {selectedUnit.name} to {targetTile.GetGridPosition()}");
            
            // Check if the move is valid
            bool canMove = unitManager.CanMoveUnit(selectedUnit, targetTile);
            Debug.Log($"UnitManager.CanMoveUnit result: {canMove}");
            
            if (canMove)
            {
                Debug.Log($"✓ Calling UnitManager.MoveUnit...");
                unitManager.MoveUnit(selectedUnit, targetTile);
                Debug.Log($"✓ Unit moved successfully to {targetTile.GetGridPosition()}");
            }
            else
            {
                Debug.LogWarning($"✗ Cannot move unit to {targetTile.GetGridPosition()} - invalid destination");
            }
            
            ClearSelections();
        }
        else
        {
            Debug.LogError("MoveSelectedUnit called with null unit or tile!");
        }
    }
    
    public void EndTurn()
    {
        Debug.Log($"Player {currentPlayer} ending turn...");
        
        // Reset all units for current player
        if (unitManager != null)
        {
            unitManager.ResetPlayerUnits(currentPlayer);
        }
        
        // Switch to next player
        currentPlayer = (currentPlayer + 1) % totalPlayers;
        
        if (currentPlayer == 0)
        {
            currentTurn++;
            currentDay++;
        }
        
        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateUI();
        }
        
        Debug.Log($"Now Player {currentPlayer}'s turn (Turn {currentTurn})");
    }
    
    public void ResetAllUnits()
    {
        if (unitManager != null)
        {
            unitManager.ResetAllUnits();
            Debug.Log("All units reset!");
        }
    }
    
    public void SpawnTestUnitAtCursor()
    {
        if (unitManager != null && gridManager != null && inputManager != null && testUnitTypes != null)
        {
            // Get tile under cursor
            GridTile cursorTile = inputManager.GetTileUnderCursor();
            if (cursorTile != null && cursorTile.occupyingUnit == null && testUnitTypes.Length > 0)
            {
                // Spawn random unit type
                int randomUnitType = Random.Range(0, testUnitTypes.Length);
                unitManager.CreateUnit(testUnitTypes[randomUnitType], currentPlayer, cursorTile.GetGridPosition());
                Debug.Log($"Spawned test unit type {randomUnitType} at cursor position");
            }
        }
    }
    
    public bool IsCurrentPlayerTurn(int playerIndex)
    {
        return playerIndex == currentPlayer;
    }
    
    public void AttackUnit(Unit attacker, Unit defender)
    {
        if (attacker == null || defender == null) return;
        
        Debug.Log($"=== COMBAT: {attacker.unitData.unitName} vs {defender.unitData.unitName} ===");
        
        // Calculate full combat result (including counter-attack)
        CombatResult combatResult = CombatCalculator.CalculateCombatResult(attacker, defender);
        
        Debug.Log($"Attack damage: {combatResult.attackerDamage}");
        Debug.Log($"Counter damage: {combatResult.defenderDamage}");
        Debug.Log($"Attacker final HP: {combatResult.attackerFinalHP}");
        Debug.Log($"Defender final HP: {combatResult.defenderFinalHP}");
        
        // Perform the attack
        attacker.AttackUnit(defender);
        
        // Apply damage to defender
        defender.TakeDamage(combatResult.attackerDamage);
        
        // Counter-attack if possible
        if (combatResult.defenderDamage > 0 && defender.currentHP > 0)
        {
            Debug.Log($"✓ Counter-attack! {defender.unitData.unitName} counter-attacks for {combatResult.defenderDamage} damage");
            attacker.TakeDamage(combatResult.defenderDamage);
        }
        
        // Report results
        switch (combatResult.winner)
        {
            case CombatWinner.Attacker:
                Debug.Log($"✓ {attacker.unitData.unitName} wins! {defender.unitData.unitName} destroyed!");
                break;
            case CombatWinner.Defender:
                Debug.Log($"✓ {defender.unitData.unitName} wins! {attacker.unitData.unitName} destroyed!");
                break;
            case CombatWinner.Draw:
                Debug.Log($"✓ Draw! Both units destroyed!");
                break;
            default:
                Debug.Log($"✓ Both units survive the combat");
                break;
        }
        
        // Clear selections after combat
        ClearSelections();
        
        Debug.Log("=== COMBAT COMPLETE ===");
    }
}

[System.Serializable]
public class Player
{
    public int id;
    public string name;
    public int funds;
    
    public Player(int id, string name, int funds)
    {
        this.id = id;
        this.name = name;
        this.funds = funds;
    }
} 