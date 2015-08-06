using System;
using System.IO;
using System.Threading.Tasks;
using DustedCodes.Core.IO;

namespace DustedCodes.Core.Data.LocalStorage
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
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");

            using (var textReader = _textReaderFactory.FromFile(fileInfo))
            {
                var article = await ParseMetadataAsync(textReader, fileInfo);

                article.Content = await textReader.ReadToEndAsync();
                article.Content = article.Content.Trim();

                if (article.Content.Length == 0)
                {
                    throw new FormatException(string.Format(
                        "Cannot parse the file '{0}' to an article, because there was no content.",
                        fileInfo.FullName));   
                }

                article.Id = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

                return article;
            }
        }

        private static async Task<Article> ParseMetadataAsync(TextReader textReader, FileSystemInfo fileInfo)
        {
            var line = await textReader.ReadLineAsync();

            if (line == null || !line.Equals("<!--"))
            {
                throw new FormatException(string.Format(
                    "Cannot parse the file '{0}' to an article. The first line has to begin with an XML comment tag '<!--'.",
                    fileInfo.FullName));
            }

            var article = new Article();

            while ((line = await textReader.ReadLineAsync()) != null && line != "-->")
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

                    case "lastedited":
                        DateTime lastEditedDateTime;
                        if (DateTime.TryParse(value, out lastEditedDateTime))
                        {
                            article.LastEditedDateTime = lastEditedDateTime;
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
                throw new FormatException(string.Format(
                    "Cannot parse the file '{0}' to an article. Couldn't find the closing tag of the meta data block.",
                    fileInfo.FullName));
            }

            return article;
        }
    }
}