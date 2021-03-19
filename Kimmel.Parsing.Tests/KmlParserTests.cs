using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Kimmel.Core.Models;
using Kimmel.Core.Parsing;
using Kimmel.Core.Parsing.Models;
using Kimmel.Core.Parsing.Models.Descriptions;

using Xunit;

namespace Kimmel.Parsing.Tests
{
    public class KmlParserTests
    {
        public Func<KmlParserMode, Options> DefaultOptions => (KmlParserMode mode) => new Options
        {
            Mode = mode.ToString(),
            Comments = "//",
            Space = ' ',
            ArrayDelimiter = ',',
            OptionsStart = '[',
            OptionsEnd = ']',
            OptionDetailStart = '(',
            OptionDetailEnd = ')',
            MoreOrLess = '+',
            Range = '-',
            Required = '*',
            PropertyDelimiter = "\n",
            SnippetStart = "...",
            Skip = "\r",
            Properties = new List<Property> {
                new Property {
                    Description= "TypePropertyDescription",
                    Identifier= null
                },
                new Property {
                    Description= "SnippetTypePropertyDescription",
                    Identifier= null
                },
                new Property {
                    Description= "LinkedItemsPropertyDescription",
                    Identifier= null
                },
                new Property{
                    Description= "AssetPropertyDescription",
                    Identifier= "Asset"
                },
                new Property{
                    Description= "TextPropertyDescription",
                    Identifier= "Text"
                },
                new Property{
                    Description= "MultipleChoicePropertyDescription",
                    Identifier= "MultipleChoice"
                },
                new Property {
                    Description= "SingleChoicePropertyDescription",
                    Identifier= "SingleChoice"
                },
                new Property {
                    Description= "DatePropertyDescription",
                    Identifier= "Date"
                },
                new Property {
                    Description= "NumberPropertyDescription",
                    Identifier= "Number"
                },
                new Property {
                    Description= "RichTextPropertyDescription",
                    Identifier= "RichText"
                },
                new Property {
                    Description= "CustomPropertyDescription",
                    Identifier= "Custom"
                }
            },
            NoPropertyFallback = "LinkedItemsPropertyDescription",
            NoOptionsFallback = "TypePropertyDescription",
            SnippetType = "SnippetTypePropertyDescription",
            TypeStart = "TypePropertyDescription"
        };

        [Theory]
        [MemberData(nameof(KmlParserTests.KmlParser_Strict_Returns_Kml_Data))]
        public void KmlParser_Strict_Returns_Kml(string kml)
        {
            var options = DefaultOptions(KmlParserMode.Strict);

            var kmlParser = new KmlParser(
                options,
                new PropertyDescriber(options),
                new TypeDescriber(options)
                );

            var result = kmlParser.ParseKml(kml);

            Assert.NotNull(result);
            Assert.IsType<Kml>(result);
        }

        private static IEnumerable<object[]> KmlParser_Strict_Returns_Kml_Data()
        {
            return ReadTestData($"{nameof(KmlParser_Strict_Returns_Kml_Data)}.kml")
                .Select(lines => new[] { lines });
        }

        [Theory]
        [MemberData(nameof(KmlParserTests.KmlParser_Strict_Returns_Kml_With_Properties_Data))]
        public void KmlParser_Strict_Returns_Kml_With_Properties(string kml, Func<Kml, bool> valid)
        {
            var options = DefaultOptions(KmlParserMode.Strict);

            var kmlParser = new KmlParser(
                options,
                new PropertyDescriber(options),
                new TypeDescriber(options)
                );

            var result = kmlParser.ParseKml(kml);

            Assert.NotNull(result);
            Assert.IsType<Kml>(result);
            Assert.True(valid(result));
        }

        private static IEnumerable<object[]> KmlParser_Strict_Returns_Kml_With_Properties_Data()
        {
            var typeStart = "Type";

            string makeType(params string[] properties) => string.Join(Environment.NewLine, properties);

            yield return new object[] {
                makeType(typeStart, "RichText[p,ul,tables(p),*] RichText"),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is RichTextPropertyDescription richTextPropertyDescription)
                        {
                            return richTextPropertyDescription.P == true
                                && richTextPropertyDescription.Ul == true
                                && richTextPropertyDescription.TablesP == true
                                && richTextPropertyDescription.Required == true;
                        }

                        return false;
                    }
                    )};

            yield return new object[] {
                makeType(typeStart, "RichText[images,tables(p,ul)] Property"),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is RichTextPropertyDescription propertyDescription)
                        {
                            return propertyDescription.Images == true
                                && propertyDescription.TablesP == true
                                && propertyDescription.TablesUl == true;
                        }

                        return false;
                    }
                    )};

            yield return new object[] {
                makeType(typeStart, "Asset[images] Property"),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is AssetPropertyDescription propertyDescription)
                        {
                            return propertyDescription.AssetMode == AssetMode.Images;
                        }

                        return false;
                    }
                    )};

            yield return new object[] {
                makeType(typeStart, "Asset[images,1+] Property"),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is AssetPropertyDescription propertyDescription)
                        {
                            return propertyDescription.AssetMode == AssetMode.Images
                                && propertyDescription.Minimum == 1;
                        }

                        return false;
                    }
                    )};

            yield return new object[] {
                makeType(typeStart, "Type[] Property"),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is LinkedItemsPropertyDescription propertyDescription)
                        {
                            return propertyDescription.LinkedTypeIds[0] == "Type";
                        }

                        return false;
                    }
                    )};

            yield return new object[] {
                makeType(typeStart, "MultipleChoice[Choice 1,Choice 2] Property"),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is MultipleChoicePropertyDescription propertyDescription)
                        {
                            return propertyDescription.Options.Length == 2;
                        }

                        return false;
                    }
                    )};

            yield return new object[] {
                makeType(typeStart, "RichText[p] Property1", typeStart, "RichText[p] Property2", typeStart),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is RichTextPropertyDescription propertyDescription)
                        {
                            return propertyDescription.Components == true
                                && propertyDescription.ComponentTypeIds[0] == typeStart;
                        }

                        return false;
                    }
                    )};

            yield return new object[] {
                makeType(typeStart, "RichText[p] Property1 ", typeStart),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is RichTextPropertyDescription propertyDescription)
                        {
                            return propertyDescription.Components == true
                                && propertyDescription.ComponentTypeIds[0] == typeStart;
                        }

                        return false;
                    }
                    )};

            yield return new object[] {
                makeType(typeStart, "Text[words(3)] Property1"),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is TextPropertyDescription propertyDescription)
                        {
                            return propertyDescription.Words == 3;
                        }

                        return false;
                    }
                    )};

            yield return new object[] {
                makeType(typeStart, "Text[characters(3)] Property1"),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is TextPropertyDescription propertyDescription)
                        {
                            return propertyDescription.Characters == 3;
                        }

                        return false;
                    }
                    )};

            yield return new object[] {
                makeType(typeStart, "Text[characters(3),words(5)] Property1"),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is TextPropertyDescription propertyDescription)
                        {
                            return propertyDescription.Characters == 3
                                && propertyDescription.Words == 5;
                        }

                        return false;
                    }
                    )};

            yield return new object[] {
                makeType(typeStart, "Text[words(3),*] Property1"),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is TextPropertyDescription propertyDescription)
                        {
                            return propertyDescription.Words == 3
                                && propertyDescription.Required == true;
                        }

                        return false;
                    }
                    )};

            yield return new object[] {
                makeType(typeStart, "RichText[words(3),*] Property1"),
                (Func<Kml, bool>)((kml) =>
                    {
                        if (kml.TypeDescriptions[0].PropertyDescriptions[0] is RichTextPropertyDescription propertyDescription)
                        {
                            return propertyDescription.Words == 3
                                && propertyDescription.Required == true;
                        }

                        return false;
                    }
                    )};
        }

        [Theory]
        [MemberData(nameof(KmlParserTests.KmlParser_Strict_Throws_KmlParserException_Data))]
        public void KmlParser_Strict_Throws_KmlParserException(string kml)
        {
            var options = DefaultOptions(KmlParserMode.Strict);

            var kmlParser = new KmlParser(
                options,
                new PropertyDescriber(options),
                new TypeDescriber(options)
                );

            Kml? result = null;

            var exception = Record.Exception(() => result = kmlParser.ParseKml(kml));

            Assert.NotNull(exception);
            Assert.IsType<ParsingException>(exception);
        }

        private static IEnumerable<object[]> KmlParser_Strict_Throws_KmlParserException_Data()
        {
            return ReadTestData($"{nameof(KmlParser_Strict_Throws_KmlParserException_Data)}.kml")
                .Select(lines => new[] { lines });
        }

        private static IEnumerable<string> ReadTestData(string filename)
        {
            return File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), filename))
                .Split("------------------------------------")
                .Where(line => !string.IsNullOrWhiteSpace(line));
        }
    }
}