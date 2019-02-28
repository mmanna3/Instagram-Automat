using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Instagram_Automat.Models
{
	public class Seguido
	{
		[Key, Column(Order = 1)]
		public int UsuarioSeguidorId { get; set; }
		public virtual Usuario UsuarioSeguidor { get; set; }

		[Key, Column(Order = 0)]
		public string Nick { get; set; }
	}
}