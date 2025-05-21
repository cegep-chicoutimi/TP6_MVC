namespace Mastermind.GameModels
{
    public class Game
    {
        public enum GameState
        {
            ComputerWin,
            PlayerWin,
            Running
        }

        public int NbColors { get; set; }
        public int NbPositions { get; set; }
        public int NbAttempts { get; set; }

        public GameState State { get; set; } = GameState.Running;
        public int CurrentPlayingRow { get; set; } = 1;
        public ComputerRow ComputerRow { get; set; } = new ComputerRow();
        public List<PlayerRow> PlayerRows { get; set; } = new List<PlayerRow>();

        public Game(int nbColors, int nbPositions, int nbAttempts)
        {
            Random rnd = new();

            NbColors = nbColors;
            NbPositions = nbPositions;
            NbAttempts = nbAttempts;

            for (int i = 0; i < NbPositions; i++)
            {
                ComputerRow.PawnColors.Add(rnd.Next(1, NbColors + 1));
            }
        }
        
        public void Validate(PlayerRow playerRow)
        {
            if (State == GameState.Running)
            {
                List<int> computerColors = new(ComputerRow.PawnColors); // Création d'une copie

                // Vérification pour les pions valides de la bonne couleur à la bonne position
                for (int position = 0; position < NbPositions; position++)
                {
                    Pawn pawn = playerRow.Pawns[position];

                    if (pawn.Color == ComputerRow.PawnColors[position])
                    {
                        pawn.Mark = Pawn.MarkState.Black;
                        computerColors.Remove(pawn.Color);
                    }
                }

                if (playerRow.NbBlackMarks == NbPositions)
                {
                    State = GameState.PlayerWin;
                }
                else
                {
                    // Vérification pour les pions qui sont de la bonne couleur mais hors position
                    for (int position = 0; position < NbPositions; position++)
                    {
                        Pawn pawn = playerRow.Pawns[position];

                        if (pawn.Mark == Pawn.MarkState.None && computerColors.Contains(pawn.Color))
                        {
                            pawn.Mark = Pawn.MarkState.White;
                            computerColors.Remove(pawn.Color);
                        }
                    }

                    if (CurrentPlayingRow == NbAttempts)
                    {
                        State = GameState.ComputerWin;
                    }
                }

                PlayerRows.Add(playerRow);
                CurrentPlayingRow++;
            }
        }
    }
}
