using System;
using System.Collections.Generic;

using Kimmel.Core.Kontent.Models.Management.References;
using Kimmel.Core.Models;

namespace Kimmel.Core.Kontent.Models.Management.Types.Elements
{
    public class RichTextElement : Accessible
    {
        public sealed class ChoiceMode : Union<string>
        {
            public static ChoiceMode Single = new("single");
            public static ChoiceMode Multiple = new("multiple");

            private ChoiceMode(string value) : base(value)
            {
            }
        }

        public sealed class Block : Union<string>
        {
            public static Block Images = new("images");
            public static Block Text = new("text");
            public static Block Tables = new("tables");
            public static Block ComponentsAndItems = new("components-and-items");

            private Block(string value) : base(value)
            {
            }
        }

        public sealed class TextBlock : Union<string>
        {
            public static TextBlock Paragraph = new("paragraph");
            public static TextBlock Heading1 = new("heading-one");
            public static TextBlock Heading2 = new("heading-two");
            public static TextBlock Heading3 = new("heading-three");
            public static TextBlock Heading4 = new("heading-four");
            public static TextBlock OrderedList = new("ordered-list");
            public static TextBlock UnorderedList = new("unordered-list");

            private TextBlock(string value) : base(value)
            {
            }
        }

        public sealed class TableBlock : Union<string>
        {
            public static TableBlock Images = new("images");
            public static TableBlock Text = new("text");

            private TableBlock(string value) : base(value)
            {
            }
        }

        public sealed class Formatting : Union<string>
        {
            public static Formatting Unstyled = new("unstyled");
            public static Formatting Bold = new("bold");
            public static Formatting Italic = new("italic");
            public static Formatting Code = new("code");
            public static Formatting Link = new("link");
            public static Formatting Subscript = new("subscript");
            public static Formatting Superscript = new("superscript");

            private Formatting(string value) : base(value)
            {
            }
        }

        public MaximumTextLength? Maximum_text_length { get; set; }

        public int? Maximum_image_size { get; set; }

        public IList<Reference>? Allowed_content_types { get; set; }

        public Block[] Allowed_blocks { get; set; } = Array.Empty<Block>();

        public TextBlock[] Allowed_text_blocks { get; set; } = Array.Empty<TextBlock>();

        public Formatting[] Allowed_formatting { get; set; } = Array.Empty<Formatting>();

        public TableBlock[] Allowed_table_blocks { get; set; } = Array.Empty<TableBlock>();

        public TextBlock[] Allowed_table_text_blocks { get; set; } = Array.Empty<TextBlock>();

        public Formatting[] Allowed_table_formatting { get; set; } = Array.Empty<Formatting>();

        public Limit? Image_width_limit { get; set; }

        public Limit? Image_height_limit { get; set; }

        public AllowedTypes Allowed_image_types { get; set; } = AllowedTypes.Any;

        public RichTextElement()
        {
            Type = "rich_text";
        }
    }
}