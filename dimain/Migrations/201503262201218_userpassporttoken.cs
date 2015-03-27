namespace dimain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userpassporttoken : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DiUserPassportToken",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        UserID = c.String(),
                        ExpirationTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DiUserPassportToken");
        }
    }
}
