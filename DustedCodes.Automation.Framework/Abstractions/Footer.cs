using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Abstractions
{
    public static class Footer
    {
        public static void GoToAbout()
        {
            Driver.Instance.FindElement(By.LinkText("Read more...")).Click();
        }

    }
}