using OpenQA.Selenium;

namespace DustedCodes.Automation.Framework.Pages
{
    public class AboutPage : BasePage
    {
        public bool IsAt()
        {
            var title = Driver.Instance.FindElement(By.CssSelector("#about > h1"));

            return title.Text == "About";
        }
    }
}
