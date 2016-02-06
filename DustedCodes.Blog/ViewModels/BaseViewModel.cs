using DustedCodes.Blog.Configuration;

namespace DustedCodes.Blog.ViewModels
{
    public abstract class BaseViewModel
    {
        private readonly IAppConfig _appConfig;
        private readonly string _pageTitle;
        private const int CssVersion = 20;
        private const int JavaScriptVersion = 5;

        protected BaseViewModel(IAppConfig appConfig, string pageTitle = null)
        {
            _appConfig = appConfig;
            _pageTitle = pageTitle;
        }

        public virtual string PageTitle => $"{_pageTitle} - {BlogTitle}";
        public string BlogTitle => _appConfig.BlogTitle;
        public string BlogDescription => _appConfig.BlogDescription;
        public string DisqusShortname => _appConfig.DisqusShortname;
        public bool IsProductionEnvironment => _appConfig.IsProductionEnvironment;
        public string RelativeCssFilePath => $"Assets/Css/site{(IsDebugMode() ? "" : ".min")}.css?v={CssVersion}";
        public string RelativeJavaScriptFilePath => $"Assets/Scripts/main{(IsDebugMode() ? "" : ".min")}.js?v={JavaScriptVersion}";

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