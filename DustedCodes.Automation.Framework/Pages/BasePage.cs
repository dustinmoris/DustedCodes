using DustedCodes.Automation.Framework.Feeds;
using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Pages
{
    public class BasePage
    {
        public void GoToUrl(string url)
        {
            Driver.Instance.Navigate().GoToUrl(url);
        }

        public HomePage GoToHome()
        {
            Driver.Instance.FindElement(By.LinkText("DUSTED CODES")).Click();
            
            return new HomePage();
        }

        public HomePage GoToBlog()
        {
            Driver.Instance.FindElement(By.LinkText("BLOG")).Click();

            return new HomePage();
        }

        public ArchivePage GoToArchive()
        {
            Driver.Instance.FindElement(By.LinkText("ARCHIVE")).Click();

            return new ArchivePage();
        }

        public AboutPage GoToAbout()
        {
            Driver.Instance.FindElement(By.LinkText("ABOUT")).Click();

            return new AboutPage();
        }

        public RssFeed GoToRssFeed()
        {
            Driver.Instance.FindElement(By.LinkText("RSS FEED")).Click();

            return new RssFeed();
        }

        public AtomFeed GoToAtomFeed()
        {
            Driver.Instance.FindElement(By.LinkText("ATOM FEED")).Click();

            return new AtomFeed();
        }
    }
}