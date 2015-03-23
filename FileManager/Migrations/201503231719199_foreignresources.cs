namespace FileManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class foreignresources : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForeignSharedResource",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        OwnerID = c.String(),
                        Name = c.String(),
                        IsDirectory = c.Boolean(nullable: false),
                        IsAccessible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ResourceRecipient",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        ResourceID = c.String(),
                        RecipientID = c.String(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.SharedResource",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        FullPath = c.String(),
                        IsDirectory = c.Boolean(nullable: false),
                        IsPublic = c.Boolean(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        IsWrite = c.Boolean(nullable: false),
                        IsAll = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SharedResource");
            DropTable("dbo.ResourceRecipient");
            DropTable("dbo.ForeignSharedResource");
        }
    }
}
