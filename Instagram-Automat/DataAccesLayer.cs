using System.Collections.Generic;
using System.Linq;
using Instagram_Automat.Models;

namespace Instagram_Automat
{
	public class DataAccesLayer
	{
		private static MyDbContext _context;

		public DataAccesLayer()
		{
			_context = new MyDbContext();
		}

		public void EliminarTodosLosSeguidores(Usuario usuario)
		{
			var seguidores = _context.Seguidores.Where(x => x.UsuarioSeguidoId == usuario.Id);

			_context.Seguidores.RemoveRange(seguidores);

			_context.SaveChanges();
		}

		public void CargarSeguidores(Usuario usuario, IList<string> seguidores)
		{
			foreach (var seguidor in seguidores)
			{
				var a = new Seguidor { Nick = seguidor, UsuarioSeguidoId = usuario.Id };
				_context.Seguidores.Add(a);
			}

			_context.SaveChanges();
		}

		public void CargarSeguidos(Usuario usuario, IList<string> seguidos)
		{
			foreach (var seguido in seguidos)
			{
				var a = new Seguido { Nick = seguido, UsuarioSeguidorId = usuario.Id };
				_context.Seguidos.Add(a);
			}

			_context.SaveChanges();
		}

		public Usuario UsuarioOrDefault(string nombreUsuario)
		{
			return _context.Usuarios.SingleOrDefault(x => x.NombreDeUsuario.Equals(nombreUsuario));
		}

		public Usuario CrearUsuario(string nombreUsuario, string contrasenia)
		{
			var usuario = new Usuario { NombreDeUsuario = nombreUsuario, Contrasenia = contrasenia };
			_context.Usuarios.Add(usuario);
			_context.SaveChanges();

			return usuario;
		}

		public void EliminarTodosLosSeguidos(Usuario usuario)
		{
			var seguidores = _context.Seguidos.Where(x => x.UsuarioSeguidorId == usuario.Id);

			_context.Seguidos.RemoveRange(seguidores);

			_context.SaveChanges();
		}

		public IList<string> UsuariosQueYoSigoPeroNoMeSiguen(Usuario usuario)
		{
			var result = new List<string>();
			var seguidos = _context.Seguidos.Select(x => x.Nick);
			var seguidores = _context.Seguidores.Select(x => x.Nick);

			foreach (var usuarioSeguido in seguidos)
				if (!seguidores.Contains(usuarioSeguido))
					result.Add(usuarioSeguido);
			
			return result;
		}

		public void EliminarSeguido(Usuario usuario, string nickUsuarioSeguido)
		{
			var seguidores = _context.Seguidos.Single(x => x.UsuarioSeguidorId == usuario.Id && x.Nick == nickUsuarioSeguido);

			_context.Seguidos.Remove(seguidores);

			_context.SaveChanges();
		}
	}
}
