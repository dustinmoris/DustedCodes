using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Pages
{
    public static class BasePage
    {
        public static void GoToUrl(string url)
        {
            Driver.Instance.Navigate().GoToUrl(url);
        }

        public static void GoToHome()
        {
            Driver.Instance.FindElement(By.LinkText("DUSTED CODES")).Click();
        }

        public static void GoToBlog()
        {
            Driver.Instance.FindElement(By.LinkText("BLOG")).Click();
        }

        public static void GoToArchive()
        {
            Driver.Instance.FindElement(By.LinkText("ARCHIVE")).Click();
        }

        public static void GoToAbout()
        {
            Driver.Instance.FindElement(By.LinkText("ABOUT")).Click();
        }

        public static void GoToRssFeed()
        {
            Driver.Instance.FindElement(By.LinkText("RSS")).Click();
        }

        public static void GoToAtomFeed()
        {
            Driver.Instance.FindElement(By.LinkText("ATOM")).Click();
        }
    }
}