using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace DustedCodes.Blog.ActionResults
{
    public abstract class FeedResult : FileResult
    {
        public readonly SyndicationFeedFormatter FeedFormatter;

        protected FeedResult(SyndicationFeedFormatter feedFormatter, string contentType) : base(contentType)
        {
            FeedFormatter = feedFormatter;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            using (var xmlWriter = XmlWriter.Create(response.OutputStream))
            {
                FeedFormatter.WriteTo(xmlWriter);
            }
        }
    }
}