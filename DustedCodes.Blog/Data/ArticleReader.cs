using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DustedCodes.Blog.IO;

namespace DustedCodes.Blog.Data
{
    public sealed class ArticleReader : IArticleReader
    {
        private readonly IDirectoryReader _directoryReader;
        private readonly IFilesReader _filesReader;
        private readonly IArticleParser _articleParser;
        private readonly string _articleDirectoryPath;

        public ArticleReader(
            IDirectoryReader directoryReader, 
            IFilesReader filesReader, 
            IArticleParser articleParser, 
            string articleDirectoryPath)
        {
            _filesReader = filesReader;
            _articleParser = articleParser;
            _articleDirectoryPath = articleDirectoryPath;
            _directoryReader = directoryReader;
        }

        public async Task<Article> ReadAsync(string articleId)
        {
            if (string.IsNullOrEmpty(articleId))
                throw new ArgumentException("articleId cannot be null or empty.");

            var filePath = string.Format("{0}\\{1}.html", _articleDirectoryPath, articleId);
            var rawContent = await _filesReader.ReadAllTextAsync(filePath);

            if (string.IsNullOrEmpty(rawContent))
                return null;

            return await _articleParser.ParseAsync(articleId, rawContent);
        }

        public async Task<IEnumerable<Article>> ReadAllAsync()
        {
            const string searchPattern = "*.html";
            var files = _directoryReader.GetFiles(_articleDirectoryPath, searchPattern);

            if (files == null || files.Length == 0)
                return null;

            var articles = new List<Article>();

            foreach (var file in files)
            {
                var rawContent = await _filesReader.ReadAllTextAsync(file.FullName);

                if (string.IsNullOrEmpty(rawContent))
                    continue;

                var id = file.Name.Replace(file.Extension, string.Empty);
                var article = await _articleParser.ParseAsync(id, rawContent);

                articles.Add(article);
            }

            return articles.OrderByDescending(a => a.Metadata.PublishDateTime);
        }
    }
}