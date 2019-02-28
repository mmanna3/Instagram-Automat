using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instagram_Automat.Models
{
	public class Seguidor
	{
		[Key, Column(Order = 1)]
		public int UsuarioSeguidoId { get; set; }
		public virtual Usuario UsuarioSeguido { get; set; }

		[Key, Column(Order = 0)]
		public string Nick { get; set; }
	}
}