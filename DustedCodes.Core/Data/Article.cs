using System;
using System.Collections.Generic;

namespace DustedCodes.Core.Data
{
    public sealed class Article
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public DateTime PublishDateTime { get; set; }
        public DateTime LastEditedDateTime { get; set; }
        public ICollection<string> Tags { get; set; }
        public string Content { get; set; }
    }
}