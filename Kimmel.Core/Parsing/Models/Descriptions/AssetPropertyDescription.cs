namespace Kimmel.Core.Parsing.Models.Descriptions
{
    public enum AssetMode
    {
        None,
        Images
    }

    public class AssetPropertyDescription : DescribesList
    {
        public AssetMode AssetMode { get; set; }

        public AssetPropertyDescription(
            string? label
            ) : base(label)
        {
        }
    }
}