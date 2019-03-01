using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Instagram_Automat.ExtensionMethods;
using Instagram_Automat.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Instagram_Automat
{
	public class SeleniumAutomat
	{
		private ChromeDriver _browser;
		private readonly Usuario _usuario;
		private static readonly Random Random = new Random();
		private readonly DataAccesLayer _dal;
		private bool _yaEspereUnaVez = false;

		public SeleniumAutomat(Usuario usuario, DataAccesLayer dal)
		{
			_usuario = usuario;
			_browser = MetodosGenerales.Browser();
			_dal = dal;

			IniciarSesion();
		}

		public int CantidadRealDeSeguidores()
		{
			IrAlPerfilDelUsuarioLogueado();

			var linkSeguidores = LinkSeguidores();
			return TextoDelSpanQueTieneElLink(linkSeguidores);
		}


		public int CantidadRealDeSeguidos()
		{
			IrAlPerfilDelUsuarioLogueado();

			var linkSeguidos = LinkSeguidos();
			return TextoDelSpanQueTieneElLink(linkSeguidos);
		}

		public IList<string> Seguidores()
		{
			IrAlPerfilDelUsuarioLogueado();

			var linkSeguidores = LinkSeguidores();
			var cantidadSeguidores = TextoDelSpanQueTieneElLink(linkSeguidores);			

			linkSeguidores.Click();

			try
			{
				Console.WriteLine("Obteniendo el nick de cada seguidor...");
				return NicksDeUsuariosRelacionados(cantidadSeguidores);
			}
			catch
			{
				return Seguidores();
			}
		}

		public IList<string> Seguidos()
		{
			IrAlPerfilDelUsuarioLogueado();

			var linkSeguidores = LinkSeguidos();
			var cantidadSeguidores = TextoDelSpanQueTieneElLink(linkSeguidores);

			linkSeguidores.Click();

			try
			{
				Console.WriteLine($"Obteniendo el nick de cada seguido...");
				return NicksDeUsuariosRelacionados(cantidadSeguidores);
			}
			catch
			{
				return Seguidos();
			}
		}

		private static int TextoDelSpanQueTieneElLink(IWebElement linkSeguidores)
		{
			return Convert.ToInt32(linkSeguidores.FindElement(By.CssSelector("span")).Text.Replace(",", ""));
		}

		private IWebElement LinkSeguidores()
		{
			return _browser.FindElement(By.XPath($"//a[contains(@href, '/{_usuario.NombreDeUsuario}/followers/')]"));
		}

		private IWebElement LinkSeguidos()
		{
			return _browser.FindElement(By.XPath($"//a[contains(@href, '/{_usuario.NombreDeUsuario}/following/')]"));
		}

		private List<string> NicksDeUsuariosRelacionados(int cantidadQueSeDebeConseguir)
		{
			var relacionadosLinkElements = _browser.FindElements(By.CssSelector("ul>div>li>div>div>div>div>a"));

			var cantidadIteracionAnterior = 0;
			while (cantidadQueSeDebeConseguir > relacionadosLinkElements.Count)
			{
				Thread.Sleep(Random.Next(3000, 4000));
				_browser.ExecuteScript($"window.scrollBy(0,{Random.Next(2000, 3000)})");
				relacionadosLinkElements = _browser.FindElements(By.CssSelector("ul>div>li>div>div>div>div>a"));

				if (cantidadIteracionAnterior == relacionadosLinkElements.Count)
					throw new Exception("Se tildó.");

				cantidadIteracionAnterior = relacionadosLinkElements.Count;
			}

			return relacionadosLinkElements.Select(x => x.Text).ToList();
		}

		private void IniciarSesion()
		{
			_browser.Url = "https://www.instagram.com/accounts/login/";

			var userNameInput = _browser.FindElement(By.CssSelector("input[aria-label='Phone number, username, or email']"));
			userNameInput.SendKeys(_usuario.NombreDeUsuario);

			var passwordInput = _browser.FindElement(By.CssSelector("input[aria-label=\"Password\"]"));
			passwordInput.SendKeys(_usuario.Contrasenia);

			var botonIngresar = _browser.FindElement(By.CssSelector("button[type=\"submit\"]"));
			botonIngresar.Click();

			EsperarEntre(1000, 2000);

			while (!MetodosGenerales.PantallaActivaEsPerfilDelUsuarioLogueado(_browser))
			{
				MetodosGenerales.RechazarOfrecimientos(_browser);
				IrAlPerfilDelUsuarioLogueado();
			}
			EsperarEntre(2000, 3000);
			IrAlPerfilDelUsuarioLogueado();
		}

		public void DejarDeSeguirUsuarios(IList<string> nicksUsuariosADejarDeSeguir)
		{
			foreach (var nick in nicksUsuariosADejarDeSeguir)
				DejarDeSeguirUsuario(nick);
		}

		private void IrAlPerfilDelUsuarioLogueado()
		{
			if (!MetodosGenerales.PantallaActivaEsPerfilDelUsuarioLogueado(_browser))
				_browser.Url = $"http://www.instagram.com/{_usuario.NombreDeUsuario}";
		}

		private void DejarDeSeguirUsuario(string nick)
		{
			MetodosGenerales.IrAlPerfilDelUsuario(_browser, nick);

			var botonFollowing = _browser.FindElement(By.XPath("//button[contains(text(), 'Following')]"), 10);
			botonFollowing.Click();
			Thread.Sleep(Random.Next(3500, 5000));

			var botonUnfollowDelPopup = _browser.FindElement(By.XPath("//button[contains(text(), 'Unfollow')]"), 10);
			botonUnfollowDelPopup.Click();
			Thread.Sleep(Random.Next(3500, 5000));

			if (CantidadRealDeSeguidos() == _usuario.CantidadSeguidos())
			{
				throw new Exception("Se colgó");				
			}				
			
			_dal.EliminarSeguido(_usuario, nick);
		}

		private static void EsperarEntre(int inicio, int fin)
		{
			Thread.Sleep(Random.Next(inicio, fin));
		}

		public void ReiniciarBrowserEIiniciarSesion()
		{
			_browser.Dispose();
			_browser = MetodosGenerales.Browser();
			IniciarSesion();
		}
	}
}
