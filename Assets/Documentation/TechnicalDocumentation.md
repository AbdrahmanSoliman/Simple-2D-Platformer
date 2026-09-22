# Simple 2D Platformer

## Overview

This project is a small 2D platformer made with Unity 6+. The player can move, jump, collect coins, defeat enemies, activate checkpoints, and reach the end of the level. The focus of the implementation is clear responsibilities between systems and a simple save flow that is easy to follow and extend.

## Running the Project

Open the project in Unity 6000.3.11f or a newer compatible version. Open the main menu scene and press Play. The main menu provides options to start a new game, continue from the latest checkpoint, or quit.

## Controls

- Move: `A` / `D` or the left and right arrow keys
- Jump: `Space`

## Main Systems

- `PlayerController` handles movement, jumping, ground detection, bouncing, and respawning.
- `PlayerHealth` manages hit points, damage, invincibility frames, and death events.
- `EnemyPatroller` and `EnemyChaser` implement the shared `IEnemy` contract. The chaser uses a small patrol/chase state machine and has an adjustable stopping distance.
- `TopHitbox` handles enemy stomps separately from side collisions.
- `CoinTracker` and `DefeatedEnemyTracker` listen for gameplay events and keep track of collected or defeated objects.
- `CheckpointManager` stores the latest respawn point and updates checkpoint visuals. Checkpoints already passed remain green when the game is loaded.
- `GameManager` controls scene flow, respawning, new games, continuing, and level completion.
- `HudUI` displays health and coin information through events rather than polling.
- `FadeObject` provides the black screen fade used when entering the game.

## Save and Load

Progress is saved when the player reaches a new checkpoint. The save file is written as JSON to Unity's `Application.persistentDataPath` and contains:

- Player health and position
- Collected coin IDs and coin count
- Defeated enemy IDs
- Latest checkpoint ID and position
- Level completion state

The project uses stable integer IDs for coins and enemies. For this fixed, single-level game, integer IDs are a better fit than GUIDs: they are easy to assign and inspect in the Unity Inspector, serialize directly to the simple JSON save format, and avoid the extra generation and validation logic that GUIDs would require. This keeps the save data readable and avoids depending on object names or hierarchy positions.

## Implementation Notes

The New Input System is used through its generated C# wrapper. Movement and jump actions are handled through input callbacks, while gameplay systems communicate through C# events. This keeps UI and managers from needing to know the internal details of player, coin, enemy, or checkpoint components.

The player is teleported back to the current checkpoint after death instead of reloading the scene. This preserves collected coins and defeated enemies during the current play session. Loading a saved game restores the state recorded at the last checkpoint.

The project deliberately uses small local solutions instead of large frameworks. The chaser only needs two states, and the save data is simple enough for Unity's built-in `JsonUtility`.
