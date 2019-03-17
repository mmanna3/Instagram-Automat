using System;
using Instagram_Automat.Utils;
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

        public static void ClickAndScrollIfException(this IWebElement element, YKNChromeDriver driver)      
        {
            //Execute(unaFuncion)
            //    .IfException(otraFuncion)
            //    .TiempoDeEsperaEntreIntentosEnSegundos(60)
            //    .CantidadDeVecesAntesDeAbortar(3);

            //try
            //{
            //    element.Click();                    //mati te ♥
            //}
            //catch (Exception)
            //{
            //    driver.Scroll(2000, 3000);
            //}
        }
    }
}
