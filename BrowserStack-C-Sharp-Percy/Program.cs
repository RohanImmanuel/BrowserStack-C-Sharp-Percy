using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using BrowserStackPercy;

namespace SeleniumTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver;
            ChromeOptions capability = new ChromeOptions();
            capability.AddAdditionalCapability("os", "Windows", true);
            capability.AddAdditionalCapability("os_version", "10", true);
            capability.AddAdditionalCapability("browser", "Chrome", true);
            capability.AddAdditionalCapability("browser_version", "latest", true);
            capability.AddAdditionalCapability("browserstack.local", "false", true);
            capability.AddAdditionalCapability("browserstack.selenium_version", "3.14.0", true);
            capability.AddAdditionalCapability("build", "BrowserStack Percy C#", true);
            capability.AddAdditionalCapability("browserstack.user", "username", true);
            capability.AddAdditionalCapability("browserstack.key", "key", true);

            driver = new RemoteWebDriver(
              new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capability
            );
            driver.Navigate().GoToUrl("https://www.google.com");
            IWebElement query = driver.FindElement(By.Name("q"));
            query.SendKeys("Browserstack");
            query.Submit();

            Percy percy = new Percy(driver);
            percy.snapshot("percy test");

            driver.Quit();
        }
    }
}