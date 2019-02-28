using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Instagram_Automat.Models
{
	public class Usuario
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string NombreDeUsuario { get; set; }
		public string Contrasenia { get; set; }

		public int CantidadSeguidos { get; set; }
		public int CantidadSeguidores { get; set; }

		public virtual ICollection<Seguido> Seguidos { get; set; }
		public virtual ICollection<Seguidor> Seguidores { get; set; }
	}
}