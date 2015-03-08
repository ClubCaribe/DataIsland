namespace dataislandcommon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usercontacts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContactCategory",
                c => new
                    {
                        ContactCategoryId = c.String(nullable: false, maxLength: 128),
                        CategoryName = c.String(),
                        ParentCategoryId = c.String(),
                    })
                .PrimaryKey(t => t.ContactCategoryId);
            
            CreateTable(
                "dbo.UserContactCategory",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ContactCategoryId = c.String(),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserContact",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Username = c.String(),
                        Name = c.String(),
                        InitialMessage = c.String(),
                        Accepted = c.Boolean(nullable: false),
                        DataIslandId = c.String(),
                        RequestToAccept = c.Boolean(nullable: false),
                        IsFavourite = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserContact");
            DropTable("dbo.UserContactCategory");
            DropTable("dbo.ContactCategory");
        }
    }
}
