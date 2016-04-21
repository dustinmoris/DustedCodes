using System;
using System.IO;
using System.Threading.Tasks;
using DustedCodes.Core.IO;
using Guardo;

namespace DustedCodes.Core.Data.StaticFileStorage
{
    public sealed class ArticleParser : IArticleParser
    {
        private readonly ITextReaderFactory _textReaderFactory;

        public ArticleParser(ITextReaderFactory textReaderFactory)
        {
            _textReaderFactory = textReaderFactory;
        }

        public async Task<Article> ParseAsync(FileInfo fileInfo)
        {
            Requires.NotNull(fileInfo, nameof(fileInfo));

            using (var textReader = _textReaderFactory.FromFile(fileInfo))
            {
                var article = await ParseMetadataAsync(textReader, fileInfo).ConfigureAwait(false);

                article.Content = await textReader.ReadToEndAsync().ConfigureAwait(false);
                article.Content = article.Content.Trim();

                if (article.Content.Length == 0)
                {
                    throw new FormatException($"Cannot parse the file '{fileInfo.FullName}' to an article, because there was no content.");   
                }

                article.Id = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

                return article;
            }
        }

        private static async Task<Article> ParseMetadataAsync(TextReader textReader, FileSystemInfo fileInfo)
        {
            var line = await textReader.ReadLineAsync().ConfigureAwait(false);

            if (line == null || !line.Equals("<!--"))
            {
                throw new FormatException($"Cannot parse the file '{fileInfo.FullName}' to an article. The first line has to begin with an XML comment tag '<!--'.");
            }

            var article = new Article();

            while ((line = await textReader.ReadLineAsync().ConfigureAwait(false)) != null && line != "-->")
            {
                var pair = line.Split(new[] { ':' }, 2, StringSplitOptions.None);

                if (pair.Length != 2)
                    continue;

                var key = pair[0].Trim().ToLower();
                var value = pair[1].Trim();

                switch (key)
                {
                    case "title":
                        article.Title = value;
                        break;

                    case "author":
                        article.Author = value;
                        break;

                    case "published":
                        DateTime publishDateTime;
                        if (DateTime.TryParse(value, out publishDateTime))
                        {
                            article.PublishDateTime = publishDateTime;
                        }
                        break;

                    case "tags":
                        var tags = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        article.Tags = tags;
                        break;
                }
            }

            if (line != "-->")
            {
                throw new FormatException($"Cannot parse the file '{fileInfo.FullName}' to an article. Couldn't find the closing tag of the meta data block.");
            }

            return article;
        }
    }
}