namespace dimain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ver1_datacache : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataCache",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Data = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DataCache");
        }
    }
}
