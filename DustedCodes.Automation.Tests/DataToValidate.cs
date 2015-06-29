using System.Collections.Generic;

namespace DustedCodes.Automation.Tests
{
    public static class DataToValidate
    {
        public class BlogPost
        {
            public string Title { get; set; }
            public string PermalinkId { get; set; }
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
                        Title = "Hello World",
                        PermalinkId = "hello-world"
                    },
                    new BlogPost
                    {
                        Title = "PHP UK Conference 2015",
                        PermalinkId = "php-uk-conference-2015",
                        Tags = new[] { "php-uk","versioning", "hhvm" }
                    },
                    new BlogPost
                    {
                        Title = "Making Font Awesome awesome - Using icons without i-tags",
                        PermalinkId = "making-font-awesome-awesome-using-icons-without-i-tags",
                        Tags = new[] { "font-awesome", "css" }
                    },
                    new BlogPost
                    {
                        Title = "Guard clauses without test coverage, a common TDD pitfall",
                        PermalinkId = "guard-clauses-without-test-coverage-a-common-tdd-pitfall",
                        Tags = new[] { "tdd", "guard-clauses" }
                    },
                    new BlogPost
                    {
                        Title = "Demystifying ASP.NET MVC 5 Error Pages and Error Logging",
                        PermalinkId = "demystifying-aspnet-mvc-5-error-pages-and-error-logging",
                        Tags = new[] { "asp-net", "mvc", "error-pages", "error-logging" }
                    },
                    new BlogPost
                    {
                        Title = "Effective defense against distributed brute force attacks",
                        PermalinkId = "effective-defense-against-distributed-brute-force-attacks",
                        Tags = new[] { "security", "brute-force-attacks" }
                    },
                    new BlogPost
                    {
                        Title = "Running free tier and paid tier web apps on the same Microsoft Azure subscription",
                        PermalinkId = "running-free-tier-and-paid-tier-web-apps-on-the-same-microsoft-azure-subscription",
                        Tags = new[] { "microsoft-azure", "app-hosting-plan" }
                    },
                    new BlogPost
                    {
                        Title = "The beauty of asymmetric encryption - RSA crash course for developers",
                        PermalinkId = "the-beauty-of-asymmetric-encryption-rsa-crash-course-for-developers",
                        Tags = new[] { "security", "rsa", "asymmetric-encryption", "cryptography" }
                    }
                };
            }
        }
    }
}