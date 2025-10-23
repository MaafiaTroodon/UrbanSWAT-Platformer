Individual Assignment
Game: 3D Platformer – SWAT vs Vampire
Author: Malhar Datta Mahajan
Net Id: B00934337

Overview:
This is a third-person 3D action platformer built in Unity. You play as a SWAT operative stranded on floating sky platforms, battling Vampire enemies. The player must survive, defeat all enemies, and reach the teleporter on the top platform to advance through levels. The game combines smooth movement, sprinting, jumping, combat, and a Minecraft-style sprint FOV effect for a fast-paced feel.

Features:
- Playable SWAT character with realistic animation blending
- Vampire enemies that chase and damage the player
- Health and lives system with automatic respawn
- Smooth FOV sprint boost effect when running
- Multi-platform vertical level design
- Dynamic camera follow system
- Two fully playable levels
- HUD showing player health and remaining lives

Objectives:
Level 1:
- Defeat all Vampire enemies patrolling the platforms.
- Climb to the top platform and enter the teleporter to complete the level.

Level 2:
- Progress to the upper arena.
- Defeat the final Vampire boss on the top platform to finish the game.

Controls:
W / A / S / D - Move
Spacebar - Jump
Left Shift - Sprint
Escape - Exit Game

Developer Notes:
- FOV smoothly expands from 60° to 74° when sprinting.
- Ground check and respawn systems ensure the player never falls through floating platforms.
- Enemies are assigned Health components and can be defeated.
- Respawn includes temporary invulnerability after death.

Technical Details:
- Built with Unity 2022.3 LTS
- Core scripts:
  * PlayerControl.cs (movement, sprint, FOV)
  * Health.cs (damage and HP system)
  * PlayerLives.cs (respawn and lives tracking)
  * EnemyAI.cs (enemy chase and attack)
- Two playable scenes:
  * Level1.unity
  * Level2.unity

Goal:
Survive, eliminate all Vampires, reach the top platform, and complete both levels to conquer the skies.

Good luck, Commander SWAT!
