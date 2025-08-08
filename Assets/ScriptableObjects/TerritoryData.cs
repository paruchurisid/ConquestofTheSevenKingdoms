using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Territory Data", menuName = "Conquest/Territory Data")]
public class TerritoryData : ScriptableObject
{
    [System.Serializable]
    public class TerritoryInfo
    {
        public string territoryName = "Territory";
        public Vector2 position = Vector2.zero;
        public Territory.FactionType startingFaction = Territory.FactionType.Neutral;
        public int startingTroops = 10;
        public float troopProductionRate = 1f;
        public List<int> connectedTerritoryIndices = new List<int>();
    }

    [Header("Map Settings")]
    public string mapName = "Default Map";
    public Sprite backgroundTexture;
    public Vector2 mapSize = new Vector2(10f, 10f);
    
    [Header("Territories")]
    public List<TerritoryInfo> territories = new List<TerritoryInfo>();

    public GameObject CreateTerritoryGameObject(TerritoryInfo info, GameObject territoryPrefab)
    {
        GameObject territoryObj = Instantiate(territoryPrefab);
        Territory territory = territoryObj.GetComponent<Territory>();
        
        if (territory != null)
        {
            territory.territoryName = info.territoryName;
            territory.faction = info.startingFaction;
            territory.troopCount = info.startingTroops;
            territory.troopProductionRate = info.troopProductionRate;
            territoryObj.transform.position = info.position;
        }
        
        return territoryObj;
    }

    public void SetupConnections(List<Territory> territoryObjects)
    {
        for (int i = 0; i < territories.Count && i < territoryObjects.Count; i++)
        {
            Territory currentTerritory = territoryObjects[i];
            TerritoryInfo info = territories[i];
            
            foreach (int connectedIndex in info.connectedTerritoryIndices)
            {
                if (connectedIndex >= 0 && connectedIndex < territoryObjects.Count)
                {
                    Territory connectedTerritory = territoryObjects[connectedIndex];
                    Territory.TerritoryConnection connection = new Territory.TerritoryConnection();
                    connection.connectedTerritory = connectedTerritory;
                    connection.travelTime = 2f; // Default travel time
                    currentTerritory.connections.Add(connection);
                }
            }
        }
    }

    public bool ValidateData()
    {
        // Check for invalid connections
        for (int i = 0; i < territories.Count; i++)
        {
            foreach (int connectedIndex in territories[i].connectedTerritoryIndices)
            {
                if (connectedIndex < 0 || connectedIndex >= territories.Count)
                {
                    Debug.LogError($"Territory {i} has invalid connection to index {connectedIndex}");
                    return false;
                }
            }
        }
        return true;
    }

    [ContextMenu("Create Sample Map")]
    public void CreateSampleMap()
    {
        territories.Clear();
        
        // Create 5 territories in a connected pattern
        territories.Add(new TerritoryInfo
        {
            territoryName = "Winterfell",
            position = new Vector2(-2f, 2f),
            startingFaction = Territory.FactionType.Player,
            startingTroops = 15,
            troopProductionRate = 1.2f,
            connectedTerritoryIndices = new List<int> { 1, 2 }
        });
        
        territories.Add(new TerritoryInfo
        {
            territoryName = "The Twins",
            position = new Vector2(0f, 2f),
            startingFaction = Territory.FactionType.AI1,
            startingTroops = 12,
            troopProductionRate = 1f,
            connectedTerritoryIndices = new List<int> { 0, 2, 3 }
        });
        
        territories.Add(new TerritoryInfo
        {
            territoryName = "Riverrun",
            position = new Vector2(-1f, 0f),
            startingFaction = Territory.FactionType.Neutral,
            startingTroops = 8,
            troopProductionRate = 0.8f,
            connectedTerritoryIndices = new List<int> { 0, 1, 3, 4 }
        });
        
        territories.Add(new TerritoryInfo
        {
            territoryName = "Casterly Rock",
            position = new Vector2(1f, 0f),
            startingFaction = Territory.FactionType.AI2,
            startingTroops = 18,
            troopProductionRate = 1.5f,
            connectedTerritoryIndices = new List<int> { 1, 2, 4 }
        });
        
        territories.Add(new TerritoryInfo
        {
            territoryName = "Highgarden",
            position = new Vector2(0f, -2f),
            startingFaction = Territory.FactionType.Neutral,
            startingTroops = 10,
            troopProductionRate = 1f,
            connectedTerritoryIndices = new List<int> { 2, 3 }
        });
        
        mapName = "Westeros Sample Map";
        mapSize = new Vector2(8f, 8f);
        
        Debug.Log("Sample map created with 5 territories!");
    }
}
