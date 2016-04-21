using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DustedCodes.Core.Data.Comparer;
using DustedCodes.Core.IO;
using Guardo;

namespace DustedCodes.Core.Data.StaticFileStorage
{
    public sealed class StaticFileArticleRepository : IArticleRepository
    {
        private readonly string _articleDirectoryPath;
        private readonly IArticleParser _articleParser;
        private readonly IDirectoryReader _directoryReader;

        public StaticFileArticleRepository(
            string articleDirectoryPath, 
            IArticleParser articleParser, 
            IDirectoryReader directoryReader)
        {
            _articleDirectoryPath = articleDirectoryPath;
            _articleParser = articleParser;
            _directoryReader = directoryReader;
        }

        public async Task<Article> GetAsync(string id)
        {
            Requires.NotNullOrEmpty(id, nameof(id));

            var filePath = $"{_articleDirectoryPath}\\{id}.html";
            var fileInfo = new FileInfo(filePath);

            return fileInfo.Exists 
                ? await _articleParser.ParseAsync(fileInfo).ConfigureAwait(false)
                : null;
        }

        public async Task<IEnumerable<Article>> GetOrderedByDateAsync()
        {
            const string searchPattern = "*.html";
            var files = _directoryReader.GetFiles(_articleDirectoryPath, searchPattern);

            if (files == null || files.Length == 0)
                return null;

            var articles = new SortedSet<Article>(new ByPublishDateDescending());

            foreach (var file in files)
            {
                var article = await _articleParser.ParseAsync(file).ConfigureAwait(false);
                articles.Add(article);
            }

            return articles;
        }
    }
}