using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FactionType
{
    Player,
    AI1,
    AI2,
    Neutral
}

[System.Serializable]
public class TerritoryConnection
{
    public Territory connectedTerritory;
    public float travelTime = 2f; // Time in seconds to travel between territories
}

public class Territory : MonoBehaviour
{
    [Header("Territory Data")]
    public string territoryName = "Territory";
    public FactionType faction = FactionType.Neutral;
    public int troopCount = 10;
    public int maxTroops = 100;
    
    [Header("Production")]
    public float troopProductionRate = 1f; // Troops per second
    public float productionTimer = 0f;
    
    [Header("Connections")]
    public List<TerritoryConnection> connections = new List<TerritoryConnection>();
    
    [Header("Visual Components")]
    public SpriteRenderer territoryRenderer;
    public Text troopCountText;
    public Color playerColor = Color.blue;
    public Color ai1Color = Color.red;
    public Color ai2Color = Color.green;
    public Color neutralColor = Color.gray;
    
    [Header("Selection")]
    public bool isSelected = false;
    public Color selectedColor = Color.yellow;
    private Color originalColor;
    
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        UpdateVisuals();
        originalColor = territoryRenderer.color;
    }
    
    void Update()
    {
        // Produce troops over time
        if (faction != FactionType.Neutral && troopCount < maxTroops)
        {
            productionTimer += Time.deltaTime;
            if (productionTimer >= 1f / troopProductionRate)
            {
                troopCount++;
                productionTimer = 0f;
                UpdateVisuals();
            }
        }
    }
    
    public void UpdateVisuals()
    {
        // Update territory color based on faction
        Color factionColor = GetFactionColor();
        territoryRenderer.color = isSelected ? selectedColor : factionColor;
        
        // Update troop count text
        if (troopCountText != null)
        {
            troopCountText.text = troopCount.ToString();
            troopCountText.color = Color.white;
        }
    }
    
    public Color GetFactionColor()
    {
        switch (faction)
        {
            case FactionType.Player: return playerColor;
            case FactionType.AI1: return ai1Color;
            case FactionType.AI2: return ai2Color;
            case FactionType.Neutral: return neutralColor;
            default: return neutralColor;
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisuals();
    }
    
    public bool IsConnectedTo(Territory other)
    {
        return connections.Exists(c => c.connectedTerritory == other);
    }
    
    public TerritoryConnection GetConnection(Territory other)
    {
        return connections.Find(c => c.connectedTerritory == other);
    }
    
    public void AddTroops(int amount)
    {
        troopCount = Mathf.Min(troopCount + amount, maxTroops);
        UpdateVisuals();
    }
    
    public void RemoveTroops(int amount)
    {
        troopCount = Mathf.Max(troopCount - amount, 0);
        UpdateVisuals();
    }
    
    public void ChangeOwnership(FactionType newFaction)
    {
        faction = newFaction;
        UpdateVisuals();
    }
    
    void OnMouseDown()
    {
        if (gameManager != null)
        {
            gameManager.OnTerritoryClicked(this);
        }
    }
    
    void OnMouseEnter()
    {
        if (gameManager != null)
        {
            gameManager.OnTerritoryHover(this);
        }
    }
    
    void OnMouseExit()
    {
        if (gameManager != null)
        {
            gameManager.OnTerritoryUnhover(this);
        }
    }
}
