using System.Collections.Generic;

namespace DustedCodes.Automation.Tests
{
    public static class DataToValidate
    {
        public class BlogPost
        {
            public string Title { get; set; }
            public IEnumerable<string> Tags { get; set; }
        }

        public static IEnumerable<BlogPost> BlogPosts
        {
            get
            {
                return new[] 
                {
                    new BlogPost 
                    {
                        Title = "Hello World"
                    },
                    new BlogPost
                    {
                        Title = "PHP UK Conference 2015", 
                        Tags = new[] { "php-uk","versioning", "hhvm" }
                    },
                    new BlogPost
                    {
                        Title = "Making Font Awesome awesome - Using icons without i-tags", 
                        Tags = new[] { "font-awesome", "css" }
                    },
                    new BlogPost
                    {
                        Title = "Guard clauses without test coverage, a common TDD pitfall", 
                        Tags = new[] { "tdd", "guard-clauses" }
                    },
                    new BlogPost
                    {
                        Title = "Demystifying ASP.NET MVC 5 Error Pages and Error Logging", 
                        Tags = new[] { "asp-net", "mvc", "error-pages", "error-logging" }
                    }
                };
            }
        }
    }
}