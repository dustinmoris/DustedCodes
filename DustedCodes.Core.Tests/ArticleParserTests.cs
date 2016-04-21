using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DustedCodes.Core.Data;
using DustedCodes.Core.Data.StaticFileStorage;
using DustedCodes.Core.IO;
using NSubstitute;
using NUnit.Framework;

namespace DustedCodes.Core.Tests
{
    [TestFixture]
    public class ArticleParserTests
    {
        /// <summary>
        /// Helper class to assert articles
        /// </summary>
        private static class AssertArticle
        {
            public static void Match(Article expectedArticle, Article actualArticle)
            {
                Assert.AreEqual(expectedArticle.Id, actualArticle.Id);
                Assert.AreEqual(expectedArticle.Author, actualArticle.Author);
                Assert.AreEqual(expectedArticle.Title, actualArticle.Title);
                Assert.AreEqual(expectedArticle.PublishDateTime, actualArticle.PublishDateTime);
                CollectionAssert.AreEqual(expectedArticle.Tags, actualArticle.Tags);
                Assert.AreEqual(expectedArticle.Content, actualArticle.Content);
            }
        }

        /// <summary>
        /// Helper class to createa different articles
        /// </summary>
        private static class ArticleTestRepository
        {
            public static class Invalid
            {
                public static TextReader EmptyArticle
                {
                    get
                    {
                        const string article = "";

                        return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                    }
                }

                public static TextReader ArticleWithoutMetadata
                {
                    get
                    {
                        const string article = "Hello World";

                        return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                    }
                }

                public static TextReader ArticleWithMetadataAndNoContent
                {
                    get
                    {
                        const string article = @"<!--
Title: Article With Metadata and No Content
Author: Foo Bar
-->";

                        return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                    }
                }

                public static TextReader ArticleWithMetadataWithoutOpeningTag
                {
                    get
                    {
                        const string article = @"Published: 2015-02-26 15:00
LastEdited: 2015-03-13 14:30
Author: Foo Bar
Title: Great Article
Tags: tag-1 another-tag 3rd-tag-here
-->
Test Article";

                        return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                    }
                }

                public static TextReader ArticleWithMetadataWithoutClosingTag
                {
                    get
                    {
                        const string article = @"<!--
Published: 2015-02-26 15:00
LastEdited: 2015-03-13 14:30
Author: Foo Bar
Title: Great Article
Tags: tag-1 another-tag 3rd-tag-here

Test Article";

                        return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                    }
                }

                public static TextReader ArticleWithBadlyInitialisedMetadata
                {
                    get
                    {
                        const string article = @"<!-- Published: 2015-02-26 15:00
LastEdited: 2015-03-13 14:30
Author: Foo Bar
Title: Great Article
Tags: tag-1 another-tag 3rd-tag-here
-->
Test Article";

                        return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                    }
                }

                public static TextReader ArticleWithBadlyClosedMetadata
                {
                    get
                    {
                        const string article = @"<!-- Published: 2015-02-26 15:00
LastEdited: 2015-03-13 14:30
Author: Foo Bar
Title: Great Article
Tags: tag-1 another-tag 3rd-tag-here -->
Test Article";

                        return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                    }
                }
            }

            public static class Valid
            {
                public static class DefaultArticle
                {
                    public static TextReader Text
                    {
                        get
                        {
                            const string article = @"<!--
Title: Valid Article
-->
Hello World!";

                            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                        }
                    }

                    public static Article ExpectedArticle => new Article
                    {
                        Title = "Valid Article",
                        Content = "Hello World!"
                    };
                }

                public static class ArticleWithEmptyLinesAfterMetadata
                {
                    public static TextReader Text
                    {
                        get
                        {
                            const string article = @"<!--
Published: 2015-02-26 15:00
LastEdited: 2015-03-13 14:30
Author: Foo Bar
Title: Great Article
Tags: tag-1 another-tag 3rd-tag-here
-->


Test Article";

                            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                        }
                    }

                    public static Article ExpectedArticle => new Article
                    {
                        PublishDateTime = new DateTime(2015, 2, 26, 15, 0, 0),
                        Author = "Foo Bar",
                        Title = "Great Article",
                        Tags = new[] { "tag-1", "another-tag", "3rd-tag-here" },
                        Content = "Test Article"
                    };
                }

                public static class ArticleWithAllMetadataAndHtmlFormattedContent
                {
                    public static TextReader Text
                    {
                        get
                        {
                            const string article = @"<!--
Published: 2015-02-26 15:00
LastEdited: 2015-03-13 14:30
Author: Foo Bar
Title: Great Article
Tags: tag-1 another-tag 3rd-tag-here
-->
<h2>Hello World</h2>
<p>This is a test.</p>

<p>The End!</p>";

                            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                        }
                    }

                    public static Article ExpectedArticle => new Article
                    {
                        PublishDateTime = new DateTime(2015, 2, 26, 15, 0, 0),
                        Author = "Foo Bar",
                        Title = "Great Article",
                        Tags = new[] { "tag-1", "another-tag", "3rd-tag-here" },
                        Content = @"<h2>Hello World</h2>
<p>This is a test.</p>

<p>The End!</p>"
                    };
                }

                public static class ArticleWithDuplicateMetadata
                {
                    public static TextReader Text
                    {
                        get
                        {
                            const string article = @"<!--
Published: 2015-02-26 15:00
LastEdited: 2015-03-13 14:30
Author: Foo Bar
Title: Great Article
Tags: tag-1 another-tag 3rd-tag-here
Author: Foo Bar the second
-->
Test Article";

                            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                        }
                    }

                    public static Article ExpectedArticle => new Article
                    {
                        PublishDateTime = new DateTime(2015, 2, 26, 15, 0, 0),
                        Author = "Foo Bar the second",
                        Title = "Great Article",
                        Tags = new[] { "tag-1", "another-tag", "3rd-tag-here" },
                        Content = @"Test Article"
                    };
                }

                public static class ArticleWithWeirdlyFormattedMetadata
                {
                    public static TextReader Text
                    {
                        get
                        {
                            const string article = @"<!--

   Published  :    2015-02-26 15:00
 lastedited: 2015-03-13 14:30
Author:             Foo Bar


        title: Great Article         
Tags:    tag-1    another-tag     3rd-tag-here

Author: Foo Bar the second

-->
Test Article";

                            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                        }
                    }

                    public static Article ExpectedArticle => new Article
                    {
                        PublishDateTime = new DateTime(2015, 2, 26, 15, 0, 0),
                        Author = "Foo Bar the second",
                        Title = "Great Article",
                        Tags = new[] { "tag-1", "another-tag", "3rd-tag-here" },
                        Content = @"Test Article"
                    };
                }

                public static class ArticleWithNonSupportedMetadata
                {
                    public static TextReader Text
                    {
                        get
                        {
                            const string article = @"<!--
Year: 2015
Month: December
Author: foo Bar
ShowComments: False
-->
Test Article";

                            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(article)));
                        }
                    }

                    public static Article ExpectedArticle => new Article
                    {
                        Author = "foo Bar",
                        Content = @"Test Article"
                    };
                }
            }
        }

        private IArticleParser _sut;
        private ITextReaderFactory _textReaderFactory;
        private const string DefaultArticleId = "this-is-a-file";
        private readonly FileInfo _defaultFile = new FileInfo($"C:\\some\\{DefaultArticleId}.txt");

        [SetUp]
        public void Setup()
        {
            _textReaderFactory = Substitute.For<ITextReaderFactory>();
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Valid.DefaultArticle.Text);

            _sut = new ArticleParser(_textReaderFactory);
        }

        [TearDown]
        public void TearDown()
        {
            _sut = null;
        }

        [Test]
        public void When_Parsing_File__With_Null_File__Throws_Exception()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _sut.ParseAsync(null));
        }

        [Test]
        public async Task When_Parsing_File__With_A_Valid_Article_File__Creates_Text_Reader()
        {
            var fileInfo = _defaultFile;

            await _sut.ParseAsync(fileInfo);

            _textReaderFactory.ReceivedWithAnyArgs(1).FromFile(null);
        }

        [Test]
        public async Task When_Parsing_File__With_A_Valid_Article_File__Creates_Text_Reader_From_Correct_File()
        {
            var fileInfo = _defaultFile;

            await _sut.ParseAsync(fileInfo);

            _textReaderFactory.Received().FromFile(fileInfo);
        }

        [Test]
        public void When_Parsing_File__With_An_Empty_File__Throws_Exception()
        {
            var fileInfo = _defaultFile;
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Invalid.EmptyArticle);

            Assert.ThrowsAsync<FormatException>(async () => await _sut.ParseAsync(fileInfo));
        }

        [Test]
        public void When_Parsing_File__Without_Metadata_In_File__Throws_Exception()
        {
            var fileInfo = _defaultFile;
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Invalid.ArticleWithoutMetadata);

            Assert.ThrowsAsync<FormatException>(async () => await _sut.ParseAsync(fileInfo));
        }

        [Test]
        public void When_Parsing_File__With_Metadata_Without_Opening_Tag__Throws_Exception()
        {
            var fileInfo = _defaultFile;
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Invalid.ArticleWithMetadataWithoutOpeningTag);

            Assert.ThrowsAsync<FormatException>(async () => await _sut.ParseAsync(fileInfo));
        }

        [Test]
        public void When_Parsing_File__With_Metadata_Without_Closing_Tag__Throws_Exception()
        {
            var fileInfo = _defaultFile;
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Invalid.ArticleWithMetadataWithoutClosingTag);

            Assert.ThrowsAsync<FormatException>(async () => await _sut.ParseAsync(fileInfo));
        }

        [Test]
        public void When_Parsing_File__With_Badly_Initialised_Metadata__Throws_Exception()
        {
            var fileInfo = _defaultFile;
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Invalid.ArticleWithBadlyInitialisedMetadata);

            Assert.ThrowsAsync<FormatException>(async () => await _sut.ParseAsync(fileInfo));
        }

        [Test]
        public void When_Parsing_File__With_Badly_Closed_Metadata__Throws_Exception()
        {
            var fileInfo = _defaultFile;
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Invalid.ArticleWithBadlyClosedMetadata);

            Assert.ThrowsAsync<FormatException>(async () => await _sut.ParseAsync(fileInfo));
        }

        [Test]
        public void When_Parsing_File__With_Metadata_And_No_Content__Throws_Exception()
        {
            var fileInfo = _defaultFile;
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Invalid.ArticleWithMetadataAndNoContent);

            Assert.ThrowsAsync<FormatException>(async () => await _sut.ParseAsync(fileInfo));
        }

        [Test]
        public async Task When_Parsing_File__With_Empty_Lines_After_Metadata__Returns_Article()
        {
            var fileInfo = _defaultFile;
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Valid.ArticleWithEmptyLinesAfterMetadata.Text);
            var expectedArticle = ArticleTestRepository.Valid.ArticleWithEmptyLinesAfterMetadata.ExpectedArticle;
            expectedArticle.Id = DefaultArticleId;

            var article = await _sut.ParseAsync(fileInfo);

            AssertArticle.Match(expectedArticle, article);
        }

        [Test]
        public async Task When_Parsing_File__With_All_Metadata_And_Html_Formatted_Content__Returns_Article()
        {
            var fileInfo = _defaultFile;
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Valid.ArticleWithAllMetadataAndHtmlFormattedContent.Text);
            var expectedArticle = ArticleTestRepository.Valid.ArticleWithAllMetadataAndHtmlFormattedContent.ExpectedArticle;
            expectedArticle.Id = DefaultArticleId;

            var article = await _sut.ParseAsync(fileInfo);

            AssertArticle.Match(expectedArticle, article);
        }

        [Test]
        public async Task When_Parsing_File__With_Duplicate_Metadata__Returns_Article()
        {
            var fileInfo = _defaultFile;
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Valid.ArticleWithDuplicateMetadata.Text);
            var expectedArticle = ArticleTestRepository.Valid.ArticleWithDuplicateMetadata.ExpectedArticle;
            expectedArticle.Id = DefaultArticleId;

            var article = await _sut.ParseAsync(fileInfo);

            AssertArticle.Match(expectedArticle, article);
        }

        [Test]
        public async Task When_Parsing_File__With_Weirdly_Formatted_Metadata__Returns_Article()
        {
            var fileInfo = _defaultFile;
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Valid.ArticleWithWeirdlyFormattedMetadata.Text);
            var expectedArticle = ArticleTestRepository.Valid.ArticleWithWeirdlyFormattedMetadata.ExpectedArticle;
            expectedArticle.Id = DefaultArticleId;

            var article = await _sut.ParseAsync(fileInfo);

            AssertArticle.Match(expectedArticle, article);
        }

        [Test]
        public async Task When_Parsing_File__With_Not_Supported_Metadata__Ignores_Unkown_Metadata_And_Returns_Article()
        {
            var fileInfo = _defaultFile;
            _textReaderFactory.FromFile(null).ReturnsForAnyArgs(ArticleTestRepository.Valid.ArticleWithNonSupportedMetadata.Text);
            var expectedArticle = ArticleTestRepository.Valid.ArticleWithNonSupportedMetadata.ExpectedArticle;
            expectedArticle.Id = DefaultArticleId;

            var article = await _sut.ParseAsync(fileInfo);

            AssertArticle.Match(expectedArticle, article);
        }
    }
}
