using System.Collections.Generic;

using Kimmel.Core.Kontent.Models.Management.Types.Elements;

namespace Kimmel.Core.Kontent.Models.Management.Types.ContentTypes
{
    public class ContentType : Identified
    {
        public string Name { get; set; }

        public string? Codename { get; set; }

        public IList<Typed> Elements { get; set; }

        public ContentType(string name, IList<Typed> elements)
        {
            Name = name;
            Elements = elements;
        }
    }
}