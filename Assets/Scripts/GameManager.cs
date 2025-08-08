using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float gameSpeed = 1f;
    public bool gamePaused = false;
    
    [Header("Territories")]
    public List<Territory> allTerritories = new List<Territory>();
    public Territory selectedTerritory;
    
    [Header("Army Movement")]
    public GameObject armyPrefab;
    public List<ArmyMovement> activeArmies = new List<ArmyMovement>();
    
    [Header("UI")]
    public Text gameInfoText;
    public Text selectedTerritoryInfo;
    public Button pauseButton;
    public Text pauseButtonText;
    
    [Header("Battle Settings")]
    public float battleDuration = 1f;
    public bool showBattleAnimations = true;
    
    private AIController aiController;
    private bool isPlayerTurn = true;
    
    void Start()
    {
        aiController = FindObjectOfType<AIController>();
        if (aiController == null)
        {
            aiController = gameObject.AddComponent<AIController>();
        }
        
        // Find all territories in the scene
        Territory[] territories = FindObjectsOfType<Territory>();
        allTerritories.AddRange(territories);
        
        UpdateUI();
    }
    
    void Update()
    {
        if (!gamePaused)
        {
            // Update game speed
            Time.timeScale = gameSpeed;
            
            // Update active armies list (remove destroyed ones)
            activeArmies.RemoveAll(army => army == null);
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
    
    public void OnTerritoryClicked(Territory territory)
    {
        if (gamePaused) return;
        
        // If no territory is selected, select this one (if it's player's)
        if (selectedTerritory == null)
        {
            if (territory.faction == FactionType.Player)
            {
                SelectTerritory(territory);
            }
        }
        // If a territory is already selected, try to send army
        else
        {
            if (territory != selectedTerritory && selectedTerritory.IsConnectedTo(territory))
            {
                SendArmy(selectedTerritory, territory);
                DeselectTerritory();
            }
            else if (territory.faction == FactionType.Player)
            {
                // Select different player territory
                DeselectTerritory();
                SelectTerritory(territory);
            }
        }
    }
    
    public void OnTerritoryHover(Territory territory)
    {
        if (selectedTerritory != null && selectedTerritory.IsConnectedTo(territory))
        {
            // Show connection preview
            territory.SetSelected(true);
        }
    }
    
    public void OnTerritoryUnhover(Territory territory)
    {
        if (territory != selectedTerritory)
        {
            territory.SetSelected(false);
        }
    }
    
    void SelectTerritory(Territory territory)
    {
        selectedTerritory = territory;
        territory.SetSelected(true);
        UpdateUI();
    }
    
    void DeselectTerritory()
    {
        if (selectedTerritory != null)
        {
            selectedTerritory.SetSelected(false);
            selectedTerritory = null;
        }
        UpdateUI();
    }
    
    public void SendArmy(Territory source, Territory target)
    {
        if (source.troopCount <= 1) return; // Keep at least 1 troop
        
        int troopsToSend = source.troopCount - 1; // Send all but 1
        source.RemoveTroops(troopsToSend);
        
        // Create army
        GameObject armyObj = Instantiate(armyPrefab, source.transform.position, Quaternion.identity);
        ArmyMovement army = armyObj.GetComponent<ArmyMovement>();
        
        if (army != null)
        {
            army.Initialize(source, target, troopsToSend, source.faction);
            activeArmies.Add(army);
        }
    }
    
    public void OnArmyArrived(ArmyMovement army)
    {
        activeArmies.Remove(army);
        
        // Handle army arrival
        if (army.targetTerritory.faction == army.faction)
        {
            // Reinforce friendly territory
            army.targetTerritory.AddTroops(army.troopCount);
        }
        else
        {
            // Battle for enemy territory
            ResolveBattle(army);
        }
    }
    
    void ResolveBattle(ArmyMovement army)
    {
        Territory defendingTerritory = army.targetTerritory;
        int attackingTroops = army.troopCount;
        int defendingTroops = defendingTerritory.troopCount;
        
        // Simple battle resolution: compare troop counts
        if (attackingTroops > defendingTroops)
        {
            // Attacker wins
            int remainingTroops = attackingTroops - defendingTroops;
            defendingTerritory.ChangeOwnership(army.faction);
            defendingTerritory.troopCount = remainingTroops;
            defendingTerritory.UpdateVisuals();
        }
        else
        {
            // Defender wins or tie
            int remainingTroops = defendingTroops - attackingTroops;
            defendingTerritory.troopCount = remainingTroops;
            defendingTerritory.UpdateVisuals();
        }
        
        // Check for game over conditions
        CheckGameOver();
    }
    
    void CheckGameOver()
    {
        // Count territories by faction
        Dictionary<FactionType, int> factionTerritories = new Dictionary<FactionType, int>();
        factionTerritories[FactionType.Player] = 0;
        factionTerritories[FactionType.AI1] = 0;
        factionTerritories[FactionType.AI2] = 0;
        
        foreach (Territory territory in allTerritories)
        {
            if (factionTerritories.ContainsKey(territory.faction))
            {
                factionTerritories[territory.faction]++;
            }
        }
        
        // Check if any faction has no territories
        if (factionTerritories[FactionType.Player] == 0)
        {
            Debug.Log("Game Over: Player defeated!");
            // TODO: Show game over screen
        }
        else if (factionTerritories[FactionType.AI1] == 0 && factionTerritories[FactionType.AI2] == 0)
        {
            Debug.Log("Victory: Player has conquered all territories!");
            // TODO: Show victory screen
        }
    }
    
    public void TogglePause()
    {
        gamePaused = !gamePaused;
        UpdateUI();
    }
    
    public void SetGameSpeed(float speed)
    {
        gameSpeed = Mathf.Clamp(speed, 0.1f, 3f);
    }
    
    void UpdateUI()
    {
        if (gameInfoText != null)
        {
            gameInfoText.text = $"Active Armies: {activeArmies.Count}\nGame Speed: {gameSpeed:F1}x";
        }
        
        if (selectedTerritoryInfo != null)
        {
            if (selectedTerritory != null)
            {
                selectedTerritoryInfo.text = $"Selected: {selectedTerritory.territoryName}\nTroops: {selectedTerritory.troopCount}";
            }
            else
            {
                selectedTerritoryInfo.text = "No territory selected";
            }
        }
        
        if (pauseButtonText != null)
        {
            pauseButtonText.text = gamePaused ? "Resume" : "Pause";
        }
    }
    
    public List<Territory> GetTerritoriesByFaction(FactionType faction)
    {
        List<Territory> factionTerritories = new List<Territory>();
        foreach (Territory territory in allTerritories)
        {
            if (territory.faction == faction)
            {
                factionTerritories.Add(territory);
            }
        }
        return factionTerritories;
    }
}
