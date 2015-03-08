namespace dimain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class didata_last_update_column : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DataIslandData", "LastUpdate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DataIslandData", "LastUpdate");
        }
    }
}
