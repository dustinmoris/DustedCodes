using System;
using System.Web;
using DustedCodes.Blog.Config;
using Lanem;
using SharpRaven;
using SharpRaven.Data;

namespace DustedCodes.Blog.Modules
{
    public class SentryErrorHandlerModule : ErrorHandlerModule
    {
        private readonly IRavenClient _ravenClient;
        private readonly IAppConfig _appConfig;

        public SentryErrorHandlerModule()
        {
            _appConfig = new AppConfig();
            _ravenClient = new RavenClient(_appConfig.SentryDsn);
        }

        protected override void LogError(HttpApplication application, Exception exception)
        {
            if (_appConfig.IsProductionEnvironment)
            {
                exception.Data.Add("request-url", application.Request.RawUrl);
                _ravenClient.Capture(new SentryEvent(exception));
            }
            else
            {
                base.LogError(application, exception);
            }
        }
    }
}