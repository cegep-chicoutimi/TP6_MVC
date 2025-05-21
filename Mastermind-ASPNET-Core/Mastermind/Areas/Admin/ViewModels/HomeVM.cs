namespace Mastermind.Areas.Admin.ViewModels
{
    public class HomeVM
    {
        public int NbColors { get; set; }
        public int NbPositions { get; set; }
        public int NbAttempts { get; set; }

        public HomeVM() { }

        public HomeVM(int nbColors, int nbPositions, int nbAttempts)
        {
            NbColors = nbColors;
            NbPositions = nbPositions;
            NbAttempts = nbAttempts;
        }
    }
}
