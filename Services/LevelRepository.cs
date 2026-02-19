using System;
using System.Collections.Generic;
using TicTacWhoa.Models;

namespace TicTacWhoa.Services
{
    public class LevelRepository
    {
        private readonly Dictionary<int, LevelConfig> _levels = new Dictionary<int, LevelConfig>();

        public LevelRepository()
        {
            InitializeLevels();
        }

        private void InitializeLevels()
        {
            // Level 1: Standard 3x3
            _levels.Add(1, new LevelConfig
            {
                LevelNumber = 1,
                GridSize = 3,
                WinConditionLength = 3,
                Opponents = new List<OpponentConfig>
                {
                    new OpponentConfig { Name = "Computer", Symbol = "O" }
                }
            });

            // Level 2: 3x3 with 3s Timer
            _levels.Add(2, new LevelConfig
            {
                LevelNumber = 2,
                GridSize = 3,
                WinConditionLength = 3,
                TurnTimeLimit = TimeSpan.FromSeconds(3),
                Opponents = new List<OpponentConfig>
                {
                    new OpponentConfig { Name = "Speedy Bot", Symbol = "O", MoveDelayMs = 500 }
                }
            });

            // Level 3: 4x4, 2 Opponents
            _levels.Add(3, new LevelConfig
            {
                LevelNumber = 3,
                GridSize = 4,
                WinConditionLength = 4,
                Opponents = new List<OpponentConfig>
                {
                    new OpponentConfig { Name = "Bot A", Symbol = "O" },
                    new OpponentConfig { Name = "Bot B", Symbol = "Δ" }
                }
            });

            // Level 4: Gravity Mode
            _levels.Add(4, new LevelConfig
            {
                LevelNumber = 4,
                GridSize = 3, // Or 4? Let's stick to 3 for simplicity first, or 4 for better gravity feel. Let's do 4x4 for gravity.
                WinConditionLength = 4,
                Rule = SpecialRule.Gravity,
                Opponents = new List<OpponentConfig>
                {
                    new OpponentConfig { Name = "Newton", Symbol = "O" }
                }
            });

            // Level 5: Disappearing Pieces
            _levels.Add(5, new LevelConfig
            {
                LevelNumber = 5,
                GridSize = 3,
                WinConditionLength = 3,
                Rule = SpecialRule.DisappearingPieces,
                Opponents = new List<OpponentConfig>
                {
                    new OpponentConfig { Name = "Ghost", Symbol = "O" }
                }
            });

            // Level 6: 5x5, 4 Opponents, Gravity + Timer
            _levels.Add(6, new LevelConfig
            {
                LevelNumber = 6,
                GridSize = 5,
                WinConditionLength = 5, // Or 4? 5 is hard. Let's do 5.
                TurnTimeLimit = TimeSpan.FromSeconds(5),
                Rule = SpecialRule.Gravity,
                Opponents = new List<OpponentConfig>
                {
                    new OpponentConfig { Name = "Bot 1", Symbol = "O" },
                    new OpponentConfig { Name = "Bot 2", Symbol = "Δ" },
                    new OpponentConfig { Name = "Bot 3", Symbol = "□" },
                    new OpponentConfig { Name = "Bot 4", Symbol = "◇" }
                }
            });
        }

        public LevelConfig GetLevel(int levelNumber)
        {
            if (_levels.ContainsKey(levelNumber))
            {
                return _levels[levelNumber];
            }

            // Fallback for infinite levels? Or just wrap around / repeat last?
            // Let's loop back to level 6 configuration but maybe harder?
            // For now, just return Level 6 for anything higher.
            var maxLevel = _levels[6];
            return new LevelConfig
            {
                LevelNumber = levelNumber,
                GridSize = maxLevel.GridSize,
                WinConditionLength = maxLevel.WinConditionLength,
                TurnTimeLimit = maxLevel.TurnTimeLimit,
                Rule = maxLevel.Rule,
                Opponents = maxLevel.Opponents // Share reference for now
            };
        }
    }
}
