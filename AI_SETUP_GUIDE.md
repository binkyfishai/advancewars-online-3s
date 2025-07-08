# AI Setup Guide

This guide will help you set up the AI system in your Advance Wars Unity project.

## 🤖 Quick Setup (5 minutes)

### 1. **Add AI to Your Scene**
1. Create an empty GameObject in your scene
2. Name it "AI Manager"  
3. Add the `AIManager` component to it
4. The `AIController` will be automatically added

### 2. **Configure AI Players**
In the AI Manager component:
- **Use Custom Settings**: Check this for quick setup
- **Custom AI Player IDs**: Set to `[1]` (Player 1 = Blue Moon)
- **Custom Aggressiveness**: `0.7` (70% aggressive)

### 3. **Test the AI**
1. Run the game
2. End your turn (Spacebar) 
3. Watch the AI automatically control Player 1's units!

## ⚙️ Advanced Configuration

### Using AI Settings Assets

1. **Create AI Settings**:
   - Right-click in Project → Create → Advanced Wars → AI Settings
   - Name it "DefaultAISettings"

2. **Configure Settings**:
   ```
   AI Players: [1]
   Action Delay: 1.0s
   Turn End Delay: 2.0s
   Aggressiveness: 0.7
   Difficulty: Normal
   ```

3. **Assign to AI Manager**:
   - Uncheck "Use Custom Settings"
   - Drag your AI Settings asset to the "AI Settings" field

### AI Difficulty Levels

| Difficulty | Aggressiveness | Behavior |
|------------|----------------|----------|
| **Easy** | 0.6x | Defensive, slower decisions |
| **Normal** | 1.0x | Balanced play style |
| **Hard** | 1.2x | More aggressive attacks |
| **Expert** | 1.4x | Optimal strategic play |

## 🎮 How the AI Works

### AI Decision Making
1. **Attack Priority**: AI tries to attack enemies in range first
2. **Move Strategy**: Moves units closer to enemies
3. **Target Selection**: Prefers weak/valuable targets
4. **Defensive Play**: Uses terrain bonuses when available

### AI Behavior Settings

**Aggressiveness (0.0 - 1.0)**:
- `0.0`: Very defensive, stays back
- `0.5`: Balanced approach
- `1.0`: Very aggressive, charges forward

**Explore Factor (0.0 - 1.0)**:
- `0.0`: Focuses only on enemies
- `0.5`: Balanced exploration
- `1.0`: Explores map actively

## 🔧 Customization

### Adding More AI Players
```csharp
// Make Players 1, 2, and 3 AI-controlled
aiManager.AddAIPlayer(1); // Blue Moon
aiManager.AddAIPlayer(2); // Green Earth  
aiManager.AddAIPlayer(3); // Yellow Comet
```

### Runtime AI Control
```csharp
// Change difficulty during gameplay
aiManager.SetAIDifficulty(AIDifficulty.Hard);

// Adjust aggressiveness
aiManager.SetAIAggressiveness(0.9f);

// Remove AI control (make human-controlled)
aiManager.RemoveAIPlayer(1);
```

### Custom AI Logic
Extend `AIController.cs` to add:
- Building capture logic
- Unit production priorities  
- Economic management
- Specialized unit tactics

## 🐛 Troubleshooting

### AI Not Working?
1. **Check AI Manager**: Make sure AIManager component is in scene
2. **Player IDs**: Verify AI Player IDs match your game setup
3. **Unit Spawning**: Ensure AI players have units to control
4. **Console Logs**: Look for 🤖 AI messages in console

### AI Too Easy/Hard?
- Adjust **Aggressiveness** (0.1 = very easy, 1.0 = very hard)
- Change **Difficulty** setting
- Modify **Action Delay** for pacing

### AI Stuck?
- AI automatically ends turn after processing all units
- Check for null references in console
- Verify units have valid movement options

## 🎯 Game Balance Tips

### For Challenging AI:
```
Difficulty: Hard
Aggressiveness: 0.8-0.9
Action Delay: 0.5s (faster)
```

### For Casual Play:
```
Difficulty: Easy  
Aggressiveness: 0.4-0.6
Action Delay: 1.5s (slower)
```

### For Competitive:
```
Difficulty: Expert
Aggressiveness: 1.0
Action Delay: 0.3s (very fast)
```

## 📋 Debug Features

Enable debug logging to see AI thinking:
- **Enable Debug Logging**: See AI decisions
- **Show Thinking Process**: Detailed move evaluation

Console output examples:
```
🤖 AI Player 1 turn starting...
🤖 Infantry will attack Enemy Tank
🤖 Tank will move to (7,8)
🤖 AI Player 1 ending turn
```

## 🚀 Next Steps

Once basic AI works:
1. **Add building capture** logic
2. **Implement unit production** from factories
3. **Create specialized tactics** per unit type
4. **Add economic AI** for resource management
5. **Network multiplayer** with AI opponents

The AI system is designed to be easily extensible - start simple and add complexity as needed! 