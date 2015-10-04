using DustedCodes.Blog.Config;

namespace DustedCodes.Blog.ViewModels
{
    public abstract class BaseViewModel
    {
        private readonly IAppConfig _appConfig;
        private readonly string _pageName;

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
    }
}