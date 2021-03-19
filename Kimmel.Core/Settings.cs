namespace Kimmel.Core
{
    public class Settings
    {
        public KimmelSettings? Kimmel { get; set; }
    }

    public class KimmelSettings
    {
        public string Setting { get; set; } = string.Empty;
    }
}