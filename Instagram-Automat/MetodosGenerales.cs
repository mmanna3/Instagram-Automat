using System.Threading;
using Instagram_Automat.ExtensionMethods;
using Instagram_Automat.Models;
using Instagram_Automat.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Instagram_Automat
{
	public static class MetodosGenerales
	{
		public static void IrAlPerfilDelUsuario(IWebDriver browser, string nombreDeUsuario)
		{
			browser.Url = $"http://www.instagram.com/{nombreDeUsuario}";
		}

		public static bool PantallaActivaEsPerfilDelUsuarioLogueado(IWebDriver browser)
		{
			return browser.IsElementDisplayed(By.XPath("//button[contains(text(), 'Following')]"));
		}

        public static void RechazarOfrecimientos(IWebDriver browser)
		{
            if (browser.IsElementDisplayed(By.XPath("//button[contains(text(), 'Close')]")))
            {
                browser.FindElement(By.XPath("//button[contains(text(), 'Close')]")).Click();;                
                Thread.Sleep(4000);
            }
            if (browser.IsElementDisplayed(By.XPath("//button[contains(text(), 'Not Now')]")))
			{
				browser.FindElement(By.XPath("//button[contains(text(), 'Not Now')]")).Click();				
				Thread.Sleep(4000);
			}
			if (browser.IsElementDisplayed(By.XPath("//[contains(text(), 'Go to the App')]")))
			{
				browser.FindElement(By.XPath("//button[contains(text(), 'Go to the')]")).Click();
				Thread.Sleep(4000);
			}
			if (browser.IsElementDisplayed(By.XPath("//button[contains(text(), 'Cancel')]")))
			{
				browser.FindElement(By.XPath("//button[contains(text(), 'Cancel')]")).Click();				
				Thread.Sleep(4000);
			}			
		}

		public static YKNChromeDriver Browser()
		{
			var chromeOptions = new ChromeOptions();
			chromeOptions.EnableMobileEmulation("iPhone 4");
			//chromeOptions.AddArguments("headless");

			return new YKNChromeDriver(chromeOptions);
		}
	}
}
