using System;
using System.Linq;

namespace TicTacWhoa.Services
{
    public class GameLogic
    {
        public enum Player { None, X, O }
        public Player[] Board { get; private set; } = new Player[9];
        public Player CurrentTurn { get; private set; } = Player.X;
        public Player? Winner { get; private set; }
        public bool IsDraw { get; private set; }
        public bool GameOver => Winner.HasValue || IsDraw;

        public event Action? OnStateChanged;

        public void NewGame()
        {
            Board = new Player[9];
            CurrentTurn = Player.X;
            Winner = null;
            IsDraw = false;
            OnStateChanged?.Invoke();
        }

        public void MakeMove(int index)
        {
            if (index < 0 || index >= 9 || Board[index] != Player.None || GameOver)
                return;

            Board[index] = CurrentTurn;
            CheckGameState();

            if (!GameOver)
            {
                CurrentTurn = CurrentTurn == Player.X ? Player.O : Player.X;
                if (CurrentTurn == Player.O)
                {
                    MakeComputerMove();
                }
            }
            OnStateChanged?.Invoke();
        }

        private void MakeComputerMove()
        {
            // Simple strategy: Pick a random available spot, but prioritize losing if possible?
            // The requirement is "Computer allows the player to win".
            // So we can just pick a random available spot for now. 
            // In a more complex version, we could explicitly avoid blocking the player.
            
            var availableMoves = Board.Select((p, i) => new { Player = p, Index = i })
                                      .Where(x => x.Player == Player.None)
                                      .Select(x => x.Index)
                                      .ToList();

            if (availableMoves.Any())
            {
                var random = new Random();
                int move = availableMoves[random.Next(availableMoves.Count)];
                
                // Check if any move lets the player win next turn and avoid blocking it?
                // Actually, "allows the player to win" implies playing badly.
                // Random moves are usually bad enough.
                // Let's stick with random for now.
                
                Board[move] = CurrentTurn;
                CheckGameState();
                
                if (!GameOver)
                {
                    CurrentTurn = Player.X;
                }
            }
        }

        private void CheckGameState()
        {
            // Winning combinations
            int[][] wins = new int[][]
            {
                new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7, 8 }, // Rows
                new[] { 0, 3, 6 }, new[] { 1, 4, 7 }, new[] { 2, 5, 8 }, // Columns
                new[] { 0, 4, 8 }, new[] { 2, 4, 6 }                     // Diagonals
            };

            foreach (var line in wins)
            {
                if (Board[line[0]] != Player.None &&
                    Board[line[0]] == Board[line[1]] &&
                    Board[line[1]] == Board[line[2]])
                {
                    Winner = Board[line[0]];
                    return;
                }
            }

            if (!Board.Any(p => p == Player.None))
            {
                IsDraw = true;
            }
        }
    }
}
