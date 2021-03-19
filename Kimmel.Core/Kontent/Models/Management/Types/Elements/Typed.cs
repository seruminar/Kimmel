namespace Kimmel.Core.Kontent.Models.Management.Types.Elements
{
    public abstract class Typed : Identified
    {
        public string? Codename { get; set; }

        public string Type { get; protected set; } = "unknown";
    }
}