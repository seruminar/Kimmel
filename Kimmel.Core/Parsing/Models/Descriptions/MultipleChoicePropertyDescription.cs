using System;

namespace Kimmel.Core.Parsing.Models.Descriptions
{
    public class MultipleChoicePropertyDescription : DescribesAccessibleProperty
    {
        public string[] Options { get; set; } = Array.Empty<string>();

        public MultipleChoicePropertyDescription(
            string? label
            ) : base(label)
        {
        }
    }
}