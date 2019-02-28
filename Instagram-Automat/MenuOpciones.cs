using System.ComponentModel.DataAnnotations;

namespace Instagram_Automat
{
	public enum MenuOpciones
	{
		[Display(Name="Actualizar cantidad de seguidores")]
		OpcionActualizarCantidadDeSeguidores = 1,

		[Display(Name = "Actualizar cantidad de seguidos")]
		OpcionActualizarCantidadDeSeguidos = 2,

		[Display(Name = "Chequear si los últimos datos censados están actualizados")]
		OpcionChequearSiDiferenciaEntreBaseYRealidad = 3,

		[Display(Name = "Dejar de seguir usuarios que no me siguen")]
		DejarDeSeguirUsuariosQueNoMeSiguen = 4
	}
}