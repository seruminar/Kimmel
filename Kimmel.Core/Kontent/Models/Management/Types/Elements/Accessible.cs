using Kimmel.Core.Kontent.Models.Management.References;

namespace Kimmel.Core.Kontent.Models.Management.Types.Elements
{
    public class Accessible : Typed
    {
        public string Name { get; set; } = string.Empty;

        public bool Is_required { get; set; }

        public string? Guidelines { get; set; }

        public Reference? Content_group { get; set; }
    }
}