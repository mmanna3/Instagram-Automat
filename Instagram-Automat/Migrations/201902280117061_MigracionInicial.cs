namespace Instagram_Automat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigracionInicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreDeUsuario = c.String(),
                        Contrasenia = c.String(),
                        CantidadSeguidos = c.Int(nullable: false),
                        CantidadSeguidores = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Seguidor",
                c => new
                    {
                        Nick = c.String(nullable: false, maxLength: 128),
                        UsuarioSeguidoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Nick, t.UsuarioSeguidoId })
                .ForeignKey("dbo.Usuario", t => t.UsuarioSeguidoId)
                .Index(t => t.UsuarioSeguidoId);
            
            CreateTable(
                "dbo.Seguido",
                c => new
                    {
                        Nick = c.String(nullable: false, maxLength: 128),
                        UsuarioSeguidorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Nick, t.UsuarioSeguidorId })
                .ForeignKey("dbo.Usuario", t => t.UsuarioSeguidorId)
                .Index(t => t.UsuarioSeguidorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Seguido", "UsuarioSeguidorId", "dbo.Usuario");
            DropForeignKey("dbo.Seguidor", "UsuarioSeguidoId", "dbo.Usuario");
            DropIndex("dbo.Seguido", new[] { "UsuarioSeguidorId" });
            DropIndex("dbo.Seguidor", new[] { "UsuarioSeguidoId" });
            DropTable("dbo.Seguido");
            DropTable("dbo.Seguidor");
            DropTable("dbo.Usuario");
        }
    }
}
