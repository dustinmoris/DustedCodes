using System.Web;

namespace DustedCodes.Blog.Helpers
{
    public class UrlEncoder : IUrlEncoder
    {
        public string EncodeUrl(string url)
        {
            return HttpUtility.UrlEncode(url);
        }
    }
}