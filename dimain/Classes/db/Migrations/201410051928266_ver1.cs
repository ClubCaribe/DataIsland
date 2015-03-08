namespace dimain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ver1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DiUser",
                c => new
                    {
                        Username = c.String(nullable: false, maxLength: 128),
                        Id = c.String(),
                        Name = c.String(),
                        ProAccountExpirationDate = c.DateTime(nullable: false),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Username);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Username = c.String(maxLength: 128),
                        Role = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DiUser", t => t.Username)
                .Index(t => t.Username);
            
            CreateTable(
                "dbo.DiUserExternalLogin",
                c => new
                    {
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.ProviderKey);
            
            CreateTable(
                "dbo.MainDiSetting",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.OAuthClient",
                c => new
                    {
                        ClientId = c.Int(nullable: false, identity: true),
                        ClientIdentifier = c.String(),
                        ClientSecret = c.String(),
                        Callback = c.String(),
                        Name = c.String(),
                        ClientType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClientId);
            
            CreateTable(
                "dbo.OAuthClientAuthorization",
                c => new
                    {
                        AuthorizationId = c.Int(nullable: false, identity: true),
                        CreatedOnUtc = c.DateTime(nullable: false),
                        ClientId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Scope = c.String(),
                        ExpirationDateUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AuthorizationId);
            
            CreateTable(
                "dbo.OAuthNonce",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Context = c.String(),
                        Code = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.OAuthSymmetricCryptoKey",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Bucket = c.String(),
                        Handle = c.String(),
                        ExpiresUtc = c.DateTime(nullable: false),
                        SecretBase64 = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRole", "Username", "dbo.DiUser");
            DropIndex("dbo.UserRole", new[] { "Username" });
            DropTable("dbo.OAuthSymmetricCryptoKey");
            DropTable("dbo.OAuthNonce");
            DropTable("dbo.OAuthClientAuthorization");
            DropTable("dbo.OAuthClient");
            DropTable("dbo.MainDiSetting");
            DropTable("dbo.DiUserExternalLogin");
            DropTable("dbo.UserRole");
            DropTable("dbo.DiUser");
        }
    }
}
