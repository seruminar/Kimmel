using Kimmel.Core.Models;

namespace Kimmel.Core.Kontent.Models.Management.Types.Elements
{
    public class Limit
    {
        public sealed class LimitCondition : Union<string>
        {
            public static LimitCondition AtMost = new("at_most");
            public static LimitCondition Exactly = new("exactly");
            public static LimitCondition AtLeast = new("at_least");

            private LimitCondition(string value) : base(value)
            {
            }
        }

        public int Value { get; set; }

        public LimitCondition Condition { get; set; }

        public Limit(int value, LimitCondition condition)
        {
            Value = value;
            Condition = condition;
        }
    }
}