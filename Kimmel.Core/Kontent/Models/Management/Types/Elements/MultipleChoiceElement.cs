using System.Collections.Generic;

using Kimmel.Core.Models;

namespace Kimmel.Core.Kontent.Models.Management.Types.Elements
{
    public class MultipleChoiceElement : Accessible
    {
        public sealed class ChoiceMode : Union<string>
        {
            public static ChoiceMode Single = new("single");
            public static ChoiceMode Multiple = new("multiple");

            private ChoiceMode(string value) : base(value)
            {
            }
        }

        public class Option : Identified
        {
            public string? Codename { get; set; }

            public string Name { get; set; } = string.Empty;
        }

        public ChoiceMode? Mode { get; set; }

        public IList<Option> Options { get; set; } = new List<Option>();

        public MultipleChoiceElement()
        {
            Type = "multiple_choice";
        }
    }
}