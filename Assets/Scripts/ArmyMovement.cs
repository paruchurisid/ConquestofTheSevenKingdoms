using System.Collections;
using UnityEngine;

public class ArmyMovement : MonoBehaviour
{
    [Header("Army Data")]
    public Territory sourceTerritory;
    public Territory targetTerritory;
    public int troopCount;
    public FactionType faction;
    
    [Header("Movement")]
    public float travelTime = 2f;
    public float currentTravelTime = 0f;
    public bool isMoving = false;
    
    [Header("Visual")]
    public SpriteRenderer armyRenderer;
    public Color playerArmyColor = Color.cyan;
    public Color ai1ArmyColor = Color.magenta;
    public Color ai2ArmyColor = Color.yellow;
    
    private GameManager gameManager;
    private Vector3 startPosition;
    private Vector3 endPosition;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        UpdateVisuals();
    }
    
    public void Initialize(Territory source, Territory target, int troops, FactionType armyFaction)
    {
        sourceTerritory = source;
        targetTerritory = target;
        troopCount = troops;
        faction = armyFaction;
        
        // Get travel time from territory connection
        TerritoryConnection connection = source.GetConnection(target);
        if (connection != null)
        {
            travelTime = connection.travelTime;
        }
        
        startPosition = source.transform.position;
        endPosition = target.transform.position;
        transform.position = startPosition;
        
        UpdateVisuals();
        StartMovement();
    }
    
    void Update()
    {
        if (isMoving)
        {
            currentTravelTime += Time.deltaTime;
            float progress = currentTravelTime / travelTime;
            
            // Move army along the path
            transform.position = Vector3.Lerp(startPosition, endPosition, progress);
            
            // Check if army has arrived
            if (progress >= 1f)
            {
                ArriveAtDestination();
            }
        }
    }
    
    void StartMovement()
    {
        isMoving = true;
        currentTravelTime = 0f;
    }
    
    void ArriveAtDestination()
    {
        isMoving = false;
        
        if (gameManager != null)
        {
            gameManager.OnArmyArrived(this);
        }
        
        // Destroy this army object
        Destroy(gameObject);
    }
    
    void UpdateVisuals()
    {
        if (armyRenderer != null)
        {
            armyRenderer.color = GetFactionColor();
        }
    }
    
    Color GetFactionColor()
    {
        switch (faction)
        {
            case FactionType.Player: return playerArmyColor;
            case FactionType.AI1: return ai1ArmyColor;
            case FactionType.AI2: return ai2ArmyColor;
            default: return Color.white;
        }
    }
    
    public void OnDrawGizmos()
    {
        if (sourceTerritory != null && targetTerritory != null)
        {
            // Draw movement path
            Gizmos.color = GetFactionColor();
            Gizmos.DrawLine(sourceTerritory.transform.position, targetTerritory.transform.position);
            
            // Draw army position
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }
    }
}
