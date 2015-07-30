//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using DustedCodes.Blog.Data;
//using DustedCodes.Blog.Helpers;
//using NSubstitute;
//using NUnit.Framework;

//namespace DustedCodes.Blog.Tests
//{
//    [TestFixture]
//    public class ArticleParserTests
//    {
//        private IUrlGenerator _permalinkGenerator;
//        private IArticleParser _sut;

//        [SetUp]
//        public void Setup()
//        {
//            _permalinkGenerator = Substitute.For<IUrlGenerator>();
//            _sut = new ArticleParser(_permalinkGenerator);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _permalinkGenerator = null;
//            _sut = null;
//        }

//        [Test]
//        public void ParseAsync__With_Null_Id_And_Null_Content__Throws_Exception()
//        {
//            Assert.Throws<ArgumentNullException>(async () => await _sut.ParseAsync(null, null));
//        }

//        [Test]
//        public async Task ParseAsync__With_Null_Id_And_Empty_Content__Builds_Article()
//        {
//            const string rawContent = "";

//            var article = await _sut.ParseAsync(null, rawContent);

//            Assert.IsNull(article.Metadata.Id);
//            Assert.AreEqual(rawContent, article.Content);
//        }

//        [Test]
//        public void ParseAsync__With_Empty_Id_And_Null_Content__Throws_Exception()
//        {
//            const string id = "";

//            Assert.Throws<ArgumentNullException>(async () => await _sut.ParseAsync(id, null));
//        }

//        [Test]
//        public async Task ParseAsync__With_Empty_Id_And_Empty_Content__Builds_Article()
//        {
//            const string id = "";
//            const string rawContent = "";

//            var article = await _sut.ParseAsync(id, rawContent);

//            Assert.AreEqual(id, article.Metadata.Id);
//            Assert.AreEqual(rawContent, article.Content);
//        }

//        [Test]
//        public async Task ParseAsync__With_An_Id_And_Some_Content__Builds_Article()
//        {
//            const string id = "9sa£p9 u9m %c %$<>";
//            const string rawContent = "cd2434daw& * ( % $ 4vaw a4va34 <>";

//            var article = await _sut.ParseAsync(id, rawContent);

//            Assert.AreEqual(id, article.Metadata.Id);
//            Assert.AreEqual(rawContent, article.Content);
//        }

//        [Test]
//        public async Task ParseAsync__With_Multiline_Content_And_Without_Metadata__Builds_Article()
//        {
//            const string id = "9sa£p9 u9m %c %$<>";
//            const string rawContent = @"cd2434daw& * ( % $ 4vaw a4va34 <>
//
//test
//
//<hello></world>";

//            var article = await _sut.ParseAsync(id, rawContent);

//            Assert.AreEqual(id, article.Metadata.Id);
//            Assert.AreEqual(rawContent, article.Content);
//        }

//        [Test]
//        public async Task ParseAsync__With_Multiline_Content_Starting_With_An_Empty_Line_And_Without_Metadata__Builds_Article()
//        {
//            const string id = "9sa£p9 u9m %c %$<>";
//            const string rawContent = @"
//
//cd2434daw& * ( % $ 4vaw a4va34 <>";

//            var article = await _sut.ParseAsync(id, rawContent);

//            Assert.AreEqual(id, article.Metadata.Id);
//            Assert.AreEqual(rawContent, article.Content);
//        }

//        [Test]
//        public async Task ParseAsync__With_Multiline_Content_Ending_With_An_Empty_Line_And_Without_Metadata__Builds_Article()
//        {
//            const string id = "9sa£p9 u9m %c %$<>";
//            const string rawContent = @"test
//
//";

//            var article = await _sut.ParseAsync(id, rawContent);

//            Assert.AreEqual(id, article.Metadata.Id);
//            Assert.AreEqual(rawContent, article.Content);
//        }

//        [Test]
//        public async Task ParseAsync__With_Content_Starting_With_An_Empty_Line_And_Metadata_Afterwards__Doesnt_Recognise_Metadata()
//        {
//            const string id = "9sa£p9 u9m %c %$<>";
//            const string rawContent = @"
//<!--
//Title: Boom shakalaka wowaweewow
//-->
//
//Content
//";

//            var article = await _sut.ParseAsync(id, rawContent);

//            Assert.AreEqual(id, article.Metadata.Id);
//            Assert.AreEqual(rawContent, article.Content);
//        }

//        [Test]
//        public async Task ParseAsync__With_Content_Starting_With_Some_Metadata_On_The_First_Line__Doesnt_Recognise_Metadata()
//        {
//            const string id = "9sa£p9 u9m %c %$<>";
//            const string rawContent = @"<!-- Title: Boom shakalaka wowaweewow
//-->
//
//Content
//";

//            var article = await _sut.ParseAsync(id, rawContent);

//            Assert.AreEqual(id, article.Metadata.Id);
//            Assert.AreEqual(rawContent, article.Content);
//        }

//        [Test]
//        public async Task ParseAsync__With_Content_With_Correctly_Formatted_Metadata__Builds_Correct_Article()
//        {
//            const string id = "9sa£p9 u9m %c %$<>";
//            const string expectedTitle = "Hello World";
//            const string expectedAuthor = "Double O Seven";
//            var expectedPublishDateTime = new DateTime(2014, 8, 23, 23, 5, 0);
//            var expectedLastEditedDateTime = new DateTime(2014, 11, 23, 5, 11, 22);
//            var expectedTags = new[] { "asp.net", "mvc", "iis-7.5", "visual-studio" };
//            const string expectedContent = "Content";
//            var rawContent = string.Format(@"<!--
//Title: {0}
//Author: {1}
//Published: {2}
//LastEdited: {3}
//Tags: asp.net mvc iis-7.5 visual-studio
//-->
//{4}", expectedTitle, expectedAuthor, expectedPublishDateTime, expectedLastEditedDateTime.ToString("O"), expectedContent);


//            var article = await _sut.ParseAsync(id, rawContent);

//            Assert.AreEqual(id, article.Metadata.Id);
//            Assert.AreEqual(expectedContent, article.Content);
//            Assert.AreEqual(expectedTitle, article.Metadata.Title);
//            Assert.AreEqual(expectedAuthor, article.Metadata.Author);
//            Assert.AreEqual(expectedPublishDateTime, article.Metadata.PublishDateTime);
//            Assert.AreEqual(expectedLastEditedDateTime, article.Metadata.LastEditedDateTime);
//            Assert.IsTrue(expectedTags.SequenceEqual(article.Metadata.Tags));
            
//        }

//        [Test]
//        public async Task ParseAsync__With_Content_With_Slightly_Weirdly_Formatted_Metadata__Builds_Correct_Article()
//        {
//            const string id = "9sa£p9 u9m %c %$<>";
//            const string expectedTitle = "Hello World";
//            const string expectedAuthor = "Double O Seven";
//            var expectedPublishDateTime = new DateTime(2014, 8, 23, 23, 5, 0);
//            var expectedLastEditedDateTime = new DateTime(2014, 11, 23, 5, 11, 22);
//            var expectedTags = new[] { "asp.net", "mvc", "iis-7.5", "visual-studio" };
//            const string expectedContent = "Content";
//            var rawContent = string.Format(@"<!--
//Title : {0} 
//Author:  {1}  
// published: {2}
//  Lastedited  : {3}
//Tags: asp.net   mvc    iis-7.5  visual-studio
//-->
//{4}", expectedTitle, expectedAuthor, expectedPublishDateTime, expectedLastEditedDateTime.ToString("O"), expectedContent);


//            var article = await _sut.ParseAsync(id, rawContent);

//            Assert.AreEqual(id, article.Metadata.Id);
//            Assert.AreEqual(expectedContent, article.Content);
//            Assert.AreEqual(expectedTitle, article.Metadata.Title);
//            Assert.AreEqual(expectedAuthor, article.Metadata.Author);
//            Assert.AreEqual(expectedPublishDateTime, article.Metadata.PublishDateTime);
//            Assert.AreEqual(expectedLastEditedDateTime, article.Metadata.LastEditedDateTime);
//            Assert.IsTrue(expectedTags.SequenceEqual(article.Metadata.Tags));
//        }

//        [Test]
//        public async Task ParseAsync__With_Content_With_Duplicate_Metadata__Builds_Correct_Article()
//        {
//            const string id = "9sa£p9 u9m %c %$<>";
//            const string expectedTitle = "Hello World";
//            const string expectedAuthor = "Double O Seven";
//            var expectedPublishDateTime = new DateTime(2014, 8, 23, 23, 5, 0);
//            var expectedLastEditedDateTime = new DateTime(2014, 11, 23, 5, 11, 22);
//            var expectedTags = new[] { "asp.net", "mvc", "iis-7.5", "visual-studio" };
//            const string expectedContent = "Content";
//            var rawContent = string.Format(@"<!--
//Title: 1
//Author: Something else
//Author: {1}
//Title: 2
//Published: {2}
//LastEdited: {3}
//Tags: NOTHING
//Tags: asp.net mvc iis-7.5 visual-studio
//Title: {0}
//-->
//{4}", expectedTitle, expectedAuthor, expectedPublishDateTime, expectedLastEditedDateTime.ToString("O"), expectedContent);


//            var article = await _sut.ParseAsync(id, rawContent);

//            Assert.AreEqual(id, article.Metadata.Id);
//            Assert.AreEqual(expectedContent, article.Content);
//            Assert.AreEqual(expectedTitle, article.Metadata.Title);
//            Assert.AreEqual(expectedAuthor, article.Metadata.Author);
//            Assert.AreEqual(expectedPublishDateTime, article.Metadata.PublishDateTime);
//            Assert.AreEqual(expectedLastEditedDateTime, article.Metadata.LastEditedDateTime);
//            Assert.IsTrue(expectedTags.SequenceEqual(article.Metadata.Tags));
//        }

//        [Test]
//        public async Task ParseAsync__With_Content_With_Metadata_Closing_Tag_Not_On_New_Line__Builds_Article_Without_Content()
//        {
//            const string id = "9sa£p9 u9m %c %$<>";
//            const string expectedTitle = "Hello World";
//            const string expectedAuthor = "Double O Seven";
//            var expectedPublishDateTime = new DateTime(2014, 8, 23, 23, 5, 0);
//            var expectedLastEditedDateTime = new DateTime(2014, 11, 23, 5, 11, 22);
//            var expectedTags = new[] { "asp.net", "mvc", "iis-7.5", "visual-studio", "-->" };
//            const string expectedContent = "";
//            var rawContent = string.Format(@"<!--
//Title: {0}
//Author: {1}
//Published: {2}
//LastEdited: {3}
//Tags: asp.net mvc iis-7.5 visual-studio -->
//Content
//and more content", expectedTitle, expectedAuthor, expectedPublishDateTime, expectedLastEditedDateTime.ToString("O"));


//            var article = await _sut.ParseAsync(id, rawContent);

//            Assert.AreEqual(id, article.Metadata.Id);
//            Assert.AreEqual(expectedContent, article.Content);
//            Assert.AreEqual(expectedTitle, article.Metadata.Title);
//            Assert.AreEqual(expectedAuthor, article.Metadata.Author);
//            Assert.AreEqual(expectedPublishDateTime, article.Metadata.PublishDateTime);
//            Assert.AreEqual(expectedLastEditedDateTime, article.Metadata.LastEditedDateTime);
//            Assert.IsTrue(expectedTags.SequenceEqual(article.Metadata.Tags));
//        }

//        [Test]
//        public async Task ParseAsync__With_Any_Valid_Id_And_Content__Generates_Permalink_URL_Once()
//        {
//            const string id = "3dfsa3wa3wa";
//            const string rawContent = "";

//            await _sut.ParseAsync(id, rawContent);

//            _permalinkGenerator.ReceivedWithAnyArgs(1).GeneratePermalinkUrl(null);
//        }

//        [Test]
//        public async Task ParseAsync__With_Any_Valid_Id_And_Content__Generates_Correct_Permalink_URL()
//        {
//            const string id = "3dfsa3wa3wa";
//            const string rawContent = "";

//            await _sut.ParseAsync(id, rawContent);

//            _permalinkGenerator.Received().GeneratePermalinkUrl(id);
//        }

//        [Test]
//        public async Task ParseAsync__With_Any_Valid_Id_And_Content__Returns_Article_With_Permalink_URL()
//        {
//            const string expectedUrl = "dsf3wfsda3rasdfag;lkaj3;lk3";
//            const string id = "3dfsa3wa3wa";
//            const string rawContent = "";
//            _permalinkGenerator.GeneratePermalinkUrl(null).ReturnsForAnyArgs(expectedUrl);

//            var article = await _sut.ParseAsync(id, rawContent);

//            Assert.AreEqual(expectedUrl, article.PermalinkUrl);
//        }
//    }
//}