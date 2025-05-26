using Mastermind.DataAccessLayer.Factories;

namespace Mastermind.Areas.Admin.ViewModels
{
    public class HomeVM
    {
        public int MaxAttempts { get; set; }
        public int CodeLength { get; set; }
        public int AvailableColors { get; set; }
        public List<MonthlySignup> MonthlySignups { get; set; }

        public HomeVM(int maxAttempts, int codeLength, int availableColors, List<MonthlySignup> monthlySignups)
        {
            MaxAttempts = maxAttempts;
            CodeLength = codeLength;
            AvailableColors = availableColors;
            MonthlySignups = monthlySignups;
        }
    }
}
