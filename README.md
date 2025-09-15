## Centipede Game

A recreation of the classic *Centipede* arcade game originally published by Atari, Inc. in 1981.

### Game Control

- `W`, `A`, `S`, `D` on keyboard to move.
- `Space`, on keyboard to shoot bullet.
- `ESC` on the keyboard to toggle the Pause screen.

### Gameplay Mechanics

- Initial State:
  - The *centipede* spawns at the top-left of the grid.
  - The *player* spawns at the bottom-center.
  - The *spider* spawns either at the center or a random location.
  - *Mushrooms* spawn randomly across the grid, leaving a gap at the bottom row.

- Centipede:
  - Moves horizontally across the grid.
  - When hitting a *mushroom* or grid edge, it will move down one cell and reverses direction.
  - When blocked, the centipede will ignore obstacles like *mushrooms* or its own segments and continue moving.

- Spider:
  - Actively chases the player while avoids *mushrooms* using *A\* pathfinding*.

- Player:
  - Can move from left and right across the entire board up to 15% of the grid height from the bottom.
  - Player can shoot bullet to destroy *mushrooms*, *centipedes*, and the *spider*.
  - Destroying a *centipede* segment causes it to split and leave a mushroom behind.
  - When player losing a life, all enemies will reset to their initial state.

- Game Over Conditions:
  - Game Over can happen when either All *centipede* segments are destroyed or player loses all lives.
  - In *Endless Mode*, the game continues as long as the player has lives remaining.

*Note: Game settings can be configured via the `GameManager`*

*Note: The game must be start from the `GameScene`*

*Note: This project is developed using Unity LTS 6000.0.57f1.*

---
