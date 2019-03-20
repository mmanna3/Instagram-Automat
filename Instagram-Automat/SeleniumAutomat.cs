using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Instagram_Automat.ExtensionMethods;
using Instagram_Automat.Models;
using Instagram_Automat.Utils;
using OpenQA.Selenium;

namespace Instagram_Automat
{
    public class SeleniumAutomat
	{
		private YKNChromeDriver _browser;        
        private readonly Usuario _usuario;
		private static readonly Random Random = new Random();
		private readonly DataAccesLayer _dal;
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private IList<string> _nicksUsuariosSeguidores;
		private IList<string> _nicksUsuariosSeguidos;

		public SeleniumAutomat(Usuario usuario, DataAccesLayer dal)
		{
			_usuario = usuario;
			_browser = MetodosGenerales.Browser();
			_dal = dal;

			IniciarSesion();
		}

		public int CantidadDeSeguidoresQueFiguraEnElPerfil()
		{
			IrAlPerfilDelUsuarioLogueado();
			var linkSeguidores = MetodosGenerales.LinkSeguidores(_browser, _usuario.NombreDeUsuario);
            return MetodosGenerales.TextoDelSpanQueTieneElLink(linkSeguidores);
		}


		public int CantidadDeSeguidosQueFiguraEnElPerfil()
		{
			IrAlPerfilDelUsuarioLogueado();
			var linkSeguidos = MetodosGenerales.LinkSeguidos(_browser, _usuario.NombreDeUsuario);
            return MetodosGenerales.TextoDelSpanQueTieneElLink(linkSeguidos);
		}

		public IList<string> Seguidores()
		{
			new ExecuterBuilder(ObtenerNicksDeSeguidores)
				.AttemptsNumberBeforeCancel(5)
				.Execute();

			return _nicksUsuariosSeguidores;
		}

		private void ObtenerNicksDeSeguidores()
		{
			IrAlPerfilDelUsuarioLogueado();

			var linkSeguidores = MetodosGenerales.LinkSeguidores(_browser, _usuario.NombreDeUsuario);
			var cantidadSeguidores = MetodosGenerales.TextoDelSpanQueTieneElLink(linkSeguidores);
			linkSeguidores.Click();

			_nicksUsuariosSeguidores = NicksDeUsuariosRelacionados(cantidadSeguidores);
		}

		public IList<string> Seguidos()
		{
            new ExecuterBuilder(ObtenerNicksDeSeguidos)
                .AttemptsNumberBeforeCancel(5)
                .Execute();

            return _nicksUsuariosSeguidos;
		}

        private void ObtenerNicksDeSeguidos()
        {
            IrAlPerfilDelUsuarioLogueado();

            var linkSeguidos = MetodosGenerales.LinkSeguidos(_browser, _usuario.NombreDeUsuario);
            var cantidadSeguidores = MetodosGenerales.TextoDelSpanQueTieneElLink(linkSeguidos);
            linkSeguidos.Click();

            _nicksUsuariosSeguidos = NicksDeUsuariosRelacionados(cantidadSeguidores);
        }

		private List<string> NicksDeUsuariosRelacionados(int cantidadQueSeDebeConseguir)
		{
			var relacionadosLinkElements = _browser.FindElements(By.CssSelector("ul>div>li>div>div>div>div>a"));

			var cantidadIteracionAnterior = 0;
			while (cantidadQueSeDebeConseguir > relacionadosLinkElements.Count + 2) //Siempre hay uno o dos que no los consigue
			{
				EsperarEntre(2000, 3000);
                _browser.Scroll(5000, 10000);

				relacionadosLinkElements = _browser.FindElements(By.CssSelector("ul>div>li>div>div>div>div>a"));

				if (cantidadIteracionAnterior == relacionadosLinkElements.Count)
					throw new Exception($"Debían conseguirse {cantidadQueSeDebeConseguir} y se consiguieron {cantidadIteracionAnterior}.");

				cantidadIteracionAnterior = relacionadosLinkElements.Count;
			}

			Log.Info($"Debían conseguirse {cantidadQueSeDebeConseguir} y se consiguieron {cantidadIteracionAnterior}.");
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

            new ExecuterBuilder(botonIngresar.Click)
                .WaitTimeIfException(1, 5)
                .IfException(RechazarOfrecimientos)
                .WaitTimeAfterExecution(1, 2)
                .Execute();

			if (!MetodosGenerales.PantallaActivaEsPerfilDelUsuarioLogueado(_browser, _usuario))
				new ExecuterBuilder(RechazarOfrecimientos)
					.WaitTimeAfterExecution(2, 3)
					.Execute();

			IrAlPerfilDelUsuarioLogueado();
		}

        private void RechazarOfrecimientos()
        {
            MetodosGenerales.RechazarOfrecimientos(_browser);
	        _browser.Scroll(5000, 10000);
		}

		public void DejarDeSeguirUsuarios(IList<string> nicksUsuariosADejarDeSeguir)
		{
			foreach (var nick in nicksUsuariosADejarDeSeguir)
				DejarDeSeguirUsuario(nick);
		}

		private void IrAlPerfilDelUsuarioLogueado()
		{
			if (!MetodosGenerales.PantallaActivaEsPerfilDelUsuarioLogueado(_browser, _usuario))
				_browser.Url = $"http://www.instagram.com/{_usuario.NombreDeUsuario}";
		}

		private void DejarDeSeguirUsuario(string nick)
		{
			MetodosGenerales.IrAlPerfilDelUsuario(_browser, nick);

			var botonFollowing = _browser.FindElement(By.XPath("//button[contains(text(), 'Following')]"), 10);
			botonFollowing.Click();
			EsperarEntre(1000, 2000);

			var botonUnfollowDelPopup = _browser.FindElement(By.XPath("//button[contains(text(), 'Unfollow')]"), 10);
			botonUnfollowDelPopup.Click();
			EsperarEntre(1000, 2000);

			var cantidadDeSeguidosPerfil = CantidadDeSeguidosQueFiguraEnElPerfil();
			if (cantidadDeSeguidosPerfil == _usuario.CantidadSeguidos())
				throw new Exception($"No se pudo dejar de seguir al usuario '{nick}'.");

			_dal.EliminarSeguido(_usuario, nick);
			Log.Info($"Cantidad de seguidos: {cantidadDeSeguidosPerfil}. Se dejó de seguir al usuario '{nick}'.");
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
