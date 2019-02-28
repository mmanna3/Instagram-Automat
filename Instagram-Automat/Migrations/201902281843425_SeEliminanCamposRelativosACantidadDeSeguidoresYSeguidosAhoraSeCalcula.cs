namespace Instagram_Automat.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeEliminanCamposRelativosACantidadDeSeguidoresYSeguidosAhoraSeCalcula : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Usuario", "CantidadSeguidos");
            DropColumn("dbo.Usuario", "CantidadSeguidores");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Usuario", "CantidadSeguidores", c => c.Int(nullable: false));
            AddColumn("dbo.Usuario", "CantidadSeguidos", c => c.Int(nullable: false));
        }
    }
}
