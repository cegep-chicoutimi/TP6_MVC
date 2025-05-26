using Mastermind.DataAccessLayer.Factories;

namespace Mastermind.DataAccessLayer
{
    public class DAL
    {
        private ConfigFactory? _configFact;
        private GameStatsFactory? _gameStatsFact;
        private MemberFactory? _memberFact;
        private MemberPreferencesFactory? _memberPrefsFact;

        public static string? ConnectionString { get; set; }

        public ConfigFactory ConfigFact
        {
            get
            {
                _configFact ??= new ConfigFactory();

                return _configFact;
            }
        }

        public GameStatsFactory GameStatsFact
        {
            get
            {
                _gameStatsFact ??= new GameStatsFactory();

                return _gameStatsFact;
            }
        }

        public MemberFactory MemberFact
        {
            get
            {
                _memberFact ??= new MemberFactory();

                return _memberFact;
            }
        }

        public MemberPreferencesFactory MemberPrefsFact
        {
            get
            {
                _memberPrefsFact ??= new MemberPreferencesFactory();

                return _memberPrefsFact;
            }
        }
    }
}
