using System;
using System.Collections.Generic;

namespace TicTacWhoa.Models
{
    public enum SpecialRule
    {
        None,
        Gravity,
        DisappearingPieces
    }

    public class LevelConfig
    {
        public int LevelNumber { get; set; }
        public int GridSize { get; set; } = 3;
        public int WinConditionLength { get; set; } = 3;
        public TimeSpan? TurnTimeLimit { get; set; }
        public List<OpponentConfig> Opponents { get; set; } = new List<OpponentConfig>();
        public SpecialRule Rule { get; set; } = SpecialRule.None;
    }
}
