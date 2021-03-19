using System;

namespace Kimmel.Core.Parsing.Models.Descriptions
{
    public class LinkedItemsPropertyDescription : DescribesList
    {
        public string[] LinkedTypeIds { get; set; } = Array.Empty<string>();

        public LinkedItemsPropertyDescription(
            string? label
            ) : base(label)
        {
        }
    }
}