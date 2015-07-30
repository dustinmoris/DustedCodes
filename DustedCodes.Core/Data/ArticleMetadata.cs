using System;
using System.Collections.Generic;

namespace DustedCodes.Core.Data
{
    public sealed class ArticleMetadata
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public DateTime PublishDateTime { get; set; }
        public DateTime LastEditedDateTime { get; set; }
        public ICollection<string> Tags { get; set; }
    }
}