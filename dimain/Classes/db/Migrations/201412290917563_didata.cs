namespace dimain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class didata : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataIslandData",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PublicKey = c.String(),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DiUserDataIslandId",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        DatIslandId = c.String(),
                        LastUpdate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DiUserDataIslandId");
            DropTable("dbo.DataIslandData");
        }
    }
}
