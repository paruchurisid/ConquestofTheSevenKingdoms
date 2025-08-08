using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("AI Settings")]
    public float decisionInterval = 3f; // How often AI makes decisions
    public float aggressionLevel = 0.7f; // 0-1, higher = more aggressive
    public float decisionTimer = 0f;
    
    [Header("AI Factions")]
    public FactionType aiFaction1 = FactionType.AI1;
    public FactionType aiFaction2 = FactionType.AI2;
    
    [Header("Strategy Weights")]
    public float attackWeight = 0.4f;
    public float defendWeight = 0.3f;
    public float reinforceWeight = 0.3f;
    
    private GameManager gameManager;
    private Dictionary<FactionType, float> factionTimers = new Dictionary<FactionType, float>();
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("AIController: GameManager not found!");
            return;
        }
        
        // Initialize timers for each AI faction
        factionTimers[aiFaction1] = Random.Range(0f, decisionInterval);
        factionTimers[aiFaction2] = Random.Range(0f, decisionInterval);
    }
    
    void Update()
    {
        if (gameManager.gamePaused) return;
        
        // Update timers for each AI faction
        factionTimers[aiFaction1] += Time.deltaTime;
        factionTimers[aiFaction2] += Time.deltaTime;
        
        // Make decisions when timer expires
        if (factionTimers[aiFaction1] >= decisionInterval)
        {
            MakeAIDecision(aiFaction1);
            factionTimers[aiFaction1] = 0f;
        }
        
        if (factionTimers[aiFaction2] >= decisionInterval)
        {
            MakeAIDecision(aiFaction2);
            factionTimers[aiFaction2] = 0f;
        }
    }
    
    void MakeAIDecision(FactionType aiFaction)
    {
        List<Territory> aiTerritories = gameManager.GetTerritoriesByFaction(aiFaction);
        
        if (aiTerritories.Count == 0) return;
        
        // Decide on strategy
        float strategyRoll = Random.Range(0f, 1f);
        float currentAttackWeight = attackWeight * aggressionLevel;
        float currentDefendWeight = defendWeight;
        float currentReinforceWeight = reinforceWeight;
        
        if (strategyRoll < currentAttackWeight)
        {
            // Attack strategy
            TryAttack(aiFaction, aiTerritories);
        }
        else if (strategyRoll < currentAttackWeight + currentDefendWeight)
        {
            // Defend strategy
            TryDefend(aiFaction, aiTerritories);
        }
        else
        {
            // Reinforce strategy
            TryReinforce(aiFaction, aiTerritories);
        }
    }
    
    void TryAttack(FactionType aiFaction, List<Territory> aiTerritories)
    {
        // Find territories with enough troops to attack
        List<Territory> strongTerritories = new List<Territory>();
        foreach (Territory territory in aiTerritories)
        {
            if (territory.troopCount > 5) // Minimum troops needed to attack
            {
                strongTerritories.Add(territory);
            }
        }
        
        if (strongTerritories.Count == 0) return;
        
        // Find enemy territories to attack
        List<Territory> enemyTerritories = new List<Territory>();
        foreach (Territory territory in gameManager.allTerritories)
        {
            if (territory.faction != aiFaction && territory.faction != FactionType.Neutral)
            {
                enemyTerritories.Add(territory);
            }
        }
        
        if (enemyTerritories.Count == 0) return;
        
        // Try to attack from each strong territory
        foreach (Territory sourceTerritory in strongTerritories)
        {
            // Find connected enemy territories
            List<Territory> connectedEnemies = new List<Territory>();
            foreach (TerritoryConnection connection in sourceTerritory.connections)
            {
                if (connection.connectedTerritory.faction != aiFaction)
                {
                    connectedEnemies.Add(connection.connectedTerritory);
                }
            }
            
            if (connectedEnemies.Count > 0)
            {
                // Choose target (prefer weaker enemies)
                Territory target = ChooseWeakestTerritory(connectedEnemies);
                
                // Send attack
                SendAIArmy(sourceTerritory, target, aiFaction);
                break; // Only attack from one territory per decision
            }
        }
    }
    
    void TryDefend(FactionType aiFaction, List<Territory> aiTerritories)
    {
        // Find territories that are under threat (adjacent to enemies)
        List<Territory> threatenedTerritories = new List<Territory>();
        
        foreach (Territory territory in aiTerritories)
        {
            foreach (TerritoryConnection connection in territory.connections)
            {
                if (connection.connectedTerritory.faction != aiFaction && 
                    connection.connectedTerritory.faction != FactionType.Neutral)
                {
                    // Check if enemy has more troops
                    if (connection.connectedTerritory.troopCount > territory.troopCount)
                    {
                        threatenedTerritories.Add(territory);
                        break;
                    }
                }
            }
        }
        
        if (threatenedTerritories.Count == 0) return;
        
        // Reinforce threatened territories
        foreach (Territory threatenedTerritory in threatenedTerritories)
        {
            // Find a strong territory to send reinforcements from
            Territory reinforcementSource = FindStrongestTerritory(aiTerritories);
            if (reinforcementSource != null && reinforcementSource != threatenedTerritory)
            {
                SendAIArmy(reinforcementSource, threatenedTerritory, aiFaction);
                break;
            }
        }
    }
    
    void TryReinforce(FactionType aiFaction, List<Territory> aiTerritories)
    {
        // Find the weakest territory and reinforce it
        Territory weakestTerritory = FindWeakestTerritory(aiTerritories);
        Territory strongestTerritory = FindStrongestTerritory(aiTerritories);
        
        if (weakestTerritory != null && strongestTerritory != null && 
            weakestTerritory != strongestTerritory)
        {
            // Check if they're connected
            if (strongestTerritory.IsConnectedTo(weakestTerritory))
            {
                SendAIArmy(strongestTerritory, weakestTerritory, aiFaction);
            }
        }
    }
    
    Territory FindWeakestTerritory(List<Territory> territories)
    {
        Territory weakest = null;
        int lowestTroops = int.MaxValue;
        
        foreach (Territory territory in territories)
        {
            if (territory.troopCount < lowestTroops)
            {
                lowestTroops = territory.troopCount;
                weakest = territory;
            }
        }
        
        return weakest;
    }
    
    Territory FindStrongestTerritory(List<Territory> territories)
    {
        Territory strongest = null;
        int highestTroops = 0;
        
        foreach (Territory territory in territories)
        {
            if (territory.troopCount > highestTroops)
            {
                highestTroops = territory.troopCount;
                strongest = territory;
            }
        }
        
        return strongest;
    }
    
    Territory ChooseWeakestTerritory(List<Territory> territories)
    {
        return FindWeakestTerritory(territories);
    }
    
    void SendAIArmy(Territory source, Territory target, FactionType aiFaction)
    {
        if (source.troopCount <= 1) return;
        
        int troopsToSend = Mathf.Min(source.troopCount - 1, source.troopCount / 2);
        source.RemoveTroops(troopsToSend);
        
        // Create army using GameManager's army prefab
        GameObject armyObj = Instantiate(gameManager.armyPrefab, source.transform.position, Quaternion.identity);
        ArmyMovement army = armyObj.GetComponent<ArmyMovement>();
        
        if (army != null)
        {
            army.Initialize(source, target, troopsToSend, aiFaction);
            gameManager.activeArmies.Add(army);
        }
    }
    
    public void SetAggressionLevel(float level)
    {
        aggressionLevel = Mathf.Clamp01(level);
    }
    
    public void SetDecisionInterval(float interval)
    {
        decisionInterval = Mathf.Max(0.5f, interval);
    }
}
