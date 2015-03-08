namespace dimain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usercachesettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DiUser", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DiUser", "UserId");
        }
    }
}
