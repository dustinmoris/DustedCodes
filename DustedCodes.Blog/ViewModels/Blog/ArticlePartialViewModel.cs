using System.Collections.Generic;

namespace DustedCodes.Blog.ViewModels.Blog
{
    public class ArticlePartialViewModel
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string PermalinkUrl { get; set; }
        public string UserFriendlyPublishDateTime { get; set; }
        public string ValidHtml5TPublishDateTime { get; set; }
        public string EditArticleUrl { get; set; }
        public string TwitterShareUrl { get; set; }
        public string GooglePlusShareUrl { get; set; }
        public string FacebookShareUrl { get; set; }
        public string YammerShareUrl { get; set; }
        public bool RenderTitleAsLink { get; set; }
        public bool HasTags { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}