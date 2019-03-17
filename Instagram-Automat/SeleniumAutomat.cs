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
        private List<string> _nicksUsuariosSeguidos;
        private readonly Usuario _usuario;
		private static readonly Random Random = new Random();
		private readonly DataAccesLayer _dal;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
			var linkSeguidores = MetodosGenerales.LinkSeguidos(_browser, _usuario.NombreDeUsuario);
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
			IrAlPerfilDelUsuarioLogueado();

			var linkSeguidores = MetodosGenerales.LinkSeguidores(_browser, _usuario.NombreDeUsuario);
            var cantidadSeguidores = MetodosGenerales.TextoDelSpanQueTieneElLink(linkSeguidores);			

			linkSeguidores.Click();

			try
			{
				log.Info("Obteniendo el nick de cada seguidor...");
				return NicksDeUsuariosRelacionados(cantidadSeguidores);
			}
			catch
			{
				return Seguidores();
			}
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

            var linkSeguidos = MetodosGenerales.LinkSeguidores(_browser, _usuario.NombreDeUsuario);
            var cantidadSeguidores = MetodosGenerales.TextoDelSpanQueTieneElLink(linkSeguidos);
            linkSeguidos.Click();

            _nicksUsuariosSeguidos = NicksDeUsuariosRelacionados(cantidadSeguidores);
        }

		private List<string> NicksDeUsuariosRelacionados(int cantidadQueSeDebeConseguir)
		{
			var relacionadosLinkElements = _browser.FindElements(By.CssSelector("ul>div>li>div>div>div>div>a"));

			var cantidadIteracionAnterior = 0;
			while (cantidadQueSeDebeConseguir > relacionadosLinkElements.Count)
			{
				Thread.Sleep(Random.Next(3000, 4000));
                _browser.Scroll(2000, 3000);
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

            new ExecuterBuilder(botonIngresar.Click)
                .WaitTimeIfException(1, 5)
                .IfException(RechazarOfrecimientos)
                .WaitTimeAfterExecution(1, 2)
                .Execute();

            while (!MetodosGenerales.PantallaActivaEsPerfilDelUsuarioLogueado(_browser, _usuario))
			{
				MetodosGenerales.RechazarOfrecimientos(_browser);
				IrAlPerfilDelUsuarioLogueado();
			}
			EsperarEntre(2000, 3000);
			IrAlPerfilDelUsuarioLogueado();
		}

        public void RechazarOfrecimientos()
        {
            MetodosGenerales.RechazarOfrecimientos(_browser);
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
			Thread.Sleep(Random.Next(3500, 5000));

			var botonUnfollowDelPopup = _browser.FindElement(By.XPath("//button[contains(text(), 'Unfollow')]"), 10);
			botonUnfollowDelPopup.Click();
			Thread.Sleep(Random.Next(3500, 5000));

			if (CantidadDeSeguidosQueFiguraEnElPerfil() == _usuario.CantidadSeguidos())
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
