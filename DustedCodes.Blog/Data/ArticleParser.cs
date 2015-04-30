using System;
using System.IO;
using System.Threading.Tasks;
using DustedCodes.Blog.Helpers;

namespace DustedCodes.Blog.Data
{
    public sealed class ArticleParser : IArticleParser
    {
        private readonly IUrlGenerator _permalinkGenerator;

        public ArticleParser(IUrlGenerator permalinkGenerator)
        {
            _permalinkGenerator = permalinkGenerator;
        }

        public async Task<Article> ParseAsync(string id, string rawContent)
        {
            var permalinkUrl = _permalinkGenerator.GeneratePermalinkUrl(id);

            var article = new Article
            {
                Metadata = { Id = id },
                Content = rawContent,
                PermalinkUrl = permalinkUrl
            };

            using (var textReader = new StringReader(rawContent))
            {
                var line = await textReader.ReadLineAsync();

                if (line == null || !line.Equals("<!--"))
                    return article;

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
                            article.Metadata.Title = value;
                            break;

                        case "author":
                            article.Metadata.Author = value;
                            break;

                        case "published":
                            DateTime publishDateTime;
                            if (DateTime.TryParse(value, out publishDateTime))
                            {
                                article.Metadata.PublishDateTime = publishDateTime;
                            }
                            break;

                        case "lastedited":
                            DateTime lastEditedDateTime;
                            if (DateTime.TryParse(value, out lastEditedDateTime))
                            {
                                article.Metadata.LastEditedDateTime = lastEditedDateTime;
                            }
                            break;

                        case "tags":
                            var tags = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            article.Metadata.Tags = tags;
                            break;
                    }
                }

                article.Content = await textReader.ReadToEndAsync();
            }

            return article;
        }
    }
}