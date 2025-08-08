# Conquest of the Seven Kingdoms

A medieval territory conquest game built in Unity 2D where players command armies to expand their realm.

## Features
- **Territory Management**: Polygonal territories with troop counts and faction ownership
- **Army Movement**: Click-to-send armies between connected territories
- **Combat System**: Automatic battles when armies reach enemy territories
- **AI Opponents**: Intelligent AI factions with adjustable difficulty
- **Medieval Aesthetic**: Fantasy-themed UI and visual design
- **Modular Design**: Easy to expand with new maps, factions, and unit types

## Game Mechanics
- Territories produce troops over time
- Armies travel between connected territories
- Enemy territory = battle for control
- Friendly territory = reinforcement
- AI factions make strategic decisions automatically

## Technical Requirements
- Unity 2022.3 LTS or newer
- 2D rendering pipeline
- C# scripting

## Setup Instructions
1. Open the project in Unity
2. Open the main scene: `Assets/Scenes/MainGame.unity`
3. Press Play to start the game
4. Click on your territories (blue) then connected territories to send armies

## Project Structure
- `Scripts/` - Core game logic and systems
- `ScriptableObjects/` - Territory and game data
- `Prefabs/` - Reusable game objects
- `Scenes/` - Game scenes
- `Art/` - Placeholder art and textures
