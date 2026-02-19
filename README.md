# Tic-Tac-Whoa! 🎮

**Tic-Tac-Whoa!** is a modern, level-based twist on the classic Tic-Tac-Toe game, built with **Blazor**. Progress through increasingly difficult levels with unique rules, multiple opponents, and dynamic board mechanics.

## ✨ Key Features

- **Level-Based Progression**: Advance through levels that introduce new challenges.
- **Dynamic Grid Sizes**: Play on boards ranging from the classic 3x3 to 5x5 and potentially larger.
- **Multiple Opponents**: Compete against one or more AI opponents with different behaviors.
- **Special Game Rules**:
  - ⏳ **Timer Mode**: Make your move before the clock runs out!
  - ⚓ **Gravity Mode**: Pieces fall to the bottom of the column (Connect 4 style).
  - 👻 **Disappearing Pieces**: Only your most recent moves stay on the board.
- **Win Conditions**: Longer boards require longer lines to win (e.g., 4-in-a-row on 4x4).
- **Interactive UI**: Smooth animations and celebratory confetti when you win!

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later.

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/stereoa/TicTacWhoa.git
   cd TicTacWhoa
   ```

2. Run the application:
   ```bash
   dotnet watch run
   ```

3. Open your browser and navigate to `http://localhost:5000` (or the port specified in your terminal).

## 🛠️ Built With

- **[ASP.NET Core Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-8.0)** - UI framework.
- **C#** - Game logic and backend services.
- **HTML/CSS** - Styling and layout.

## 📂 Project Structure

- `Pages/`: Razor components for the game UI (`TicTacToe.razor`).
- `Services/`: Core game logic (`GameLogic.cs`) and level definitions (`LevelRepository.cs`).
- `Models/`: Data structures for levels and opponents.
- `wwwroot/`: Static assets including CSS and JavaScript for special effects like confetti.

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
