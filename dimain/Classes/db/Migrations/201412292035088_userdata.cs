namespace dimain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userdata : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DiUserDataIslandId", newName: "DiUserData");
            AddColumn("dbo.DiUserData", "PublicKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DiUserData", "PublicKey");
            RenameTable(name: "dbo.DiUserData", newName: "DiUserDataIslandId");
        }
    }
}
