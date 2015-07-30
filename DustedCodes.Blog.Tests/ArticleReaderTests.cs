//using System;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using DustedCodes.Blog.Data;
//using DustedCodes.Blog.IO;
//using NSubstitute;
//using NUnit.Framework;

//namespace DustedCodes.Blog.Tests
//{
//    [TestFixture]
//    public class ArticleReaderTests
//    {
//        private IDirectoryReader _directoryReader;
//        private IFileReader _filesReader;
//        private IArticleParser _articleParser;
//        private string _articleDirectoryPath;

//        private IArticleReader _sut;

//        [SetUp]
//        public void Setup()
//        {
//            _directoryReader = Substitute.For<IDirectoryReader>();
//            _filesReader = Substitute.For<IFileReader>();
//            _articleParser = Substitute.For<IArticleParser>();
//            _articleDirectoryPath = "Some path";

//            _sut = new ArticleReader(
//                _directoryReader,
//                _filesReader,
//                _articleParser,
//                _articleDirectoryPath);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _directoryReader = null;
//            _filesReader = null;
//            _articleParser = null;
//            _articleDirectoryPath = null;
//            _sut = null;
//        }

//        [Test]
//        public void ReadAsync__With_Null_Article_Id__Throw_Exception()
//        {
//            Assert.Throws<ArgumentException>(async () => await _sut.ReadAsync(null));
//        }

//        [Test]
//        public void ReadAsync__With_Empty_Article_Id__Throw_Exception()
//        {
//            Assert.Throws<ArgumentException>(async () => await _sut.ReadAsync(""));
//        }

//        [Test]
//        public async Task ReadAsync__With_Valid_Article_Id__Reads_File()
//        {
//            const string articleId = "some-id";

//            await _sut.ReadAsync(articleId);

//            await Task.FromResult(_filesReader.ReceivedWithAnyArgs(1).ReadAllTextAsync(null));
//        }

//        [Test]
//        public async Task ReadAsync__With_Valid_Article_Id__Reads_File_From_Correct_Path()
//        {
//            const string articleId = "some-id";
//            var expectedPath = string.Concat(_articleDirectoryPath, "\\", articleId, ".html");

//            await _sut.ReadAsync(articleId);

//            await Task.FromResult(_filesReader.Received().ReadAllTextAsync(expectedPath));
//        }

//        [Test]
//        public async Task ReadAsync__When_File_Is_Empty__Returns_Null()
//        {
//            const string articleId = "some-id";
//            _filesReader.ReadAllTextAsync(null).ReturnsForAnyArgs(Task.FromResult(""));

//            var actual = await _sut.ReadAsync(articleId);

//            Assert.IsNull(actual);
//        }

//        [Test]
//        public async Task ReadAsync__When_File_Is_Not_Empty__Then_Parses_A_File()
//        {
//            const string articleId = "some-id";
//            _filesReader.ReadAllTextAsync(null).ReturnsForAnyArgs(Task.FromResult("not empty"));

//            await _sut.ReadAsync(articleId);

//            await Task.FromResult(_articleParser.ReceivedWithAnyArgs(1).ParseAsync(null, null));
//        }

//        [Test]
//        public async Task ReadAsync__When_File_Is_Not_Empty__Then_Parses_Correct_File()
//        {
//            const string articleId = "some-id";
//            const string rawContent = "asdfjw3ljfasddl;jkfas";
//            _filesReader.ReadAllTextAsync(null).ReturnsForAnyArgs(Task.FromResult(rawContent));

//            await _sut.ReadAsync(articleId);

//            await Task.FromResult(_articleParser.Received().ParseAsync(articleId, rawContent));
//        }

//        [Test]
//        public async Task ReadAsync__After_Parsing_File__Returns_Result_From_Parser()
//        {
//            const string articleId = "some-id";
//            var expected = new Article();
//            _filesReader.ReadAllTextAsync(null).ReturnsForAnyArgs(Task.FromResult("not empty"));
//            _articleParser.ParseAsync(null, null).ReturnsForAnyArgs(Task.FromResult(expected));

//            var actual = await _sut.ReadAsync(articleId);

//            Assert.AreSame(expected, actual);
//        }

//        [Test]
//        public async Task ReadAllAsync__With_Any_Directory_Path__Gets_Files()
//        {
//            await _sut.ReadAllAsync();

//            _directoryReader.ReceivedWithAnyArgs(1).GetFiles(null);
//        }

//        [Test]
//        public async Task ReadAllAsync__With_Directory_Path__Gets_Correct_Files()
//        {
//            await _sut.ReadAllAsync();

//            _directoryReader.Received().GetFiles(_articleDirectoryPath, "*.html");
//        }

//        [Test]
//        public async Task ReadAllAsync__When_Null_Returned_From_GetFiles__Returns_Null()
//        {
//            _directoryReader.GetFiles(null).ReturnsForAnyArgs((FileInfo[])null);

//            var actual = await _sut.ReadAllAsync();

//            Assert.IsNull(actual);
//        }

//        [Test]
//        public async Task ReadAllAsync__When_No_Matching_Files_In_Directory__Returns_Null()
//        {
//            _directoryReader.GetFiles(null).ReturnsForAnyArgs(new FileInfo[] { });

//            var actual = await _sut.ReadAllAsync();

//            Assert.IsNull(actual);
//        }

//        [Test]
//        public async Task ReadAllAsync__For_Each_Matching_File_In_Directory__Reads_All_Text()
//        {
//            var files = new[]
//            {
//                new FileInfo("C:\\temp\\made\\up\\test.txt"),
//                new FileInfo("C:\\temp\\another\\made\\up\\path\\test.txt"),
//                new FileInfo("C:\\temp\\third\\path\\test.txt")
//            };

//            _directoryReader.GetFiles(null).ReturnsForAnyArgs(files);

//            await _sut.ReadAllAsync();

//            await Task.FromResult(_filesReader.ReceivedWithAnyArgs(3).ReadAllTextAsync(null));
//        }

//        [Test]
//        public async Task ReadAllAsync__For_Each_Matching_File_In_Directory__Reads_From_Correct_Files()
//        {
//            const string filePath1 = "C:\\temp\\made\\up\\test.txt";
//            const string filePath2 = "C:\\temp\\another\\made\\up\\path\\test.txt";
//            const string filePath3 = "C:\\temp\\third\\path\\test.txt";

//            var files = new[]
//            {
//                new FileInfo(filePath1),
//                new FileInfo(filePath2),
//                new FileInfo(filePath3)
//            };

//            _directoryReader.GetFiles(null).ReturnsForAnyArgs(files);

//            await _sut.ReadAllAsync();

//            await Task.FromResult(_filesReader.Received().ReadAllTextAsync(filePath1));
//            await Task.FromResult(_filesReader.Received().ReadAllTextAsync(filePath2));
//            await Task.FromResult(_filesReader.Received().ReadAllTextAsync(filePath3));
//        }

//        [Test]
//        public async Task ReadAllAsync__After_Reading_Files__Parses_To_Articles_Only_For_Files_With_Content()
//        {
//            var files = new[]
//            {
//                new FileInfo("C:\\made\\up\\test.txt"),
//                new FileInfo("C:\\made\\up\\test.txt"),
//                new FileInfo("C:\\made\\up\\test.txt"),
//                new FileInfo("C:\\made\\up\\test.txt")
//            };

//            _directoryReader.GetFiles(null).ReturnsForAnyArgs(files);
//            _filesReader.ReadAllTextAsync(null).ReturnsForAnyArgs(
//                Task.FromResult((string)null),
//                Task.FromResult("something"),
//                Task.FromResult(""),
//                Task.FromResult("something else"));

//            await _sut.ReadAllAsync();

//            await Task.FromResult(_articleParser.ReceivedWithAnyArgs(2).ParseAsync(null, null));
//        }

//        [Test]
//        public async Task ReadAllAsync__After_Reading_Files__Parses_Raw_Content_From_Files()
//        {
//            var files = new[]
//            {
//                new FileInfo("C:\\made\\up\\test1.txt"),
//                new FileInfo("C:\\made\\up\\test2.txt"),
//                new FileInfo("C:\\made\\up\\test3.txt"),
//                new FileInfo("C:\\made\\up\\test4.txt")
//            };

//            _directoryReader.GetFiles(null).ReturnsForAnyArgs(files);
//            _filesReader.ReadAllTextAsync(null).ReturnsForAnyArgs(
//                Task.FromResult((string)null),
//                Task.FromResult("something"),
//                Task.FromResult(""),
//                Task.FromResult("something else"));

//            await _sut.ReadAllAsync();

//            await Task.FromResult(_articleParser.Received().ParseAsync("test2", "something"));
//            await Task.FromResult(_articleParser.Received().ParseAsync("test4", "something else"));
//        }

//        [Test]
//        public async Task ReadAllAsync__After_Parsing__Returns_Articles_In_Descending_Order()
//        {
//            var articles = new[]
//            {
//                new Article {Metadata = {Id = "1", PublishDateTime = DateTime.Now}},
//                new Article {Metadata = {Id = "2", PublishDateTime = DateTime.Now.AddDays(-1)}},
//                new Article {Metadata = {Id = "3", PublishDateTime = DateTime.Now.AddDays(-3)}},
//                new Article {Metadata = {Id = "4", PublishDateTime = DateTime.Now.AddDays(-10)}}
//            };
//            var files = new[]
//            {
//                new FileInfo("C:\\made\\up\\test1.txt"),
//                new FileInfo("C:\\made\\up\\test2.txt"),
//                new FileInfo("C:\\made\\up\\test3.txt"),
//                new FileInfo("C:\\made\\up\\test4.txt")
//            };

//            _directoryReader.GetFiles(null).ReturnsForAnyArgs(files);
//            _filesReader.ReadAllTextAsync(null).ReturnsForAnyArgs(Task.FromResult("something"));
//            _articleParser.ParseAsync(null, null).ReturnsForAnyArgs(
//                Task.FromResult(articles.ElementAt(2)),
//                Task.FromResult(articles.ElementAt(1)),
//                Task.FromResult(articles.ElementAt(0)),
//                Task.FromResult(articles.ElementAt(3)));

//            var actual = await _sut.ReadAllAsync();

//            Assert.IsTrue(articles.SequenceEqual(actual));
//        }
//    }
//}