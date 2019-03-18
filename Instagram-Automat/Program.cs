using System;
using System.Linq;
using System.Threading;
using Instagram_Automat.Models;
using Instagram_Automat.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Instagram_Automat
{
	internal class Program
	{
		//todo: EN VEZ DE INICIAR SESIÓN, HACELO CON LOS TOKEN
		private static Usuario _usuario;				
		private static SeleniumAutomat _seleniumAutomat;
		private static readonly DataAccesLayer Dal = new DataAccesLayer();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main(string[] args)
		{
            ObtenerUsuarioPrincipal();						

			var opcionElegida = MostrarMenu();

			while (opcionElegida != 0)
			{
				switch (opcionElegida)
				{
					case MenuOpciones.OpcionActualizarCantidadDeSeguidores:
					{
						ActualizarCantidadDeSeguidores();
						MostrarOperacionRealizadaConExito();
						opcionElegida = MostrarMenu();
						break;
					}
					case MenuOpciones.OpcionActualizarCantidadDeSeguidos:
					{
						ActualizarListaDeSeguidos();
						MostrarOperacionRealizadaConExito();
						opcionElegida = MostrarMenu();
						break;
					}
					case MenuOpciones.OpcionChequearSiDiferenciaEntreBaseYRealidad:
					{
						MostrarChequeoDeSeguidoresYSeguidosCensados();
						MostrarOperacionRealizadaConExito();
						opcionElegida = MostrarMenu();
						break;
					}
					case MenuOpciones.DejarDeSeguirUsuariosQueNoMeSiguen:
					{
                        new ExecuterBuilder(DejarDeSeguirUsuariosQueNoMeSiguen)
                                .AttemptsNumberBeforeCancel(20)
                                .WaitTimeBetweenAttempts(600, 900)
								.Execute();

                        MostrarOperacionRealizadaConExito();
						opcionElegida = MostrarMenu();
						break;
					}
				}
			}
		}

		private static void DejarDeSeguirUsuariosQueNoMeSiguen()
		{
			var usuariosQueYoSigoPeroNoMeSiguen = Dal.UsuariosQueYoSigoPeroNoMeSiguen(_usuario);
            _seleniumAutomat.DejarDeSeguirUsuarios(usuariosQueYoSigoPeroNoMeSiguen);            
		}

		private static void MostrarOperacionRealizadaConExito()
		{
			log.Info("Operación finalizada. Presione enter para continuar.");
			Console.ReadLine();
		}

		private static void ActualizarListaDeSeguidos()
		{
            log.Info("Obteniendo nicks de cada usuario seguido...");
            var seguidos = _seleniumAutomat.Seguidos();

			log.Info("Eliminando nicks de seguidos de la base...");
			Dal.EliminarTodosLosSeguidos(_usuario);

			log.Info("Guardando nicks actualizados de seguidos en la base...");
			Dal.GuardarSeguidos(_usuario, seguidos);
		}

		private static void MostrarChequeoDeSeguidoresYSeguidosCensados()
		{
			var seguidosDb = _usuario.CantidadSeguidos();
			var seguidosPerfil = _seleniumAutomat.CantidadDeSeguidosQueFiguraEnElPerfil();

			var seguidoresDb = _usuario.CantidadSeguidores();
			var seguidoresPerfil = _seleniumAutomat.CantidadDeSeguidoresQueFiguraEnElPerfil();

			log.Info($"Seguidos en la base: {seguidosDb}. Seguidos reales: {seguidosPerfil}");
			log.Info($"Seguidores en la base: {seguidoresDb}. Seguidores reales: {seguidoresPerfil}");

			log.Info(seguidosDb == seguidosPerfil
				? "\nNo es necesario actualizar la cantidad de seguidos."
				: "\nSe recomienda actualizar la cantidad de seguidos.");

			log.Info(seguidoresDb == seguidoresPerfil
				? "No es necesario actualizar la cantidad de seguidores.\n"
				: "Se recomienda actualizar la cantidad de seguidores.\n");
		}

		private static void ActualizarCantidadDeSeguidores()
		{
			log.Info("Obteniendo nicks de cada usuario seguidor...");
			var seguidores = _seleniumAutomat.Seguidores();

			log.Info("Eliminando nicks de seguidos de la base...");
			Dal.EliminarTodosLosSeguidores(_usuario);

			log.Info("Guardando nicks actualizados de seguidos en la base...");
			Dal.GuardarSeguidores(_usuario, seguidores);
		}

		private static MenuOpciones MostrarMenu()
		{
			Console.Clear();
			MostrarDatosDelUsuarioPrincipal();

			foreach (var opcion in Enum.GetValues(typeof(MenuOpciones)).Cast<MenuOpciones>())
                Console.WriteLine($@"{(int) opcion} - {opcion.Descripcion()}");

			int opcionElegida;
			do
			{
				Console.WriteLine("\nSeleccionar una opción:");
				opcionElegida = Convert.ToInt32(Console.ReadLine());
			}
			while (!Enum.IsDefined(typeof(MenuOpciones), opcionElegida));

			return (MenuOpciones) Enum.Parse(typeof(MenuOpciones), opcionElegida.ToString());
		}


		private static void MostrarDatosDelUsuarioPrincipal()
		{
			log.Info($"Usuario logueado: {_usuario.NombreDeUsuario}.");
			log.Info($"Seguidores último censo: {_usuario.CantidadSeguidores()}.");
			log.Info($"Seguidos último censo: {_usuario.CantidadSeguidos()}.\n");			
		}

		private static void ObtenerUsuarioPrincipal()
		{
            Console.WriteLine(@"Ingresar nombre de usuario:");
			var nombreUsuario = Console.ReadLine();

			var usuario = Dal.UsuarioOrDefault(nombreUsuario);

			if (usuario == null)
			{
                Console.WriteLine(@"Ingresar contraseña:");
				var contrasenia = Console.ReadLine();

				usuario = Dal.CrearUsuario(nombreUsuario, contrasenia);
			}

			_usuario = usuario;
			_seleniumAutomat = new SeleniumAutomat(_usuario, Dal);

			Console.Clear();
		}
	}
}
