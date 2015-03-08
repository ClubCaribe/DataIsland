namespace dimain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userpassport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DiUserPassport",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PassportData = c.String(),
                        ExpireDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DiUserPassport");
        }
    }
}
