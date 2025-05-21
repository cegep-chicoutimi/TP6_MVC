namespace Mastermind.Models
{
    public class Config
    {
        public const string NB_COLORS = "nb-colors";
        public const string NB_POSITIONS = "nb-positions";
        public const string NB_ATTEMPTS = "nb-attempts";

        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public Config() { }
    }
}