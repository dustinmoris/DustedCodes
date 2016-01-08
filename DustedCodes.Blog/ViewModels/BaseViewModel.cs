using DustedCodes.Blog.Config;

namespace DustedCodes.Blog.ViewModels
{
    public abstract class BaseViewModel
    {
        private readonly IAppConfig _appConfig;
        private readonly string _pageName;
        private const int CssVersion = 16;

        protected BaseViewModel(IAppConfig appConfig, string pageName = null)
        {
            _appConfig = appConfig;
            _pageName = pageName;
        }

        public virtual string PageTitle => $"{_pageName} - {BlogTitle}";
        public string BlogTitle => _appConfig.BlogTitle;
        public string BlogDescription => _appConfig.BlogDescription;
        public string DisqusShortname => _appConfig.DisqusShortname;
        public bool IsProductionEnvironment => _appConfig.IsProductionEnvironment;
        public string RelativeCssFilePath => $"Content/Css/site{(IsDebugMode() ? "" : ".min")}.css?v={CssVersion}";

        private static bool IsDebugMode()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}