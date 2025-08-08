# Conquest of the Seven Kingdoms - Setup Guide

## Overview
A medieval territory conquest game where players command armies to expand their realm against AI opponents. Features polygonal territories, troop production, strategic army movement, and intelligent AI factions.

## Quick Start
1. **Open Unity 2022.3 LTS or newer**
2. **Create new 2D project**
3. **Import all scripts to Assets/Scripts/**
4. **Follow setup steps below**

## Project Structure
```
Assets/
├── Scripts/
│   ├── Territory.cs           # Territory management
│   ├── ArmyMovement.cs        # Army travel and combat
│   ├── GameManager.cs         # Main game logic
│   ├── AIController.cs        # AI decision making
│   ├── MapLoader.cs           # Map loading system
│   ├── CameraController.cs    # 2D camera controls
│   └── UIController.cs        # Medieval UI system
├── ScriptableObjects/
│   └── TerritoryData.cs       # Map configuration
├── Prefabs/
│   ├── TerritoryPrefab.prefab # Territory template
│   └── ArmyPrefab.prefab      # Army template
└── Scenes/
    └── MainGame.unity         # Main game scene
```

## Setup Steps

### 1. Create Folder Structure
Create these folders in your Unity project:
- `Assets/Scripts/`
- `Assets/ScriptableObjects/`
- `Assets/Prefabs/`
- `Assets/Scenes/`
- `Assets/Art/`

### 2. Import Scripts
Copy all `.cs` files to `Assets/Scripts/` folder.

### 3. Set Up Main Scene
1. **Create GameManager GameObject:**
   - Add empty GameObject named "GameManager"
   - Add `GameManager` script component
   - Add `AIController` script component

2. **Set Up Camera:**
   - Select Main Camera
   - Add `CameraController` script component
   - Set camera to orthographic mode

3. **Create MapLoader:**
   - Add empty GameObject named "MapLoader"
   - Add `MapLoader` script component

### 4. Create TerritoryData
1. **Right-click in Project window**
2. **Create → Conquest → Territory Data**
3. **Select the created asset**
4. **In Inspector, click "Create Sample Map"**
5. **Assign to MapLoader's Territory Data field**

### 5. Create Prefabs
1. **Territory Prefab:**
   - Create empty GameObject
   - Add `Territory` script
   - Add `SpriteRenderer` component
   - Add `BoxCollider2D` component
   - Drag to Prefabs folder

2. **Army Prefab:**
   - Create empty GameObject
   - Add `ArmyMovement` script
   - Add `SpriteRenderer` component
   - Set scale to (0.5, 0.5, 1)
   - Drag to Prefabs folder

### 6. Configure MapLoader
Assign the created prefabs to MapLoader component:
- Territory Prefab
- Army Prefab
- Territory Data (created in step 4)

### 7. Set Up UI (Optional)
1. **Create Canvas**
2. **Add UI elements:**
   - Game info text
   - Selected territory info
   - Pause button
   - Game speed slider
   - AI settings sliders
3. **Add UIController script to Canvas**
4. **Connect UI elements in Inspector**

## Game Features

### Core Mechanics
- **Territory Management:** 5 territories with troop counts
- **Army Movement:** Click-to-send armies between connected territories
- **Combat System:** Automatic battles when armies arrive
- **Troop Production:** Territories generate troops over time
- **AI Opponents:** 2 AI factions with adjustable difficulty

### Controls
- **Left Click:** Select territory, send army
- **Right Click + Drag:** Pan camera
- **Mouse Wheel:** Zoom in/out
- **WASD:** Pan camera
- **+/-:** Zoom in/out
- **Space:** Reset camera view
- **Escape:** Pause menu

### AI Settings
- **Aggression Level:** How often AI attacks (0-1)
- **Decision Speed:** How often AI makes decisions (0.5-5 seconds)

## Customization

### Adding New Territories
1. **Edit TerritoryData asset**
2. **Add new TerritoryInfo entries**
3. **Set positions, connections, and starting data**
4. **Click "Create Sample Map" to reset to default**

### Changing Map
1. **Create new TerritoryData asset**
2. **Design territory layout**
3. **Assign to MapLoader**
4. **Game automatically loads new map**

### Modifying AI Behavior
- **Edit AIController.cs** for strategy changes
- **Adjust weights** for attack/defend/reinforce
- **Modify decision logic** in MakeAIDecision method

### Visual Customization
- **Replace territory sprites** in TerritoryPrefab
- **Change faction colors** in Territory script
- **Modify army appearance** in ArmyPrefab
- **Add background textures** to TerritoryData

## Troubleshooting

### Common Issues
1. **Scripts not found:** Ensure all scripts are in Assets/Scripts/
2. **Prefabs missing:** Create and assign prefabs to MapLoader
3. **Territories not appearing:** Check TerritoryData configuration
4. **AI not working:** Verify AIController is attached to GameManager

### Debug Tips
- **Check Console** for error messages
- **Verify component assignments** in Inspector
- **Test with sample map** first
- **Use Unity's Debug.Log** for troubleshooting

## Expansion Ideas

### Future Features
- **More unit types** (cavalry, archers, siege)
- **Territory bonuses** (mines, farms, castles)
- **Diplomacy system** (alliances, trade)
- **Weather effects** (snow, rain affecting movement)
- **Hero units** with special abilities
- **Campaign mode** with multiple maps

### Technical Improvements
- **Save/load system** for game state
- **Network multiplayer** support
- **Advanced AI** with machine learning
- **Particle effects** for battles
- **Sound effects** and music
- **Mobile touch controls**

## Credits
This game prototype demonstrates:
- Modular Unity architecture
- ScriptableObject data management
- AI decision-making systems
- 2D game development patterns
- Medieval fantasy theming

Perfect foundation for expanding into a full strategy game!
