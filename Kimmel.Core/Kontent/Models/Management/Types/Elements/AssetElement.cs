namespace Kimmel.Core.Kontent.Models.Management.Types.Elements
{
    public class AssetElement : Accessible
    {
        public Limit? Asset_count_limit { get; set; }

        public int? Maximum_file_size { get; set; }

        public AllowedTypes? Allowed_file_types { get; set; }

        public Limit? Image_width_limit { get; set; }

        public Limit? Image_height_limit { get; set; }

        public AssetElement()
        {
            Type = "asset";
        }
    }
}