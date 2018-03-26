namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedisActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoresToRelays", "isActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoresToRelays", "isActive");
        }
    }
}
