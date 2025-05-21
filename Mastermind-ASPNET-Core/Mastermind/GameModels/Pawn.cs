namespace Mastermind.GameModels
{
    public class Pawn
    {
        public enum MarkState
        {
            None,
            Black, // bonne couleur à la bonne position
            White  // bonne couleur hors position
        }

        public int Color { get; set; } = 0;
        public MarkState Mark { get; set; } = MarkState.None;

        public Pawn() { }
    }
}
