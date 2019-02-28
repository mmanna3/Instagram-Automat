using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instagram_Automat.Models
{
	public class MyDbContext : DbContext
	{
		public DbSet<Usuario> Usuarios { get; set; }
		public DbSet<Seguido> UsuariosSecundarios { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

			base.OnModelCreating(modelBuilder);
		}
	}
}
