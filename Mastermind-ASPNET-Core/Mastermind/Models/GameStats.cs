using System;

namespace Mastermind.Models
{
    public class GameStats
    {
        public int MemberId { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int? BestScore { get; set; }
    }
} 