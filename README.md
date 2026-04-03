# 🚀 Battle Tank Game (C# Console)

## 🎮 About

**Battle Tank Game** is a console-based game built with C# where players control a tank to battle against AI-controlled enemies. The game features real-time movement, shooting mechanics, collision detection, and simple AI behavior — all rendered directly in the terminal.

This project demonstrates core game development concepts, object-oriented programming (OOP), and problem-solving within a text-based environment.

---

## 🧩 Features

* 🎯 Player-controlled tank using keyboard input
* 🤖 AI enemy tanks with basic targeting behavior
* 💥 Shooting system with directional bullets
* 🧱 Collision detection:

  * Walls and map boundaries
  * Other tanks
* 💣 Explosion animation system
* 🗺️ Custom ASCII map rendering
* 🔄 Real-time game loop

---

## 🎮 Controls

| Key | Action      |
| --- | ----------- |
| W   | Move Up     |
| S   | Move Down   |
| A   | Move Left   |
| D   | Move Right  |
| ↑   | Shoot Up    |
| ↓   | Shoot Down  |
| ←   | Shoot Left  |
| →   | Shoot Right |
| ESC | Exit Game   |

---

## 🛠️ Technologies Used

* C# (.NET)
* Console / Terminal Rendering
* Object-Oriented Programming (OOP)

---

## 📂 Project Structure

```
Battle_Tank_Game/
│
├── Program.cs        # Main game loop and setup
├── Tank.cs           # Tank behavior, movement, shooting
├── Bullet.cs         # Bullet logic
└── Assets (inline)   # ASCII sprites & map
```

---

## 🧠 Key Concepts Demonstrated

* Game loop design
* Real-time input handling
* Collision detection systems
* State management (alive, exploding, destroyed)
* Basic AI decision making
* Console rendering optimization

---

## ▶️ How to Run

1. Make sure you have **.NET SDK** installed
2. Clone this repository:

   ```bash
   git clone https://github.com/yourusername/battle-tank-game.git
   ```
3. Navigate to the project folder:

   ```bash
   cd battle-tank-game
   ```
4. Run the game:

   ```bash
   dotnet run
   ```

---

## 🖼️ Gameplay Preview

```
╔═════════════════════════════════════════════════════════════════════════╗
║                                                                         ║
║        [PLAYER]                         [ENEMY]                         ║
║                                                                         ║
║               ^ (bullet traveling)                                      ║
║                                                                         ║
╚═════════════════════════════════════════════════════════════════════════╝
```

---

## 💡 Future Improvements

* 🧠 Smarter AI (pathfinding, dodging)
* ❤️ Health bar UI
* 🔊 Sound effects
* 🧱 Destructible environment
* 🕹️ Multiplayer mode
* 🎨 Improved graphics (colors)

---

## 📜 License

This project is open-source and available under the MIT License.

---

## 🙌 Acknowledgments

Inspired by classic retro tank games and built as a learning project for mastering C# and game logic.
