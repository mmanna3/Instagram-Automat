using System.Threading;
using Instagram_Automat.ExtensionMethods;
using Instagram_Automat.Models;
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
			return browser.IsElementDisplayed(By.XPath("//button[contains(text(), 'Edit Profile')]"));
		}

		public static void RechazarOfrecimientos(IWebDriver browser)
		{
			if (browser.IsElementDisplayed(By.XPath("//button[contains(text(), 'Not Now')]")))
			{
				var botonRechazar = browser.FindElement(By.XPath("//button[contains(text(), 'Not Now')]"));
				botonRechazar.Click();
				Thread.Sleep(4000);
			}
			if (browser.IsElementDisplayed(By.XPath("//[contains(text(), 'Go to the App')]")))
			{
				var botonRechazar = browser.FindElement(By.XPath("//button[contains(text(), 'Go to the')]"));
				botonRechazar.Click();
				Thread.Sleep(4000);
			}
			if (browser.IsElementDisplayed(By.XPath("//button[contains(text(), 'Cancel')]")))
			{
				var botonRechazar = browser.FindElement(By.XPath("//button[contains(text(), 'Cancel')]"));
				botonRechazar.Click();
				Thread.Sleep(4000);
			}			
		}

		public static ChromeDriver Browser()
		{
			var chromeOptions = new ChromeOptions();
			chromeOptions.EnableMobileEmulation("iPhone 4");
			//chromeOptions.AddArguments("headless");

			return new ChromeDriver(chromeOptions);
		}
	}
}
