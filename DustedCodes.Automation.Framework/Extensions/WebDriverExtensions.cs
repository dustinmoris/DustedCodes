using System.Linq;
using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Extensions
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElementOrNull(this IWebDriver webDriver, By by)
        {
            var elements = webDriver.FindElements(by);

            return elements.Count == 0 ? null : elements.First();
        }
    }
}