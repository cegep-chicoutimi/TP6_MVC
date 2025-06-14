﻿using Mastermind.GameModels;
using Mastermind.Models;

namespace Mastermind.ViewModels
{
    public class GameVM
    {
        public Game Game { get; set; }
        public int MaxAttempts => Game.NbAttempts;
        public int CurrentAttempt => Game.CurrentPlayingRow;
        public int PegCount => Game.NbPositions;
        public GameStats? Stats { get; set; }
        public List<string> Colors { get; set; } = new List<string>
        {
            "#FF0000", // Red
            "#00FF00", // Green
            "#0000FF", // Blue
            "#FFFF00", // Yellow
            "#FF00FF", // Magenta
            "#00FFFF"  // Cyan
        };
        
        //TODO: Ajoutez les statistiques du joueur connecté

        public GameVM(Game game, GameStats? stats = null)
        {
            Game = game;
            Stats = stats;
        }
    }
}
