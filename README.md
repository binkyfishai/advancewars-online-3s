![image](https://github.com/user-attachments/assets/a97018d5-0b7e-4187-8d67-f7d9ee221024)


# Advance Wars Online 3S

A Unity-based recreation of the classic Advance Wars strategy game, featuring turn-based tactical combat, multiple unit types, and a complete battle system.

## 🎮 Features

### ✅ Core Gameplay
- **Turn-based Strategy**: Classic Advance Wars gameplay mechanics
- **Grid-based Movement**: Strategic unit positioning on a 20x20 grid
- **Complete Combat System**: Damage calculations with terrain bonuses, health modifiers, and counter-attacks
- **Multiple Unit Types**: Infantry, Tanks, Artillery, and more with unique stats and abilities
- **Faction System**: Multiple armies (Orange Star, Blue Moon, etc.) with distinct visual styles

### ✅ Combat Mechanics
- **Damage Tables**: Authentic unit vs unit damage calculations
- **Terrain Defense**: Mountains, forests, and cities provide defensive bonuses
- **Health Impact**: Damaged units deal reduced damage (10 HP = 100% damage, 1 HP = 10% damage)
- **Counter-attacks**: Defending units can strike back when in range
- **Ammo System**: Units have limited ammunition for attacks
- **Random Variance**: ±10% damage variation for realistic combat

### ✅ Visual & Audio
- **3D Models**: High-quality unit and terrain models
- **Multiple Terrains**: Normal, Desert, Snow, and Wasteland environments
- **Faction Buildings**: Army-specific structures (airports, factories, cities)
- **Unity URP**: Modern rendering pipeline for optimized performance

## 🛠️ Technical Stack

- **Engine**: Unity 2022.3+ LTS
- **Rendering**: Universal Render Pipeline (URP)
- **Platform**: Windows/WebGL ready
- **Architecture**: Component-based design with ScriptableObjects for data

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # GameManager, InputManager, combat logic
│   ├── Data/           # ScriptableObjects for units, terrain, factions
│   ├── Grid/           # Grid system and tile management
│   ├── Units/          # Unit behavior and management
│   └── Combat/         # Combat calculations and systems
├── Models/
│   ├── units/          # 3D unit models for all factions
│   ├── terrain/        # Terrain tiles and buildings
│   └── buildings/      # Faction-specific structures
├── Materials/          # Unit and terrain materials
└── Scenes/
    └── GameScene       # Main gameplay scene
```

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or newer
- Git for version control

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/binkyfishai/advancewars-online-3s.git
   ```

2. Open Unity Hub and add the project folder

3. Open the project in Unity

4. Open the `GameScene` in the Scenes folder

5. Press Play to start the game

### Quick Setup
- The game automatically generates a 20x20 grid of terrain tiles
- Test units are spawned at startup for immediate gameplay testing
- Click on units to select them, then click on tiles to move or attack

## 🎯 How to Play

### Basic Controls
- **Left Click**: Select units or tiles
- **Unit Selection**: Click on your units to select them
- **Movement**: Click on empty tiles within range to move selected units
- **Combat**: Click on enemy units within attack range to engage in battle

### Game Flow
1. **Select a Unit**: Click on one of your units (Orange Star faction)
2. **Move or Attack**: 
   - Click on highlighted tiles to move
   - Click on enemy units to attack (if in range and have ammo)
3. **Turn Management**: Each unit can move and attack once per turn
4. **Strategic Combat**: Use terrain for defense bonuses and plan unit positioning

### Combat Tips
- **High Ground Advantage**: Mountains provide +4 defense, forests +2
- **Health Matters**: Damaged units deal proportionally less damage
- **Range Considerations**: Different units have different attack ranges
- **Ammo Management**: Units have limited ammunition - use it wisely

## 🔧 Development

### Key Components

#### GameManager
- Central game state management
- Turn handling and player switching
- Unit spawning and selection logic

#### CombatCalculator
- Damage calculation algorithms
- Terrain bonus calculations
- Health impact modifiers
- Random variance application

#### GridManager
- 20x20 grid generation and management
- Tile positioning and reference system
- Pathfinding and movement validation

#### UnitManager
- Unit lifecycle management
- Unit data and stats handling
- Movement and action coordination

### Data Architecture
- **UnitData**: ScriptableObjects defining unit stats, movement, and combat values
- **TerrainData**: Tile types, movement costs, and defense bonuses
- **FactionData**: Army colors, building styles, and unit variants

## 🎨 Asset Credits

### Models & Textures
- Unit models: Custom 3D models for authentic Advance Wars look
- Terrain tiles: Multiple environment variants (Normal, Desert, Snow, Wasteland)
- Buildings: Faction-specific structures for each army

### Factions Available
- **Orange Star** (Player default)
- **Blue Moon** (Enemy faction)
- **Green Earth**
- **Yellow Comet**
- **Red Tower**
- **Purple Lightning**
- **Teal Galaxy**
- **Silver Claw**
- **Umber Wilds**
- **White Nova**

## 🚧 Roadmap

### Planned Features
- [ ] **Online Multiplayer**: Real-time multiplayer battles
- [ ] **Campaign Mode**: Single-player story missions
- [ ] **Map Editor**: Custom battlefield creation
- [ ] **CO Powers**: Special commander abilities
- [ ] **More Unit Types**: Naval and air units
- [ ] **Weather System**: Rain, snow affecting gameplay
- [ ] **Fog of War**: Limited visibility mechanics
- [ ] **Unit Production**: Factory and airport unit creation

### Technical Improvements
- [ ] **WebGL Optimization**: Enhanced web performance
- [ ] **Mobile Support**: Touch controls and UI scaling
- [ ] **Save System**: Game state persistence
- [ ] **AI Opponents**: Computer-controlled enemies
- [ ] **Animation System**: Unit movement and combat animations

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📜 License

This project is a fan recreation of Advance Wars for educational purposes. All original game concepts belong to Nintendo/Intelligent Systems.

## 🙏 Acknowledgments

- **Nintendo/Intelligent Systems** for the original Advance Wars series
- **Unity Technologies** for the game engine
- **Nova-Odos** for the video game models

---

## 🎮 Current Status: **Playable Alpha**

The core gameplay loop is complete and functional:
- ✅ Unit selection and movement
- ✅ Combat system with damage calculations
- ✅ Turn-based mechanics
- ✅ Terrain interactions
- ✅ Basic AI opponent units

**Ready to play and test!** 🚀 
