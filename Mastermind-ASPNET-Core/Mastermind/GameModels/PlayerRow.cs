namespace Mastermind.GameModels
{
    public class PlayerRow
    {
        public List<Pawn> Pawns { get; set; } = new List<Pawn>();

        public int NbBlackMarks
        {
            get
            {
                return Pawns.Count(pawn => pawn.Mark == Pawn.MarkState.Black);
            }
        }

        public int NbWhiteMarks
        {
            get
            {
                return Pawns.Count(pawn => pawn.Mark == Pawn.MarkState.White);
            }
        }

        public PlayerRow() { }
    }
}
