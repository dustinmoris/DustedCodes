using System.Net;

namespace DustedCodes.Core.Web
{
    public sealed class UrlEncoder : IUrlEncoder
    {
        public string EncodeUrl(string url)
        {
            return WebUtility.UrlEncode(url);
        }
    }
}