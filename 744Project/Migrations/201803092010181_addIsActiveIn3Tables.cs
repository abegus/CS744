namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIsActiveIn3Tables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RelayToProcessCenterConnections", "isActive", c => c.Boolean(nullable: true));
            AddColumn("dbo.Relays", "isActive", c => c.Boolean(nullable: true));
            AddColumn("dbo.RelayToRelayConnections", "isActive", c => c.Boolean(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RelayToRelayConnections", "isActive");
            DropColumn("dbo.Relays", "isActive");
            DropColumn("dbo.RelayToProcessCenterConnections", "isActive");
        }
    }
}
