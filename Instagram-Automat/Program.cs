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
						ActualizarCantidadDeSeguidos();
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
                                .AttemptsNumberBeforeCancel(10)
                                .WaitTimeBetweenAttempts(100, 150)
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
			Console.WriteLine("Operación realizada con éxito");
			Console.ReadLine();
		}

		private static void ActualizarCantidadDeSeguidos()
		{
			var seguidos = _seleniumAutomat.Seguidos();

			Console.WriteLine($"Guardando nick de cada seguido en la base...");

			Dal.EliminarTodosLosSeguidos(_usuario);

			Dal.CargarSeguidos(_usuario, seguidos);
		}

		private static void MostrarChequeoDeSeguidoresYSeguidosCensados()
		{
			Console.WriteLine(_usuario.CantidadSeguidos() == _seleniumAutomat.CantidadDeSeguidosQueFiguraEnElPerfil()
				? "\nNo es necesario actualizar la cantidad de seguidos."
				: "\nSe recomienda actualizar la cantidad de seguidos.");

			Console.WriteLine(_usuario.CantidadSeguidores() == _seleniumAutomat.CantidadDeSeguidoresQueFiguraEnElPerfil()
				? "No es necesario actualizar la cantidad de seguidores.\n"
				: "Se recomienda actualizar la cantidad de seguidores.\n");
		}

		private static void ActualizarCantidadDeSeguidores()
		{
			var seguidores = _seleniumAutomat.Seguidores();			

			Console.WriteLine($"Guardando nick de cada seguidor en la base...");

			Dal.EliminarTodosLosSeguidores(_usuario);

			Dal.CargarSeguidores(_usuario, seguidores);
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
			Console.WriteLine($"Usuario logueado: {_usuario.NombreDeUsuario}.");
			Console.WriteLine($"Seguidores último censo: {_usuario.CantidadSeguidores()}.");
			Console.WriteLine($"Seguidos último censo: {_usuario.CantidadSeguidos()}.\n");			
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
