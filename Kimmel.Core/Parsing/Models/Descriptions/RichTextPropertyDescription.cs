using System;

namespace Kimmel.Core.Parsing.Models.Descriptions
{
    public class RichTextPropertyDescription : DescribesAccessibleProperty
    {
        public string[] ComponentTypeIds { get; set; } = Array.Empty<string>();

        public bool P { get; set; }

        public bool B { get; set; }

        public bool I { get; set; }

        public bool A { get; set; }

        public bool H1 { get; set; }

        public bool H2 { get; set; }

        public bool H3 { get; set; }

        public bool H4 { get; set; }

        public bool Ul { get; set; }

        public bool Ol { get; set; }

        public bool Sub { get; set; }

        public bool Sup { get; set; }

        public bool Images { get; set; }

        public int? Words { get; set; }

        public int? Characters { get; set; }

        public bool TablesP { get; set; }

        public bool TablesB { get; set; }

        public bool TablesI { get; set; }

        public bool TablesA { get; set; }

        public bool TablesH1 { get; set; }

        public bool TablesH2 { get; set; }

        public bool TablesH3 { get; set; }

        public bool TablesH4 { get; set; }

        public bool TablesUl { get; set; }

        public bool TablesOl { get; set; }

        public bool TablesSub { get; set; }

        public bool TablesSup { get; set; }

        public bool TablesImages { get; set; }

        public bool Tables { get; set; }

        public bool Components { get; set; }

        public RichTextPropertyDescription(
            string? label
            ) : base(label)
        {
        }
    }
}