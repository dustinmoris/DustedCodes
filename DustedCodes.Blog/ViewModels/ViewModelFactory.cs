using System.Collections.Generic;
using System.Linq;
using DustedCodes.Blog.Config;
using DustedCodes.Blog.Data;
using DustedCodes.Blog.Helpers;
using DustedCodes.Blog.ViewModels.Blog;
using DustedCodes.Blog.ViewModels.Home;

namespace DustedCodes.Blog.ViewModels
{
    public sealed class ViewModelFactory : IViewModelFactory
    {
        private readonly IAppConfig _appConfig;
        private readonly IUrlGenerator _urlGenerator;
        private readonly IUrlEncoder _urlEncoder;

        public ViewModelFactory(IAppConfig appConfig, IUrlGenerator urlGenerator, IUrlEncoder urlEncoder)
        {
            _appConfig = appConfig;
            _urlGenerator = urlGenerator;
            _urlEncoder = urlEncoder;
        }

        private ArticlePartialViewModel CreateArticlePartialViewModel(Article article, bool renderTitleAsLink)
        {
            var permalinkUrl = _urlGenerator.GeneratePermalinkUrl(article.Metadata.Id);
            var encodedPermalinkUrl = _urlEncoder.EncodeUrl(permalinkUrl);

            return new ArticlePartialViewModel
                {
                    Id = article.Metadata.Id,
                    Author = article.Metadata.Author,
                    Content = article.Content,
                    Title = article.Metadata.Title,
                    PermalinkUrl = permalinkUrl,
                    EditArticleUrl = string.Format(_appConfig.EditArticleUrlFormat, article.Metadata.Id),
                    UserFriendlyPublishDateTime = article.Metadata.PublishDateTime.ToString(_appConfig.DateTimeFormat),
                    ValidHtml5TPublishDateTime = article.Metadata.PublishDateTime.ToString(_appConfig.HtmlDateTimeFormat),
                    TwitterShareUrl = string.Format(_appConfig.TwitterShareUrlFormat, encodedPermalinkUrl),
                    GooglePlusShareUrl = string.Format(_appConfig.GooglePlusShareUrlFormat, encodedPermalinkUrl),
                    FacebookShareUrl = string.Format(_appConfig.FacebookShareUrlFormat, encodedPermalinkUrl),
                    YammerShareUrl = string.Format(_appConfig.YammerShareUrlFormat, encodedPermalinkUrl),
                    RenderTitleAsLink = renderTitleAsLink,
                    HasTags = article.Metadata.Tags != null && article.Metadata.Tags.Any(),
                    Tags = article.Metadata.Tags
                };
        }

        public AboutViewModel CreateAboutViewModel()
        {
            return new AboutViewModel
                {
                    BlogTitle = _appConfig.BlogTitle,
                    BlogDescription = _appConfig.BlogDescription,
                    DisqusShortname = _appConfig.DisqusShortname,
                    IsProductionEnvironment =  _appConfig.IsProductionEnvironment
                };
        }

        public ArticleViewModel CreateArticleViewModel(Article article)
        {
            var partialViewModel = CreateArticlePartialViewModel(article, false);

            return new ArticleViewModel
                {
                    Article = partialViewModel,
                    BlogTitle = _appConfig.BlogTitle,
                    BlogDescription = _appConfig.BlogDescription,
                    DisqusShortname = _appConfig.DisqusShortname,
                    IsProductionEnvironment = _appConfig.IsProductionEnvironment
                };
        }

        public IndexViewModel CreateIndexViewModel(IEnumerable<Article> articles, int totalPageCount, int currentPage)
        {
            var articlePartialViewModels = articles.Select(a => CreateArticlePartialViewModel(a, true));

            return new IndexViewModel
                {
                    Articles = articlePartialViewModels,
                    TotalPageCount = totalPageCount,
                    CurrentPage = currentPage,
                    BlogTitle = _appConfig.BlogTitle,
                    BlogDescription = _appConfig.BlogDescription,
                    DisqusShortname = _appConfig.DisqusShortname,
                    IsProductionEnvironment = _appConfig.IsProductionEnvironment
                };
        }

        public ArchiveViewModel CreateArchiveViewModel(IEnumerable<ArticleMetadata> articleMetadata)
        {
            return new ArchiveViewModel
                {
                    BlogTitle = _appConfig.BlogTitle,
                    BlogDescription = _appConfig.BlogDescription,
                    DisqusShortname = _appConfig.DisqusShortname,
                    IsProductionEnvironment = _appConfig.IsProductionEnvironment,
                    ArticleMetadata = articleMetadata
                };
        }
    }
}