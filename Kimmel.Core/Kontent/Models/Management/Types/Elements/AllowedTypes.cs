using Kimmel.Core.Models;

namespace Kimmel.Core.Kontent.Models.Management.Types.Elements
{
    public sealed class AllowedTypes : Union<string>
    {
        public static AllowedTypes Adjustable = new("adjustable");
        public static AllowedTypes Any = new("any");

        private AllowedTypes(string value) : base(value)
        {
        }
    }
}