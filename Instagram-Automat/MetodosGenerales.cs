using System;
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
		public static void IrAlPerfilDelUsuarioLogueado(IWebDriver browser, Usuario usuario)
		{
			if (!PantallaActivaEsPerfilDelUsuarioLogueado(browser, usuario))
				browser.Url = $"http://www.instagram.com/{usuario.NombreDeUsuario}";
		}

		public static void IrAlPerfilDelUsuario(IWebDriver browser, string nombreDeUsuario)
		{
			browser.Url = $"http://www.instagram.com/{nombreDeUsuario}";
		}

		public static bool PantallaActivaEsPerfilDelUsuarioLogueado(IWebDriver browser, Usuario usuario)
		{
			return browser.IsElementDisplayed(By.XPath($"//a[contains(@href, '/{usuario.NombreDeUsuario}/followers/')]"));
        }

        public static void RechazarOfrecimientos(YKNChromeDriver browser)
        {
	        browser.ClickIfDisplayedAndWait(By.XPath("//[contains(text(), 'Close')]"), 3, 5);
	        browser.ClickIfDisplayedAndWait(By.XPath("//button[contains(text(), 'Not Now')]"), 3, 5);
			browser.ClickIfDisplayedAndWait(By.XPath("//[contains(text(), 'Go to the App')]"), 3, 5);
			browser.ClickIfDisplayedAndWait(By.XPath("//button[contains(text(), 'Cancel')]"), 3, 5);
		}

        public static IWebElement LinkSeguidores(YKNChromeDriver browser, string nombreDeUsuario)
        {
            return browser.FindElement(By.XPath($"//a[contains(@href, '/{nombreDeUsuario}/followers/')]"));
        }

        public static IWebElement LinkSeguidos(YKNChromeDriver browser, string nombreDeUsuario)
        {
            return browser.FindElement(By.XPath($"//a[contains(@href, '/{nombreDeUsuario}/following/')]"));
        }

        public static int TextoDelSpanQueTieneElLink(IWebElement linkSeguidores)
        {
            return Convert.ToInt32(linkSeguidores.FindElement(By.CssSelector("span")).Text.Replace(",", ""));
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
