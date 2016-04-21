using System.Runtime.Caching;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using DustedCodes.Blog.Controllers;
using DustedCodes.Blog.Helpers;
using DustedCodes.Blog.ViewModels;
using DustedCodes.Core.Analytics;
using DustedCodes.Core.Caching;
using DustedCodes.Core.Data;
using DustedCodes.Core.Data.StaticFileStorage;
using DustedCodes.Core.Feeds;
using DustedCodes.Core.IO;
using DustedCodes.Core.Services;
using DustedCodes.Core.Web;

namespace DustedCodes.Blog.Config
{
    public static class DependencyConfig
    {
        public static void Setup()
        {
            var builder = new ContainerBuilder();
            var appConfig = new AppConfig();

            if (appConfig.UseCache)
            {
                builder.RegisterType<ObjectCacheWrapper>()
                    .As<ICache>()
                    .WithParameter("objectCache", MemoryCache.Default)
                    .WithParameter("defaultCacheItemPolicy", new CacheItemPolicy());
            }
            else
            {
                builder.RegisterType<NullCache>().As<ICache>();
            }

            builder.RegisterType<AppConfig>().As<IAppConfig>().InstancePerDependency();
            builder.RegisterType<TextReaderFactory>().As<ITextReaderFactory>().InstancePerDependency();
            builder.RegisterType<ArticleParser>().As<IArticleParser>().InstancePerDependency();

            builder.RegisterType<StaticFileArticleRepository>()
                .As<IArticleRepository>()
                .WithParameter("articleDirectoryPath", appConfig.ArticlesDirectoryPath)
                .AsSelf();

            builder.RegisterType<CachedArticleRepository>()
                .As<IArticleRepository>()
                .WithParameter(
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(IArticleRepository),
                        (pi, ctx) => ctx.Resolve<StaticFileArticleRepository>()))
                .AsSelf();

            builder.RegisterType<GoogleAnalyticsClient>()
                .As<IGoogleAnalyticsClient>()
                .WithParameter("privateKeyPath", appConfig.GoogleAnalyticsPrivateKeyPath)
                .WithParameter("viewId", appConfig.GoogleAnalyticsViewId)
                .AsSelf();

            builder.RegisterType<CachedGoogleAnalyticsClient>()
                .As<IGoogleAnalyticsClient>()
                .WithParameter(
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(IGoogleAnalyticsClient),
                        (pi, ctx) => ctx.Resolve<GoogleAnalyticsClient>()))
                .AsSelf();

            builder.RegisterType<FeedItemConverter>().As<IFeedItemConverter>();

            builder.RegisterType<FeedFactory>().As<IFeedFactory>()
                .WithParameter("feedTitle", appConfig.BlogTitle)
                .WithParameter("feedDescription", appConfig.BlogDescription)
                .WithParameter("maxItemCount", appConfig.FeedMaxItemCount);

            builder.RegisterType<UrlEncoder>().As<IUrlEncoder>();
            builder.RegisterType<UrlGenerator>().As<IUrlGenerator>();
            builder.RegisterType<DirectoryReader>().As<IDirectoryReader>();

            builder.RegisterType<ArticleService>()
                .As<IArticleService>()
                .WithParameter(
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(IArticleRepository),
                        (pi, ctx) => ctx.Resolve<CachedArticleRepository>()));

            builder.RegisterType<ViewModelFactory>().As<IViewModelFactory>();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<BlogController>().AsSelf().WithParameter("pageSize", appConfig.BlogPageSize);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }
    }
}