using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace DustedCodes.Automation.Framework
{
    internal static class Driver
    {
        public static IWebDriver Instance { get; private set; }

        public static void Init()
        {
            Instance = new FirefoxDriver();

            // Give the page a maximum of 5 seconds to load when requesting elements
            Instance.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
        }
    }
}