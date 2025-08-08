using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [Header("Map Configuration")]
    public TerritoryData territoryData;
    public GameObject territoryPrefab;
    public GameObject armyPrefab;
    public SpriteRenderer backgroundRenderer;
    
    [Header("Generated Objects")]
    public List<Territory> loadedTerritories = new List<Territory>();
    
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("MapLoader: GameManager not found!");
            return;
        }
        
        LoadMap();
    }
    
    /// <summary>
    /// Loads the map from TerritoryData
    /// </summary>
    public void LoadMap()
    {
        if (territoryData == null)
        {
            Debug.LogError("MapLoader: No TerritoryData assigned!");
            return;
        }
        
        if (territoryPrefab == null)
        {
            Debug.LogError("MapLoader: No territory prefab assigned!");
            return;
        }
        
        // Validate the data
        if (!territoryData.ValidateData())
        {
            Debug.LogError("MapLoader: Territory data validation failed!");
            return;
        }
        
        // Clear existing territories
        ClearMap();
        
        // Set background
        if (backgroundRenderer != null && territoryData.backgroundTexture != null)
        {
            backgroundRenderer.sprite = territoryData.backgroundTexture;
        }
        
        // Create territories
        CreateTerritories();
        
        // Setup connections
        territoryData.SetupConnections(loadedTerritories);
        
        // Update GameManager
        if (gameManager != null)
        {
            gameManager.allTerritories.Clear();
            gameManager.allTerritories.AddRange(loadedTerritories);
            gameManager.armyPrefab = armyPrefab;
        }
        
        Debug.Log($"Map loaded: {loadedTerritories.Count} territories");
    }
    
    /// <summary>
    /// Creates territory GameObjects from the data
    /// </summary>
    void CreateTerritories()
    {
        loadedTerritories.Clear();
        
        foreach (TerritoryData.TerritoryInfo info in territoryData.territories)
        {
            GameObject territoryObj = territoryData.CreateTerritoryGameObject(info, territoryPrefab);
            Territory territory = territoryObj.GetComponent<Territory>();
            
            if (territory != null)
            {
                loadedTerritories.Add(territory);
                
                // Set up visual components
                SetupTerritoryVisuals(territory, territoryObj);
            }
        }
    }
    
    /// <summary>
    /// Sets up the visual components for a territory
    /// </summary>
    void SetupTerritoryVisuals(Territory territory, GameObject territoryObj)
    {
        // Get or create SpriteRenderer
        SpriteRenderer renderer = territoryObj.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = territoryObj.AddComponent<SpriteRenderer>();
        }
        
        // Create a simple polygon sprite if none exists
        if (renderer.sprite == null)
        {
            renderer.sprite = CreateTerritorySprite();
        }
        
        territory.territoryRenderer = renderer;
        
        // Create troop count text
        CreateTroopCountText(territory, territoryObj);
    }
    
    /// <summary>
    /// Creates a simple polygon sprite for territories
    /// </summary>
    Sprite CreateTerritorySprite()
    {
        // Create a simple circle texture
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                if (distance <= radius)
                {
                    // Create a border effect
                    if (distance >= radius - 2f)
                    {
                        texture.SetPixel(x, y, Color.black);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.white);
                    }
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    /// <summary>
    /// Creates the troop count text for a territory
    /// </summary>
    void CreateTroopCountText(Territory territory, GameObject territoryObj)
    {
        // Create a canvas for the text if it doesn't exist
        Canvas canvas = territoryObj.GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("TerritoryCanvas");
            canvasObj.transform.SetParent(territoryObj.transform);
            canvasObj.transform.localPosition = Vector3.zero;
            
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            
            // Add CanvasScaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Add GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Create or find the text component
        Text troopText = canvas.GetComponentInChildren<Text>();
        if (troopText == null)
        {
            GameObject textObj = new GameObject("TroopCount");
            textObj.transform.SetParent(canvas.transform);
            textObj.transform.localPosition = Vector3.zero;
            
            troopText = textObj.AddComponent<Text>();
            troopText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            troopText.fontSize = 12;
            troopText.alignment = TextAnchor.MiddleCenter;
            troopText.color = Color.white;
            
            // Add outline for better visibility
            Outline outline = textObj.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(1f, 1f);
        }
        
        territory.troopCountText = troopText;
    }
    
    /// <summary>
    /// Clears all loaded territories
    /// </summary>
    void ClearMap()
    {
        foreach (Territory territory in loadedTerritories)
        {
            if (territory != null)
            {
                DestroyImmediate(territory.gameObject);
            }
        }
        
        loadedTerritories.Clear();
    }
    
    /// <summary>
    /// Loads a different map
    /// </summary>
    public void LoadMap(TerritoryData newTerritoryData)
    {
        territoryData = newTerritoryData;
        LoadMap();
    }
    
    /// <summary>
    /// Creates a placeholder for future hand-drawn map integration
    /// </summary>
    [ContextMenu("Load Hand-Drawn Map Placeholder")]
    public void LoadHandDrawnMapPlaceholder()
    {
        Debug.Log("Hand-drawn map loading placeholder - replace with actual map texture loading");
        // TODO: Implement hand-drawn map loading
        // This would load a Game of Thrones-style hand-drawn fantasy map texture
        // and potentially adjust territory positions to match the map
    }
    
    void OnDrawGizmos()
    {
        if (loadedTerritories.Count > 0)
        {
            // Draw territory connections
            Gizmos.color = Color.yellow;
            foreach (Territory territory in loadedTerritories)
            {
                foreach (TerritoryConnection connection in territory.connections)
                {
                    if (connection.connectedTerritory != null)
                    {
                        Gizmos.DrawLine(territory.transform.position, connection.connectedTerritory.transform.position);
                    }
                }
            }
        }
    }
}
