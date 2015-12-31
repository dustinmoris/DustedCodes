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

        public static IEnumerable<BlogPost> BlogPosts => new[] 
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
                Tags = new[] { "windows-azure", "app-hosting-plan" }
            },
            new BlogPost
            {
                Title = "The beauty of asymmetric encryption - RSA crash course for developers",
                PermalinkId = "the-beauty-of-asymmetric-encryption-rsa-crash-course-for-developers",
                Tags = new[] { "security", "rsa", "asymmetric-encryption", "cryptography" }
            },
            new BlogPost
            {
                Title = "How to use RSA in .NET: RSACryptoServiceProvider vs. RSACng and good practise patterns",
                PermalinkId = "how-to-use-rsa-in-dotnet-rsacryptoserviceprovider-vs-rsacng-and-good-practise-patterns",
                Tags = new[] { "dotnet", "rsa", "security", "asymmetric-encryption", "cryptography" }
            },
            new BlogPost
            {
                Title = "Using C# 6 features in ASP.NET MVC 5 razor views",
                PermalinkId = "using-csharp-6-features-in-aspdotnet-mvc-5-razor-views",
                Tags = new[] { "asp-net", "mvc-5", "csharp-6", "razor" }
            },
            new BlogPost
            {
                Title = "Display build history charts for AppVeyor or TravisCI builds with an SVG widget",
                PermalinkId = "display-build-history-charts-for-appveyor-or-travisci-builds-with-an-svg-widget",
                Tags = new[] { "appveyor", "travisci", "github", "svg" }
            },
            new BlogPost
            {
                Title = "Death of a QA in Scrum",
                PermalinkId = "death-of-a-qa-in-scrum",
                Tags = new[] { "scrum", "agile", "testing" }
            },
            new BlogPost
            {
                Title = "When to use Scrum? Waterfall vs. Scrum vs. Kanban vs. Scrumban",
                PermalinkId = "when-to-use-scrum-waterfall-vs-scrum-vs-kanban-vs-scrumban",
                Tags = new[] { "scrum", "kanban", "scrumban", "waterfall", "agile" }
            },
            new BlogPost
            {
                Title = "How to make an IStatusCodeHandler portable in NancyFx",
                PermalinkId = "how-to-make-an-istatuscodehandler-portable-in-nancyfx",
                Tags = new[] { "nancyfx", "architecture" }
            },
            new BlogPost
            {
                Title = "Design, test and document RESTful APIs using RAML in .NET",
                PermalinkId = "design-test-and-document-restful-apis-using-raml-in-dotnet",
                Tags = new[] { "raml", "restful-api", "dotnet", "anypoint" }
            },
            new BlogPost
            {
                Title = "Diagnosing CSS issues on mobile devices",
                PermalinkId = "diagnosing-css-issues-on-mobile-devices",
                Tags = new[] { "css" }
            }
        };
    }
}