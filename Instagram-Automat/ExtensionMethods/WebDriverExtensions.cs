using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Instagram_Automat.ExtensionMethods
{
	public static class WebDriverExtensions
	{
		public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
		{
			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
			return wait.Until(ExpectedConditions.ElementExists(by));
		}

		public static bool IsElementDisplayed(this IWebDriver driver, By by)
		{
			try
			{
				driver.FindElement(by);
				return true;
			}
			catch (NoSuchElementException)
			{
				return false;
			}
		}
	}
}
