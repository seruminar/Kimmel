namespace Kimmel.Core.Kontent.Models.Management.Elements
{
    public class UrlSlugElement : AbstractElement<string>
    {
        public const string Type = "url_slug";

        public string? Mode { get; set; }
    }
}