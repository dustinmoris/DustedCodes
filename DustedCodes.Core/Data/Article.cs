using System;
using System.Collections.Generic;

namespace DustedCodes.Core.Data
{
    public sealed class Article
    {
        public Article(string id)
        {
            Id = id;
        }

        public string Id { get; }
        public string Author { get; set; }
        public string Title { get; set; }
        public DateTime PublishDateTime { get; set; }
        public ICollection<string> Tags { get; set; }
        public string Content { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            return ReferenceEquals(this, obj) || Equals(obj as Article);
        }

        public bool Equals(Article article)
        {
            return article != null && Id.Equals(article.Id);
        }
    }
}