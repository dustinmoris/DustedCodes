using System;
using System.IO;
using System.Threading.Tasks;

namespace DustedCodes.Blog.Data.LocalStorage
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
            using (var textReader = _textReaderFactory.FromFile(fileInfo))
            {
                var article = new Article
                {
                    Metadata = await ParseMetadataAsync(textReader, fileInfo),
                    Content = await textReader.ReadToEndAsync()
                };

                article.Metadata.Id = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

                return article;
            }
        }

        private static async Task<ArticleMetadata> ParseMetadataAsync(TextReader textReader, FileSystemInfo fileInfo)
        {
            var line = await textReader.ReadLineAsync();

            if (line == null || !line.Equals("<!--"))
            {
                throw new FormatException(string.Format(
                    "Cannot parse the file '{0}' to an article. The first line has to begin with an XML comment tag '<!--'.",
                    fileInfo.FullName));
            }

            var articleMetadata = new ArticleMetadata();

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
                        articleMetadata.Title = value;
                        break;

                    case "author":
                        articleMetadata.Author = value;
                        break;

                    case "published":
                        DateTime publishDateTime;
                        if (DateTime.TryParse(value, out publishDateTime))
                        {
                            articleMetadata.PublishDateTime = publishDateTime;
                        }
                        break;

                    case "lastedited":
                        DateTime lastEditedDateTime;
                        if (DateTime.TryParse(value, out lastEditedDateTime))
                        {
                            articleMetadata.LastEditedDateTime = lastEditedDateTime;
                        }
                        break;

                    case "tags":
                        var tags = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        articleMetadata.Tags = tags;
                        break;
                }
            }

            return articleMetadata;
        }
    }
}