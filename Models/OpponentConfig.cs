using System;
using System.Collections.Generic;

namespace TicTacWhoa.Models
{
    public class OpponentConfig
    {
        public string Name { get; set; } = "Computer";
        public string Symbol { get; set; } = "O";
        public double TimeoutChance { get; set; } = 0.0;
        public int MoveDelayMs { get; set; } = 1000;
    }
}
