using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 15;
    public int gridHeight = 15;
    public float tileSize = 2f;
    
    // Convenience property for terrain manager
    public Vector2Int gridSize => new Vector2Int(gridWidth, gridHeight);
    
    [Header("Terrain Prefabs")]
    public GameObject plainTilePrefab;
    public GameObject mountainTilePrefab;
    
    [Header("Grid Visual")]
    public Material highlightMaterial;
    public Material movementRangeMaterial;
    public Material attackRangeMaterial;
    
    // Grid data
    private GridTile[,] grid;
    private Grid unityGrid;
    private Camera mainCamera;
    
    // Visual indicators
    private List<GridTile> highlightedTiles = new List<GridTile>();
    private List<GridTile> movementRangeTiles = new List<GridTile>();
    
    // Singleton access
    public static GridManager Instance { get; private set; }
    
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
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();
    }
    
    public void InitializeGrid()
    {
        // Create Unity Grid component
        unityGrid = GetComponent<Grid>();
        if (unityGrid == null)
        {
            unityGrid = gameObject.AddComponent<Grid>();
        }
        
        // Set grid cell size
        unityGrid.cellSize = new Vector3(tileSize, tileSize, tileSize);
        
        // Initialize grid array
        grid = new GridTile[gridWidth, gridHeight];
        
        // Generate the map
        GenerateMap();
        
        Debug.Log($"Grid initialized: {gridWidth}x{gridHeight} tiles");
    }
    
    void GenerateMap()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // 90% plains, 10% mountains (similar to Three.js version)
                TileType tileType = (Random.Range(0f, 1f) < 0.9f) ? TileType.Plain : TileType.Mountain;
                
                // Create the tile
                CreateTile(x, z, tileType);
            }
        }
    }
    
    void CreateTile(int x, int z, TileType tileType)
    {
        // Choose prefab based on tile type
        GameObject prefab = (tileType == TileType.Plain) ? plainTilePrefab : mountainTilePrefab;
        
        if (prefab == null)
        {
            Debug.LogWarning($"No prefab assigned for tile type: {tileType}");
            return;
        }
        
        // Calculate world position
        Vector3 worldPos = GridToWorld(new Vector2Int(x, z));
        
        // Instantiate the tile
        GameObject tileObj = Instantiate(prefab, worldPos, Quaternion.identity, transform);
        tileObj.name = $"Tile_{x}_{z}_{tileType}";
        
        // Add GridTile component
        GridTile gridTile = tileObj.GetComponent<GridTile>();
        if (gridTile == null)
        {
            gridTile = tileObj.AddComponent<GridTile>();
        }
        
        // Initialize tile data
        gridTile.Initialize(x, z, tileType, this);
        
        // Store in grid array
        grid[x, z] = gridTile;
        
        // Add collider for raycasting if not present
        if (tileObj.GetComponent<Collider>() == null)
        {
            BoxCollider collider = tileObj.AddComponent<BoxCollider>();
            collider.size = new Vector3(tileSize * 0.9f, 0.1f, tileSize * 0.9f);
        }
        
        // Set layer for raycasting
        tileObj.layer = LayerMask.NameToLayer("Tiles");
        if (tileObj.layer == -1)
        {
            // Create Tiles layer if it doesn't exist
            tileObj.layer = 8; // Default to layer 8
        }
    }
    
    // Convert grid coordinates to world position
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        if (unityGrid != null)
        {
            Vector3Int cellPos = new Vector3Int(gridPos.x, 0, gridPos.y);
            return unityGrid.CellToWorld(cellPos);
        }
        
        // Fallback calculation
        return new Vector3(gridPos.x * tileSize, 0, gridPos.y * tileSize);
    }
    
    // Convert world position to grid coordinates
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        if (unityGrid != null)
        {
            Vector3Int cellPos = unityGrid.WorldToCell(worldPos);
            return new Vector2Int(cellPos.x, cellPos.z);
        }
        
        // Fallback calculation
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / tileSize),
            Mathf.FloorToInt(worldPos.z / tileSize)
        );
    }
    
    // Get tile at grid coordinates
    public GridTile GetTile(int x, int z)
    {
        if (IsValidGridPosition(x, z))
            return grid[x, z];
        return null;
    }
    
    public GridTile GetTile(Vector2Int gridPos)
    {
        return GetTile(gridPos.x, gridPos.y);
    }
    
    // Check if grid position is valid
    public bool IsValidGridPosition(int x, int z)
    {
        return x >= 0 && x < gridWidth && z >= 0 && z < gridHeight;
    }
    
    public bool IsValidGridPosition(Vector2Int gridPos)
    {
        return IsValidGridPosition(gridPos.x, gridPos.y);
    }
    
    // Get neighbors of a tile
    public List<GridTile> GetNeighbors(GridTile tile)
    {
        List<GridTile> neighbors = new List<GridTile>();
        
        // Check all 4 directions
        Vector2Int[] directions = {
            Vector2Int.up,    // North
            Vector2Int.down,  // South
            Vector2Int.left,  // West
            Vector2Int.right  // East
        };
        
        foreach (Vector2Int dir in directions)
        {
            int newX = tile.gridX + dir.x;
            int newZ = tile.gridZ + dir.y;
            
            GridTile neighbor = GetTile(newX, newZ);
            if (neighbor != null)
                neighbors.Add(neighbor);
        }
        
        return neighbors;
    }
    
    // Highlight tiles
    public void HighlightTile(GridTile tile)
    {
        if (tile == null) return;
        
        tile.SetHighlight(true, highlightMaterial);
        if (!highlightedTiles.Contains(tile))
            highlightedTiles.Add(tile);
    }
    
    public void ClearHighlight(GridTile tile)
    {
        if (tile == null) return;
        
        tile.SetHighlight(false, null);
        highlightedTiles.Remove(tile);
    }
    
    public void ClearAllHighlights()
    {
        foreach (GridTile tile in highlightedTiles)
        {
            if (tile != null)
                tile.SetHighlight(false, null);
        }
        highlightedTiles.Clear();
    }
    
    // Show movement range
    public void ShowMovementRange(List<GridTile> tiles)
    {
        ClearMovementRange();
        
        foreach (GridTile tile in tiles)
        {
            if (tile != null)
            {
                tile.SetMovementRange(true, movementRangeMaterial);
                movementRangeTiles.Add(tile);
            }
        }
    }
    
    public void ClearMovementRange()
    {
        foreach (GridTile tile in movementRangeTiles)
        {
            if (tile != null)
                tile.SetMovementRange(false, null);
        }
        movementRangeTiles.Clear();
    }
    
    // Raycast from screen position to get tile
    public GridTile GetTileFromScreenPosition(Vector2 screenPos)
    {
        if (mainCamera == null) return null;
        
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        
        // Raycast against tiles layer
        int tilesLayer = LayerMask.GetMask("Tiles");
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tilesLayer))
        {
            return hit.collider.GetComponent<GridTile>();
        }
        
        return null;
    }
    
    // Calculate distance between two tiles
    public int GetDistance(GridTile tile1, GridTile tile2)
    {
        if (tile1 == null || tile2 == null) return int.MaxValue;
        
        return Mathf.Abs(tile1.gridX - tile2.gridX) + Mathf.Abs(tile1.gridZ - tile2.gridZ);
    }
    
    // Find path between two tiles (A* pathfinding)
    public List<GridTile> FindPath(GridTile start, GridTile end, int maxMovement)
    {
        if (start == null || end == null) return new List<GridTile>();
        
        // TODO: Implement A* pathfinding
        // For now, return empty list
        return new List<GridTile>();
    }
    
    // Get all tiles within movement range
    public List<GridTile> GetTilesInRange(GridTile center, int range)
    {
        List<GridTile> tilesInRange = new List<GridTile>();
        
        if (center == null) return tilesInRange;
        
        // Simple flood-fill algorithm
        Queue<GridTile> queue = new Queue<GridTile>();
        HashSet<GridTile> visited = new HashSet<GridTile>();
        Dictionary<GridTile, int> distances = new Dictionary<GridTile, int>();
        
        queue.Enqueue(center);
        visited.Add(center);
        distances[center] = 0;
        
        while (queue.Count > 0)
        {
            GridTile current = queue.Dequeue();
            int currentDistance = distances[current];
            
            if (currentDistance < range)
            {
                foreach (GridTile neighbor in GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor))
                    {
                        int newDistance = currentDistance + neighbor.movementCost;
                        if (newDistance <= range)
                        {
                            visited.Add(neighbor);
                            distances[neighbor] = newDistance;
                            queue.Enqueue(neighbor);
                            tilesInRange.Add(neighbor);
                        }
                    }
                }
            }
        }
        
        return tilesInRange;
    }
}

public enum TileType
{
    Plain,
    Mountain,
    Forest,
    Road,
    River,
    Bridge,
    Reef,
    Building
} 