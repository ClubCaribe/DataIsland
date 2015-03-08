namespace dataislandcommon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ver1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserExternalLogin",
                c => new
                    {
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(),
                    })
                .PrimaryKey(t => t.ProviderKey);
            
            CreateTable(
                "dbo.UserAccount",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        RegisterDate = c.DateTime(nullable: false),
                        Roles = c.String(),
                        Password = c.String(),
                        ProAccountExpirationTime = c.DateTime(nullable: false),
                        Name = c.String(),
                        PublicKey = c.String(),
                        PrivateKey = c.String(),
                        LastLoginTime = c.DateTime(nullable: false),
                        UiLanguage = c.String(),
                        LanguageDirection = c.String(),
                        Email = c.String(),
                        CountryCode = c.String(),
                        SecurityStamp = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserClaim",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Type = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserSetting",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                        Visible = c.Boolean(nullable: false),
                        Category = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserSetting");
            DropTable("dbo.UserClaim");
            DropTable("dbo.UserAccount");
            DropTable("dbo.UserExternalLogin");
        }
    }
}
