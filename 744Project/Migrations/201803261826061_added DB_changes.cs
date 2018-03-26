namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedDB_changes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoresToRelays", "weight", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoresToRelays", "weight");
        }
    }
}
