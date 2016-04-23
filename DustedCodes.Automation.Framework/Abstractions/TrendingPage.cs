using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Abstractions
{
    public static class TrendingPage
    {
        public static bool IsAt()
        {
            var title = Driver.Instance.FindElement(By.CssSelector("body > main > article > h1"));
            return title.Text == "Top 10 articles by popularity";
        }
    }
}