using Kimmel.Core.Kontent.Models.Management.References;

using Newtonsoft.Json;

namespace Kimmel.Core.Kontent.Models.Management.Elements
{
    [JsonConverter(typeof(ElementTypeResolver))]
    public interface IElement
    {
        public Reference? Element { get; set; }
    }
}