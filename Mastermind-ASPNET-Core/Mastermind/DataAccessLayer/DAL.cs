using Mastermind.DataAccessLayer.Factories;

namespace Mastermind.DataAccessLayer
{
    public class DAL
    {
        private ConfigFactory? _configFact;

        public static string? ConnectionString { get; set; }

        public ConfigFactory ConfigFact
        {
            get
            {
                _configFact ??= new ConfigFactory();

                return _configFact;
            }
        }
    }
}
