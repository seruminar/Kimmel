namespace Kimmel.Core.Parsing.Models.Descriptions
{
    public abstract class DescribesList : DescribesAccessibleProperty
    {
        public int? Minimum { get; set; }

        public int? Maximum { get; set; }

        protected DescribesList(
            string? label
            ) : base(label)
        {
        }
    }
}