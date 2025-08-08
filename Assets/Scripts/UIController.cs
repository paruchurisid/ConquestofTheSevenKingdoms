using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainHUD;
    public GameObject pauseMenu;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    
    [Header("HUD Elements")]
    public Text gameInfoText;
    public Text selectedTerritoryInfo;
    public Button pauseButton;
    public Text pauseButtonText;
    public Slider gameSpeedSlider;
    public Text gameSpeedText;
    
    [Header("AI Settings")]
    public Slider aiAggressionSlider;
    public Slider aiDecisionSpeedSlider;
    public Text aiAggressionText;
    public Text aiDecisionSpeedText;
    
    [Header("Medieval Theme")]
    public Color medievalGold = new Color(0.8f, 0.6f, 0.2f, 1f);
    public Color medievalBrown = new Color(0.4f, 0.2f, 0.1f, 1f);
    public Color medievalRed = new Color(0.7f, 0.2f, 0.2f, 1f);
    
    private GameManager gameManager;
    private AIController aiController;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        aiController = FindObjectOfType<AIController>();
        
        SetupUI();
        ApplyMedievalTheme();
    }
    
    void SetupUI()
    {
        // Connect UI elements to GameManager
        if (gameManager != null)
        {
            gameManager.gameInfoText = gameInfoText;
            gameManager.selectedTerritoryInfo = selectedTerritoryInfo;
            gameManager.pauseButton = pauseButton;
            gameManager.pauseButtonText = pauseButtonText;
        }
        
        // Setup sliders
        if (gameSpeedSlider != null)
        {
            gameSpeedSlider.minValue = 0.1f;
            gameSpeedSlider.maxValue = 3f;
            gameSpeedSlider.value = 1f;
            gameSpeedSlider.onValueChanged.AddListener(OnGameSpeedChanged);
        }
        
        if (aiAggressionSlider != null)
        {
            aiAggressionSlider.minValue = 0f;
            aiAggressionSlider.maxValue = 1f;
            aiAggressionSlider.value = 0.7f;
            aiAggressionSlider.onValueChanged.AddListener(OnAIAggressionChanged);
        }
        
        if (aiDecisionSpeedSlider != null)
        {
            aiDecisionSpeedSlider.minValue = 0.5f;
            aiDecisionSpeedSlider.maxValue = 5f;
            aiDecisionSpeedSlider.value = 3f;
            aiDecisionSpeedSlider.onValueChanged.AddListener(OnAIDecisionSpeedChanged);
        }
        
        // Setup pause button
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }
        
        // Hide panels initially
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }
    
    void ApplyMedievalTheme()
    {
        // Apply medieval colors to UI elements
        Image[] images = GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            if (img.name.Contains("Background"))
            {
                img.color = medievalBrown;
            }
            else if (img.name.Contains("Button"))
            {
                img.color = medievalGold;
            }
        }
        
        // Apply medieval font styling
        Text[] texts = GetComponentsInChildren<Text>();
        foreach (Text text in texts)
        {
            if (text.name.Contains("Title"))
            {
                text.color = medievalGold;
                text.fontStyle = FontStyle.Bold;
            }
            else if (text.name.Contains("Info"))
            {
                text.color = Color.white;
            }
        }
    }
    
    void OnGameSpeedChanged(float value)
    {
        if (gameManager != null)
        {
            gameManager.SetGameSpeed(value);
        }
        
        if (gameSpeedText != null)
        {
            gameSpeedText.text = $"Game Speed: {value:F1}x";
        }
    }
    
    void OnAIAggressionChanged(float value)
    {
        if (aiController != null)
        {
            aiController.SetAggressionLevel(value);
        }
        
        if (aiAggressionText != null)
        {
            aiAggressionText.text = $"AI Aggression: {value:F1}";
        }
    }
    
    void OnAIDecisionSpeedChanged(float value)
    {
        if (aiController != null)
        {
            aiController.SetDecisionInterval(value);
        }
        
        if (aiDecisionSpeedText != null)
        {
            aiDecisionSpeedText.text = $"AI Decision Speed: {value:F1}s";
        }
    }
    
    void OnPauseButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.TogglePause();
        }
        
        // Show/hide pause menu
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(gameManager.gamePaused);
        }
    }
    
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    
    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }
    
    public void RestartGame()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    void Update()
    {
        // Handle escape key for pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPauseButtonClicked();
        }
    }
}
