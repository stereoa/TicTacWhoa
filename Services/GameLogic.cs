using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TicTacWhoa.Models;
using Timer = System.Timers.Timer;

namespace TicTacWhoa.Services
{
    public class GameLogic : IDisposable
    {
        private readonly LevelRepository _levelRepository;
        private Timer? _turnTimer;

        public class PlayerInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Symbol { get; set; } = string.Empty;
            public bool IsHuman { get; set; }
            public OpponentConfig? Config { get; set; }
        }

        public int CurrentLevelNumber { get; private set; } = 1;
        public LevelConfig CurrentLevelConfig { get; private set; } = new LevelConfig();

        public PlayerInfo[] Players { get; private set; } = Array.Empty<PlayerInfo>();
        public int CurrentPlayerIndex { get; private set; }
        public PlayerInfo CurrentPlayer => Players[CurrentPlayerIndex];

        // Board stores the index of the player who owns the cell, or -1 if empty
        public int[] Board { get; private set; } = Array.Empty<int>();

        public PlayerInfo? Winner { get; private set; }
        public bool IsDraw { get; private set; }
        public bool GameOver => Winner != null || IsDraw;
        public string GameOverReason { get; private set; } = string.Empty;

        public double TimeRemaining { get; private set; }

        private Queue<(int PlayerIndex, int BoardIndex)> _moveHistory = new Queue<(int, int)>();

        public event Action? OnStateChanged;

        public GameLogic(LevelRepository levelRepository)
        {
            _levelRepository = levelRepository;
        }

        public void NewGame(int? level = null)
        {
            if (level.HasValue)
            {
                CurrentLevelNumber = level.Value;
            }

            CurrentLevelConfig = _levelRepository.GetLevel(CurrentLevelNumber);

            InitializePlayers();
            InitializeBoard();

            Winner = null;
            IsDraw = false;
            GameOverReason = string.Empty;
            CurrentPlayerIndex = 0;
            _moveHistory.Clear();

            SetupTimer();
            StartTurn();

            OnStateChanged?.Invoke();
        }

        public void InitializePlayers()
        {
            var playerList = new List<PlayerInfo>
            {
                new PlayerInfo { Name = "You", Symbol = "X", IsHuman = true }
            };

            foreach (var opponent in CurrentLevelConfig.Opponents)
            {
                playerList.Add(new PlayerInfo
                {
                    Name = opponent.Name,
                    Symbol = opponent.Symbol,
                    IsHuman = false,
                    Config = opponent
                });
            }

            Players = playerList.ToArray();
        }

        private void InitializeBoard()
        {
            int totalCells = CurrentLevelConfig.GridSize * CurrentLevelConfig.GridSize;
            Board = Enumerable.Repeat(-1, totalCells).ToArray();
        }

        private void SetupTimer()
        {
            _turnTimer?.Dispose();
            if (CurrentLevelConfig.TurnTimeLimit.HasValue)
            {
                _turnTimer = new Timer(100);
                _turnTimer.Elapsed += OnTimerTick;
                _turnTimer.AutoReset = true;
            }
            else
            {
                _turnTimer = null;
            }
        }

        private void StartTurn()
        {
            if (GameOver) return;

            if (_turnTimer != null && CurrentLevelConfig.TurnTimeLimit.HasValue)
            {
                TimeRemaining = CurrentLevelConfig.TurnTimeLimit.Value.TotalSeconds;
                _turnTimer.Start();
            }

            if (!CurrentPlayer.IsHuman)
            {
                _ = MakeComputerMove();
            }

            OnStateChanged?.Invoke();
        }

        private void OnTimerTick(object? sender, ElapsedEventArgs e)
        {
            TimeRemaining -= 0.1;
            if (TimeRemaining <= 0)
            {
                _turnTimer?.Stop();
                TimeRemaining = 0;
                HandleTimeout();
            }
            OnStateChanged?.Invoke();
        }

        private void HandleTimeout()
        {
            GameOverReason = $"{CurrentPlayer.Name} timed out!";

            if (CurrentPlayer.IsHuman)
            {
                // Human lost
                Winner = Players.FirstOrDefault(p => !p.IsHuman); // Winner is first opponent
            }
            else
            {
                // Computer lost -> Human wins
                Winner = Players.First(p => p.IsHuman);
            }

            OnStateChanged?.Invoke();
        }

        public async Task MakeMove(int index)
        {
            if (GameOver || index < 0 || index >= Board.Length) return;

            int targetIndex = index;

            if (CurrentLevelConfig.Rule == SpecialRule.Gravity)
            {
                // Find lowest empty slot in the column
                int gridSize = CurrentLevelConfig.GridSize;
                int col = index % gridSize;
                // Start from bottom row
                for (int row = gridSize - 1; row >= 0; row--)
                {
                    int idx = row * gridSize + col;
                    if (Board[idx] == -1)
                    {
                        targetIndex = idx;
                        break;
                    }
                    if (row == 0) return; // Column full
                }
            }
            else
            {
                // Standard rule validation
                if (Board[targetIndex] != -1) return;
            }

            // Execute Move
            _turnTimer?.Stop();

            // Disappearing Pieces Rule
            if (CurrentLevelConfig.Rule == SpecialRule.DisappearingPieces)
            {
                int maxPieces = 3;
                var playerMoves = _moveHistory.Where(m => m.PlayerIndex == CurrentPlayerIndex).ToList();
                if (playerMoves.Count >= maxPieces)
                {
                    var oldestMove = _moveHistory.FirstOrDefault(m => m.PlayerIndex == CurrentPlayerIndex);

                    if (Board[oldestMove.BoardIndex] == CurrentPlayerIndex)
                    {
                        Board[oldestMove.BoardIndex] = -1;
                    }

                    _moveHistory = new Queue<(int, int)>(_moveHistory.Where(x => x != oldestMove));
                }
            }

            Board[targetIndex] = CurrentPlayerIndex;
            _moveHistory.Enqueue((CurrentPlayerIndex, targetIndex));

            CheckGameState();

            if (!GameOver)
            {
                CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Length;
                StartTurn();
            }
            else if (Winner?.IsHuman == true)
            {
                // Level Up Logic handled by UI
            }

            OnStateChanged?.Invoke();
        }

        private async Task MakeComputerMove()
        {
            if (CurrentPlayer.IsHuman) return;

            var config = CurrentPlayer.Config;
            int delay = config?.MoveDelayMs ?? 1000;

            // Simulate thinking
            await Task.Delay(delay);

            if (GameOver) return;

            // Timeout Chance
            if (config != null && config.TimeoutChance > 0)
            {
                var random = new Random();
                if (random.NextDouble() < config.TimeoutChance)
                {
                    // Intentionally miss turn (wait for timeout)
                    return;
                }
            }

            // AI Logic: Pick Random Valid Move
            var availableMoves = new List<int>();
            int gridSize = CurrentLevelConfig.GridSize;

            if (CurrentLevelConfig.Rule == SpecialRule.Gravity)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    // Check if top cell is empty as a proxy for available column
                    if (Board[c] == -1)
                    {
                        availableMoves.Add(c);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Board.Length; i++)
                {
                    if (Board[i] == -1) availableMoves.Add(i);
                }
            }

            if (availableMoves.Any())
            {
                var random = new Random();
                int moveIndex = availableMoves[random.Next(availableMoves.Count)];
                await MakeMove(moveIndex);
            }
            else
            {
                IsDraw = true;
                GameOverReason = "Stalemate";
                _turnTimer?.Stop();
                OnStateChanged?.Invoke();
            }
        }

        private void CheckGameState()
        {
            int gridSize = CurrentLevelConfig.GridSize;
            int winLength = CurrentLevelConfig.WinConditionLength;

            int GetCell(int r, int c)
            {
                if (r < 0 || r >= gridSize || c < 0 || c >= gridSize) return -2;
                return Board[r * gridSize + c];
            }

            // Check Directions: Horizontal, Vertical, Diagonal (TopLeft-BotRight), Diagonal (TopRight-BotLeft)
            int[][] directions = new int[][]
            {
                new[] { 0, 1 },  // Right
                new[] { 1, 0 },  // Down
                new[] { 1, 1 },  // Diagonal Down-Right
                new[] { 1, -1 }  // Diagonal Down-Left
            };

            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    int currentPlayer = GetCell(r, c);
                    if (currentPlayer == -1) continue;

                    foreach (var dir in directions)
                    {
                        int dr = dir[0];
                        int dc = dir[1];

                        bool win = true;
                        for (int k = 1; k < winLength; k++)
                        {
                            if (GetCell(r + dr * k, c + dc * k) != currentPlayer)
                            {
                                win = false;
                                break;
                            }
                        }

                        if (win)
                        {
                            Winner = Players[currentPlayer];
                            GameOverReason = $"{Winner.Name} Win!";
                            _turnTimer?.Stop();
                            return;
                        }
                    }
                }
            }

            if (!Board.Contains(-1))
            {
                IsDraw = true;
                GameOverReason = "Draw!";
                _turnTimer?.Stop();
            }
        }

        public void Dispose()
        {
            _turnTimer?.Dispose();
        }
    }
}
