using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Instagram_Automat
{
	internal class Program
	{
		//todo: EN VEZ DE INICIAR SESIÓN, HACELO CON LOS TOKEN
		private static string _userName = "amanteaoficial";
		private static string _password = "gordomotoneta1";
		private static Random _random;
		private static XMLHelper _xmlHelper;
		private static int _cantidadSeguidosAntesDeIntentarDejarDeSeguirAlUltimo;
		private static bool _hayUsuariosALosQueDeboDejarDeSeguir = true;
		private static bool _seTildo = false;

		private static void Main(string[] args)
		{																
			var chromeOptions = new ChromeOptions();
			chromeOptions.EnableMobileEmulation("iPhone 4");
			//chromeOptions.AddArguments("headless");
			//chromeOptions.AddArgument("user-data-dir=C:\\Users\\matia\\AppData\\Local\\Google\\Chrome\\User Data"); 
			_random = new Random();
			_xmlHelper = new XMLHelper(_userName);			

			while (_hayUsuariosALosQueDeboDejarDeSeguir)
			{
				_seTildo =  false;
				using (var browser = new ChromeDriver(chromeOptions))
				{
					IniciarSesion(browser);

					RechazarLaPrimeraPantalla(browser);

					var usuariosSeguidos = ListaSeguidos(browser);
					if (_seTildo)
						break;

					var seguidores = ListaSeguidores(browser);
					if (_seTildo)
						break;

					var usuarioQueYoSigoPeroQueNoMeSiguen = UsuarioQueYoSigoPeroQueNoMeSiguen(usuariosSeguidos, seguidores);

					foreach (var usuario in usuarioQueYoSigoPeroQueNoMeSiguen)
					{
						var cantidadSeguidos = DejarDeSeguir(browser, usuario);
						if (_cantidadSeguidosAntesDeIntentarDejarDeSeguirAlUltimo != cantidadSeguidos)
							_cantidadSeguidosAntesDeIntentarDejarDeSeguirAlUltimo = cantidadSeguidos;
						else
							break;
					}

					if (!usuarioQueYoSigoPeroQueNoMeSiguen.Any())
						_hayUsuariosALosQueDeboDejarDeSeguir = false;
				}				
			}
			Console.ReadLine();
		}

		private static int DejarDeSeguir(IWebDriver browser, string usuario)
		{
			Thread.Sleep(_random.Next(2000, 2500));
			IrAlPerfilDelUsuario(browser, usuario);

			var botonFollowing = browser.FindElement(By.XPath("//button[contains(text(), 'Following')]"), 10);
			botonFollowing.Click();
			Thread.Sleep(_random.Next(2000, 2500));

			var botonUnfollow = browser.FindElement(By.XPath("//button[contains(text(), 'Unfollow')]"), 10);
			botonUnfollow.Click();
			Thread.Sleep(_random.Next(2000, 2500));

			IrAlPerfilDelUsuarioLogueado(browser);
			Thread.Sleep(_random.Next(2000, 2500));

			var linkSeguidos = browser.FindElement(By.XPath($"//a[contains(@href, '/{_userName}/following/')]"));
			var cantidadSeguidos = Convert.ToInt32(linkSeguidos.FindElement(By.CssSelector("span")).Text.Replace(",", ""));				

			Console.WriteLine($"Seguidos: {cantidadSeguidos}");

			return cantidadSeguidos;
		}

		private static IList<string> UsuarioQueYoSigoPeroQueNoMeSiguen(IList<string> usuariosSeguidos, IList<string> seguidores)
		{
			var result = new List<string>();			

			foreach (var usuarioSeguido in usuariosSeguidos)
				if (!seguidores.Contains(usuarioSeguido))
					result.Add(usuarioSeguido);

			Console.WriteLine($"Usuarios que yo sigo pero que no me siguen: {result.Count}");
			return result;
		}

		private static IList<string> ListaSeguidores(RemoteWebDriver browser)
		{
			IrAlPerfilDelUsuarioLogueado(browser);

			var linkSeguidores = browser.FindElement(By.XPath($"//a[contains(@href, '/{_userName}/followers/')]"));
			var cantidadSeguidores = Convert.ToInt32(linkSeguidores.FindElement(By.CssSelector("span")).Text.Replace(",", ""));

			Console.WriteLine($"Seguidores: {cantidadSeguidores}");

			linkSeguidores.Click();

			var seguidoresAnchorElements = browser.FindElements(By.CssSelector("ul>div>li>div>div>div>div>a"));

			var cantidadDeSeguidosElementsIteracionAnterior = cantidadSeguidores;

			while (cantidadSeguidores > seguidoresAnchorElements.Count && !_seTildo)
			{
				Thread.Sleep(_random.Next(1000, 2000));
				browser.ExecuteScript($"window.scrollBy(0,{_random.Next(2000, 3000)})");
				seguidoresAnchorElements = browser.FindElements(By.CssSelector("ul>div>li>div>div>div>div>a"));

				if (cantidadDeSeguidosElementsIteracionAnterior == seguidoresAnchorElements.Count)
					_seTildo = true;
			}

			return seguidoresAnchorElements.Select(x => x.Text).ToList();
		}

		private static IList<string> ListaSeguidos(RemoteWebDriver browser)
		{
			IrAlPerfilDelUsuarioLogueado(browser);

			var linkSeguidos = browser.FindElement(By.XPath($"//a[contains(@href, '/{_userName}/following/')]"));
			var cantidadSeguidos = Convert.ToInt32(linkSeguidos.FindElement(By.CssSelector("span")).Text.Replace(",", ""));

			Console.WriteLine($"Seguidos: {cantidadSeguidos}");

			linkSeguidos.Click();

			var seguidosAnchorElement = browser.FindElements(By.CssSelector("ul>div>li>div>div>div>div>a"));
			var cantidadDeSeguidosElementsIteracionAnterior = cantidadSeguidos;

			while (cantidadSeguidos > seguidosAnchorElement.Count && !_seTildo)
			{
				Thread.Sleep(_random.Next(1000, 2000));
				browser.ExecuteScript($"window.scrollBy(0,{_random.Next(2000, 3000)})");
				seguidosAnchorElement = browser.FindElements(By.CssSelector("ul>div>li>div>div>div>div>a"));

				if (cantidadDeSeguidosElementsIteracionAnterior == seguidosAnchorElement.Count)
					_seTildo = true;
			}

			return seguidosAnchorElement.Select(x => x.Text).ToList();
		}

		private static void IrAlPerfilDelUsuarioLogueado(IWebDriver browser)
		{
			browser.Url = $"http://www.instagram.com/{_userName}";
		}

		private static void IrAlPerfilDelUsuario(IWebDriver browser, string userName)
		{
			browser.Url = $"http://www.instagram.com/{userName}";
		}

		private static void RechazarLaPrimeraPantalla(ChromeDriver browser)
		{
			try
			{
				var botonRechazarNotificaciones = browser.FindElement(By.XPath("//button[contains(text(), 'Not Now')]"), 10);
				botonRechazarNotificaciones.Click();
			}
			catch (Exception e)
			{
				Console.Write(e.Message);
			}
		}

		private static void IniciarSesion(IWebDriver browser)
		{
			browser.Url = "https://www.instagram.com/accounts/login/";
			
			var userNameInput = browser.FindElement(By.CssSelector("input[aria-label='Phone number, username, or email']"));
			userNameInput.SendKeys(_userName);

			var passwordInput = browser.FindElement(By.CssSelector("input[aria-label=\"Password\"]"));
			passwordInput.SendKeys(_password);

			var botonIngresar = browser.FindElement(By.CssSelector("button[type=\"submit\"]"));
			botonIngresar.Click();
		}
	}

	public static class WebDriverExtensions
	{
		public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
		{
			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
			return wait.Until(ExpectedConditions.ElementExists(by));		
		}
	}

	//var builder = new Actions(browser);
	//builder.KeyDown(Keys.PageDown).Perform();
}
