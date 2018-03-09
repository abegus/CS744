namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedQueueInRelays : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Relays", "relayQueue", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Relays", "relayQueue");
        }
    }
}
