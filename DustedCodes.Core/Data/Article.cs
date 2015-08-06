using System;
using System.Collections.Generic;

namespace DustedCodes.Core.Data
{
    public sealed class Article : IEquatable<Article>
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public DateTime PublishDateTime { get; set; }
        public DateTime LastEditedDateTime { get; set; }
        public ICollection<string> Tags { get; set; }
        public string Content { get; set; }

        // Article is considered to be equal if the Ids match:
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Article && Equals((Article) obj);
        }

        public bool Equals(Article other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || string.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }
    }
}