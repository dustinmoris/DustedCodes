using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DustedCodes.Blog.Data;
using NSubstitute;
using NUnit.Framework;

namespace DustedCodes.Blog.Tests
{
    [TestFixture]
    public class LocalArticleRepositoryTests
    {
        private IArticleCache _articleCache;
        private IArticleReader _articleReader;
        private IArticleRepository _sut;

        [SetUp]
        public void Setup()
        {
            _articleCache = Substitute.For<IArticleCache>();
            _articleReader = Substitute.For<IArticleReader>();
            _sut = new LocalArticleRepository(
                _articleCache,
                _articleReader);

            _articleCache.Metadata.Returns(new List<ArticleMetadata>());
            _articleReader.ReadAsync(null).ReturnsForAnyArgs(Task.FromResult(new Article()));
        }

        [TearDown]
        public void TearDown()
        {
            _articleCache = null;
            _articleReader = null;
            _sut = null;
        }

        [Test]
        public async Task GetTotalCount__With_Null_Cache__Reads_All_Articles()
        {
            _articleCache.Metadata.Returns((List<ArticleMetadata>)null);

            await _sut.GetTotalCount();

            await Task.FromResult(_articleReader.Received(1).ReadAllAsync());
        }

        [Test]
        public async Task GetTotalCount__With_Non_Null_Cache__Doesnt_Read_All_Articles()
        {
            _articleCache.Metadata.Returns(new List<ArticleMetadata>());

            await _sut.GetTotalCount();

            await Task.FromResult(_articleReader.DidNotReceive().ReadAllAsync());
        }

        [Test]
        public async Task GetTotalCount__With_Null_Cache__Sets_Cache_With_Result_From_Reader()
        {
            _articleCache.Metadata.Returns((List<ArticleMetadata>)null);
            var articles = new List<Article>
                {
                    new Article {Metadata = {Id = "1"}},
                    new Article {Metadata = {Id = "b"}},
                    new Article {Metadata = {Id = "C"}}
                };
            _articleReader.ReadAllAsync().ReturnsForAnyArgs(Task.FromResult((IEnumerable<Article>)articles));
            var expectedMetadata = articles.Select(b => b.Metadata);

            await _sut.GetTotalCount();

            Assert.IsTrue(expectedMetadata.SequenceEqual(_articleCache.Metadata));
        }

        [Test]
        public async Task GetTotalCount__With_Initialised_Cache__Returns_Cache_Item_Count()
        {
            var cache = new List<ArticleMetadata>
                {
                    new ArticleMetadata { Id = "1" },
                    new ArticleMetadata { Id = "2" },
                    new ArticleMetadata { Id = "3" },
                    new ArticleMetadata { Id = "4" }
                };
            _articleCache.Metadata.Returns(cache);

            var totalCount = await _sut.GetTotalCount();

            Assert.AreEqual(4, totalCount);
        }

        [Test]
        public void FindAsync__With_Null_Id__Throws_Exception()
        {
            Assert.Throws<ArgumentException>(async () => await _sut.FindAsync(null));
        }

        [Test]
        public void FindAsync__With_Empty_Id__Throws_Excetion()
        {
            Assert.Throws<ArgumentException>(async () => await _sut.FindAsync(""));
        }

        [Test]
        public async Task FindAsync__With_Null_Cache__Reads_All_Articles()
        {
            const string articleId = "test";
            _articleCache.Metadata.Returns((List<ArticleMetadata>)null);

            await _sut.FindAsync(articleId);

            await Task.FromResult(_articleReader.Received(1).ReadAllAsync());
        }

        [Test]
        public async Task FindAsync__With_Non_Null_Cache__Doesnt_Read_All_Articles()
        {
            const string articleId = "test";
            _articleCache.Metadata.Returns(new List<ArticleMetadata>());

            await _sut.FindAsync(articleId);

            await Task.FromResult(_articleReader.DidNotReceive().ReadAllAsync());
        }

        [Test]
        public async Task FindAsync__With_Null_Cache__Sets_Cache_With_Result_From_Reader()
        {
            const string articleId = "test";
            _articleCache.Metadata.Returns((List<ArticleMetadata>)null);
            var articles = new List<Article>
                {
                    new Article {Metadata = {Id = "1"}},
                    new Article {Metadata = {Id = "b"}},
                    new Article {Metadata = {Id = "C"}}
                };
            _articleReader.ReadAllAsync().ReturnsForAnyArgs(Task.FromResult((IEnumerable<Article>)articles));
            var expectedMetadata = articles.Select(b => b.Metadata);

            await _sut.FindAsync(articleId);

            Assert.IsTrue(expectedMetadata.SequenceEqual(_articleCache.Metadata));
        }

        [Test]
        public async Task FindAsync__When_Finds_Id_In_Cache__Then_Calls_Reader_Only_Once()
        {
            const string articleId = "test";
            var cachedMetadata = new List<ArticleMetadata>
            {
                new ArticleMetadata {Id = "test"}
            };
            _articleCache.Metadata.Returns(cachedMetadata);

            await _sut.FindAsync(articleId);

            await Task.FromResult(_articleReader.ReceivedWithAnyArgs().ReadAsync(null));
        }

        [Test]
        public async Task FindAsync__When_Finds_Id_In_Cache__Then_Reads_Article()
        {
            const string articleId = "test";
            var cachedMetadata = new List<ArticleMetadata>
            {
                new ArticleMetadata {Id = "test"}
            };
            _articleCache.Metadata.Returns(cachedMetadata);

            await _sut.FindAsync(articleId);

            await Task.FromResult(_articleReader.Received().ReadAsync(articleId));
        }

        [Test]
        public async Task FindAsync__When_Cannot_Find_Id_In_Cache__Then_Returns_Null()
        {
            const string articleId = "test";
            var cachedMetadata = new List<ArticleMetadata>
            {
                new ArticleMetadata {Id = "test2"}
            };
            _articleCache.Metadata.Returns(cachedMetadata);

            var result = await _sut.FindAsync(articleId);

            Assert.IsNull(result);
        }

        [Test]
        public void FindAsync__When_Finds_Multiple_Entries_With_Same_Id_In_Cache__Then_Throws_Exception()
        {
            const string articleId = "test";
            var cachedMetadata = new List<ArticleMetadata>
            {
                new ArticleMetadata {Id = "test2"},
                new ArticleMetadata {Id = "test"},
                new ArticleMetadata {Id = "test"}
            };
            _articleCache.Metadata.Returns(cachedMetadata);

            Assert.Throws<InvalidOperationException>(async () => await _sut.FindAsync(articleId));
        }

        [Test]
        public async Task FindAsync__When_Finds_Id_In_Cache__Then_Returns_Result_From_Reader()
        {
            const string articleId = "test";
            var expected = new Article();
            var cachedMetadata = new List<ArticleMetadata> { new ArticleMetadata { Id = "test" } };
            _articleCache.Metadata.Returns(cachedMetadata);
            _articleReader.ReadAsync(null).ReturnsForAnyArgs(Task.FromResult(expected));

            var actual = await _sut.FindAsync(articleId);

            Assert.AreSame(expected, actual);
        }

        [Test]
        public async Task GetMostRecentAsync__With_Null_Cache__Reads_All_Articles()
        {
            const int page = 1;
            const int pageSize = 10;
            _articleCache.Metadata.Returns((List<ArticleMetadata>)null);

            await _sut.GetMostRecentAsync(page, pageSize);

            await Task.FromResult(_articleReader.Received(1).ReadAllAsync());
        }

        [Test]
        public async Task GetMostRecentAsync__With_Non_Null_Cache__Doesnt_Read_All_Articles()
        {
            const int page = 1;
            const int pageSize = 10;
            _articleCache.Metadata.Returns(new List<ArticleMetadata>());

            await _sut.GetMostRecentAsync(page, pageSize);

            await Task.FromResult(_articleReader.DidNotReceive().ReadAllAsync());
        }

        [Test]
        public async Task GetMostRecentAsync__With_Null_Cache__Sets_Cache_With_Result_From_Reader()
        {
            const int page = 1;
            const int pageSize = 10;
            _articleCache.Metadata.Returns((List<ArticleMetadata>)null);
            var articles = new List<Article>
                {
                    new Article {Metadata = {Id = "1"}},
                    new Article {Metadata = {Id = "b"}},
                    new Article {Metadata = {Id = "C"}}
                };
            _articleReader.ReadAllAsync().ReturnsForAnyArgs(Task.FromResult((IEnumerable<Article>)articles));
            var expectedMetadata = articles.Select(b => b.Metadata);

            await _sut.GetMostRecentAsync(page, pageSize);

            Assert.IsTrue(expectedMetadata.SequenceEqual(_articleCache.Metadata));
        }

        [Test]
        public async Task GetMostRecentAsync__With_Empty_Cache__Doesnt_Read_Any_Articles()
        {
            const int page = 1;
            const int pageSize = 3;
            _articleCache.Metadata.Returns(new List<ArticleMetadata>());

            await _sut.GetMostRecentAsync(page, pageSize);

            await Task.FromResult(_articleReader.DidNotReceiveWithAnyArgs().ReadAsync(null));
        }

        [Test]
        public async Task GetMostRecentAsync__With_Empty_Cache__Returns_Empty_List_Of_Articles()
        {
            const int page = 1;
            const int pageSize = 3;
            _articleCache.Metadata.Returns(new List<ArticleMetadata>());

            var actual = await _sut.GetMostRecentAsync(page, pageSize);

            Assert.IsEmpty(actual);
        }

        [Test]
        public async Task GetMostRecentAsync__With_More_Blog_Posts_Than_Page_Size__Reads_Correct_Number_Of_Articles_In_Descending_Order()
        {
            const int page = 1;
            const int pageSize = 3;
            var cachedMetadata = new List<ArticleMetadata>
                {
                    new ArticleMetadata {Id="1", PublishDateTime = DateTime.Now.AddDays(100)},
                    new ArticleMetadata {Id="2", PublishDateTime = DateTime.Now.AddDays(5)},
                    new ArticleMetadata {Id="3", PublishDateTime = DateTime.Now.AddDays(50)},
                    new ArticleMetadata {Id="4", PublishDateTime = DateTime.Now.AddDays(30)}
                };
            _articleCache.Metadata.Returns(cachedMetadata);

            await _sut.GetMostRecentAsync(page, pageSize);

            await Task.FromResult(_articleReader.ReceivedWithAnyArgs(3).ReadAsync(null));
            Received.InOrder(() =>
                {
                    _articleReader.ReadAsync("1");
                    _articleReader.ReadAsync("3");
                    _articleReader.ReadAsync("4");
                });
        }

        [Test]
        public async Task GetMostRecentAsync__When_Selecting_Second_Page__Reads_Correct_Number_Of_Articles_In_Descending_Order()
        {
            const int page = 2;
            const int pageSize = 2;
            var cachedMetadata = new List<ArticleMetadata>
                {
                    new ArticleMetadata {Id="1", PublishDateTime = DateTime.Now.AddDays(100)},
                    new ArticleMetadata {Id="2", PublishDateTime = DateTime.Now.AddDays(5)},
                    new ArticleMetadata {Id="3", PublishDateTime = DateTime.Now.AddDays(50)},
                    new ArticleMetadata {Id="4", PublishDateTime = DateTime.Now.AddDays(30)}
                };
            _articleCache.Metadata.Returns(cachedMetadata);

            await _sut.GetMostRecentAsync(page, pageSize);

            await Task.FromResult(_articleReader.ReceivedWithAnyArgs(2).ReadAsync(null));
            Received.InOrder(() =>
            {
                _articleReader.ReadAsync("4");
                _articleReader.ReadAsync("2");
            });
        }

        [Test]
        public async Task GetMostRecentAsync__With_Less_Blog_Posts_Than_Page_Size__Reads_Correct_Number_Of_Articles_In_Descending_Order()
        {
            const int page = 1;
            const int pageSize = 10;
            var cachedMetadata = new List<ArticleMetadata>
                {
                    new ArticleMetadata {Id="1", PublishDateTime = DateTime.Now.AddDays(100)},
                    new ArticleMetadata {Id="2", PublishDateTime = DateTime.Now.AddDays(5)},
                    new ArticleMetadata {Id="3", PublishDateTime = DateTime.Now.AddDays(50)},
                    new ArticleMetadata {Id="4", PublishDateTime = DateTime.Now.AddDays(30)}
                };
            _articleCache.Metadata.Returns(cachedMetadata);

            await _sut.GetMostRecentAsync(page, pageSize);

            await Task.FromResult(_articleReader.ReceivedWithAnyArgs(4).ReadAsync(null));
            Received.InOrder(() =>
            {
                _articleReader.ReadAsync("1");
                _articleReader.ReadAsync("3");
                _articleReader.ReadAsync("4");
                _articleReader.ReadAsync("2");
            });
        }

        [Test]
        public async Task GetMostRecentAsync__When_Finished_Reading_Articles__Returns_Result_From_Reader()
        {
            const int page = 1;
            const int pageSize = 10;
            _articleCache.Metadata.Returns(
                new List<ArticleMetadata>
                {
                    new ArticleMetadata(), new ArticleMetadata(), new ArticleMetadata()
                });
            var articles = new List<Article>
                {
                    new Article{Metadata = {Id="1"}},
                    new Article{Metadata = {Id="2"}},
                    new Article{Metadata = {Id="3"}}
                };
            _articleReader.ReadAsync(null).ReturnsForAnyArgs(
                Task.FromResult(articles.ElementAt(2)),
                Task.FromResult(articles.ElementAt(0)),
                Task.FromResult(articles.ElementAt(1)));
            var expected = new List<Article>
                {
                    articles.ElementAt(2),
                    articles.ElementAt(0),
                    articles.ElementAt(1)
                };

            var actual = await _sut.GetMostRecentAsync(page, pageSize);

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public async Task FindByTagAsync__With_Null_Tag__Throws_Exception()
        {
            Assert.Throws<ArgumentException>(async () => await _sut.FindByTagAsync(null));
        }

        [Test]
        public async Task FindByTagAsync__With_Empty_Tag__Throws_Exception()
        {
            Assert.Throws<ArgumentException>(async () => await _sut.FindByTagAsync(""));
        }

        [Test]
        public async Task FindByTagAsync__With_Null_Cache__Reads_All_Articles()
        {
            const string tag = "some-tag";
            _articleCache.Metadata.Returns((List<ArticleMetadata>)null);

            await _sut.FindByTagAsync(tag);

            await Task.FromResult(_articleReader.Received(1).ReadAllAsync());
        }

        [Test]
        public async Task FindByTagAsync__With_Non_Null_Cache__Doesnt_Read_All_Articles()
        {
            const string tag = "some-tag";
            _articleCache.Metadata.Returns(new List<ArticleMetadata>());

            await _sut.FindByTagAsync(tag);

            await Task.FromResult(_articleReader.DidNotReceive().ReadAllAsync());
        }

        [Test]
        public async Task FindByTagAsync__With_Null_Cache__Sets_Cache_With_Result_From_Reader()
        {
            const string tag = "some-tag";
            _articleCache.Metadata.Returns((List<ArticleMetadata>)null);
            var articles = new List<Article>
                {
                    new Article {Metadata = {Id = "1"}},
                    new Article {Metadata = {Id = "b"}},
                    new Article {Metadata = {Id = "C"}}
                };
            _articleReader.ReadAllAsync().ReturnsForAnyArgs(Task.FromResult((IEnumerable<Article>)articles));
            var expectedMetadata = articles.Select(b => b.Metadata);

            await _sut.FindByTagAsync(tag);

            Assert.IsTrue(expectedMetadata.SequenceEqual(_articleCache.Metadata));
        }

        [Test]
        public async Task FindByTagAsync__When_Cannot_Find_Articles_With_Tag_In_Cache__Doesnt_Read_Any_Articles()
        {
            const string tag = "some-tag";
            _articleCache.Metadata.Returns(new List<ArticleMetadata>());

            await _sut.FindByTagAsync(tag);

            await Task.FromResult(_articleReader.DidNotReceiveWithAnyArgs().ReadAsync(null));
        }

        [Test]
        public async Task FindByTagAsync__When_Cannot_Find_Articles_With_Tag_In_Cache__Returns_Empty_List_Of_Articles()
        {
            const string tag = "some-tag";
            _articleCache.Metadata.Returns(new List<ArticleMetadata>());

            var actual = await _sut.FindByTagAsync(tag);

            Assert.IsEmpty(actual);
        }

        [Test]
        public async Task FindByTagAsync__When_Can_Find_Articles_With_Tag__Reads_Correct_Articles_In_Descending_Order()
        {
            const string tag = "some-tag";
            var cachedMetadata = new List<ArticleMetadata>
                {
                    new ArticleMetadata {Id="1", Tags = new[]{ "some-tag" }, PublishDateTime = DateTime.Now.AddDays(1)},
                    new ArticleMetadata {Id="2", PublishDateTime = DateTime.Now.AddDays(5)},
                    new ArticleMetadata {Id="3", Tags = new[]{ "some-tag" }, PublishDateTime = DateTime.Now.AddDays(50)},
                    new ArticleMetadata {Id="4", Tags = new[]{ "other-tag" }, PublishDateTime = DateTime.Now.AddDays(30)}
                };
            _articleCache.Metadata.Returns(cachedMetadata);

            await _sut.FindByTagAsync(tag);

            await Task.FromResult(_articleReader.ReceivedWithAnyArgs(2).ReadAsync(null));
            Received.InOrder(() =>
            {
                _articleReader.ReadAsync("3");
                _articleReader.ReadAsync("1");
            });
        }

        [Test]
        public async Task FindByTagAsync__When_Finished_Reading_Articles__Returns_Result_From_Reader()
        {
            const string tag = "some-tag";
            _articleCache.Metadata.Returns(
                new List<ArticleMetadata>
                {
                    new ArticleMetadata {Tags = new[] {"some-tag"}},
                    new ArticleMetadata {Tags = new[] {"some-tag"}},
                    new ArticleMetadata {Tags = new[] {"some-tag"}}
                });
            var articles = new List<Article>
                {
                    new Article{Metadata = {Id="1"}},
                    new Article{Metadata = {Id="2"}},
                    new Article{Metadata = {Id="3"}}
                };
            _articleReader.ReadAsync(null).ReturnsForAnyArgs(
                Task.FromResult(articles.ElementAt(2)),
                Task.FromResult(articles.ElementAt(0)),
                Task.FromResult(articles.ElementAt(1)));
            var expected = new List<Article>
                {
                    articles.ElementAt(2),
                    articles.ElementAt(0),
                    articles.ElementAt(1)
                };

            var actual = await _sut.FindByTagAsync(tag);

            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}