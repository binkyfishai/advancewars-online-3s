using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text turnText;
    public Text playerText;
    public Text fundsText;
    public Button endTurnButton;
    
    [Header("Info Panels")]
    public GameObject unitInfoPanel;
    public GameObject tileInfoPanel;
    
    // Singleton
    public static UIManager Instance { get; private set; }
    
    // References
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
        gameManager = GameManager.Instance;
        
        // Set up end turn button
        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }
        
        // Initialize UI
        UpdateUI();
    }
    
    public void UpdateTurnDisplay(int currentPlayer, int currentTurn, int currentDay)
    {
        if (turnText != null)
        {
            turnText.text = $"Turn {currentTurn} - Day {currentDay}";
        }
        
        if (playerText != null && gameManager != null && currentPlayer < gameManager.players.Count)
        {
            playerText.text = $"Player: {gameManager.players[currentPlayer].name}";
        }
        
        if (fundsText != null && gameManager != null && currentPlayer < gameManager.players.Count)
        {
            fundsText.text = $"Funds: {gameManager.players[currentPlayer].funds}";
        }
    }
    
    public void UpdateUnitInfo(Unit unit)
    {
        if (unit == null)
        {
            HideUnitInfo();
            return;
        }
        
        // TODO: Implement unit info panel updates
        if (unitInfoPanel != null)
        {
            unitInfoPanel.SetActive(true);
        }
        
        Debug.Log($"Showing info for unit: {unit.unitData.unitName}");
    }
    
    public void UpdateTileInfo(GridTile tile)
    {
        if (tile == null)
        {
            HideTileInfo();
            return;
        }
        
        // TODO: Implement tile info panel updates
        if (tileInfoPanel != null)
        {
            tileInfoPanel.SetActive(true);
        }
        
        Debug.Log($"Showing info for tile: {tile.tileType}");
    }
    
    public void HideUnitInfo()
    {
        if (unitInfoPanel != null)
        {
            unitInfoPanel.SetActive(false);
        }
    }
    
    public void HideTileInfo()
    {
        if (tileInfoPanel != null)
        {
            tileInfoPanel.SetActive(false);
        }
    }
    
    public void UpdateUI()
    {
        if (gameManager != null)
        {
            UpdateTurnDisplay(gameManager.currentPlayer, gameManager.currentTurn, gameManager.currentDay);
        }
    }
    
    private void OnEndTurnClicked()
    {
        if (gameManager != null)
        {
            gameManager.EndTurn();
        }
    }
    
    void Update()
    {
        // Update UI every frame if needed
        UpdateUI();
    }
} 