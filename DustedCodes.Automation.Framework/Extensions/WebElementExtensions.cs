using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Extensions
{
    public static class WebElementExtensions
    {
        public static IWebElement GetParent(this IWebElement webElement)
        {
            return webElement.FindElement(By.XPath(".."));
        }
    }
}