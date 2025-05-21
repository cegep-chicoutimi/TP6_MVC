namespace Mastermind.ViewModels
{
    public class HomeVM
    {
        public int NbColors { get; set; }
        public int NbPositions { get; set; }
        public int NbAttempts { get; set; }

        public HomeVM(int nbColor, int nbPositions, int nbAttempts)
        {
            NbColors = nbColor;
            NbPositions = nbPositions;
            NbAttempts = nbAttempts;
        }
    }
}
