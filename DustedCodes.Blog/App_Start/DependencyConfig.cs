using System.Runtime.Caching;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using DustedCodes.Blog.Config;
using DustedCodes.Blog.Controllers;
using DustedCodes.Blog.Feeds;
using DustedCodes.Blog.Helpers;
using DustedCodes.Blog.ViewModels;
using DustedCodes.Core.Caching;
using DustedCodes.Core.Data;
using DustedCodes.Core.Data.LocalStorage;
using DustedCodes.Core.IO;
using DustedCodes.Core.Services;

namespace DustedCodes.Blog
{
    public static class DependencyConfig
    {
        public static void Setup()
        {
            var builder = new ContainerBuilder();
            var appConfig = new AppConfig();

            builder.RegisterType<AppConfig>().As<IAppConfig>().InstancePerDependency();
            builder.RegisterType<TextReaderFactory>().As<ITextReaderFactory>().InstancePerDependency();
            builder.RegisterType<ArticleParser>().As<IArticleParser>().InstancePerDependency();
            builder.RegisterType<StaticFileArticleRepository>().As<IArticleRepository>()
                .WithParameter("articleDirectoryPath", appConfig.ArticlesDirectoryPath);
            builder.RegisterType<FeedItemConverter>().As<IFeedItemConverter>();
            builder.RegisterType<FeedFactory>().As<IFeedFactory>()
                .WithParameter("maxItemCount", appConfig.FeedMaxItemCount);
            builder.RegisterType<UrlEncoder>().As<IUrlEncoder>();
            builder.RegisterType<UrlGenerator>().As<IUrlGenerator>();
            builder.RegisterType<DirectoryReader>().As<IDirectoryReader>();
            builder.RegisterType<ArticleService>().As<IArticleService>();
            builder.RegisterType<ViewModelFactory>().As<IViewModelFactory>();

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

            builder.RegisterType<BlogController>().AsSelf().WithParameter("pageSize", appConfig.BlogPageSize);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }
    }
}