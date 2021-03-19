using Kimmel.Core.Models;

namespace Kimmel.Core.Kontent.Models.Management.Types.Elements
{
    public class MaximumTextLength
    {
        public sealed class ValueAppliesTo : Union<string>
        {
            public static ValueAppliesTo Words = new("words");
            public static ValueAppliesTo Characters = new("characters");

            private ValueAppliesTo(string value) : base(value)
            {
            }
        }

        public int Value { get; set; }

        public ValueAppliesTo Applies_to { get; set; } = ValueAppliesTo.Words;
    }
}