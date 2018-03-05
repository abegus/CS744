namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relaytorealy : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RelayToRelayConnections", "relayID", "dbo.Relays");
            DropIndex("dbo.RelayToRelayConnections", new[] { "relayID" });
            AddColumn("dbo.RelayToRelayConnections", "relayID2", c => c.String(maxLength: 50));
            AddColumn("dbo.RelayToRelayConnections", "Relay_relayID", c => c.String(maxLength: 50));
            AddColumn("dbo.RelayToRelayConnections", "Relay2_relayID", c => c.String(maxLength: 50));
            AddColumn("dbo.RelayToRelayConnections", "Relay_relayID1", c => c.String(maxLength: 50));
            CreateIndex("dbo.RelayToRelayConnections", "Relay_relayID");
            CreateIndex("dbo.RelayToRelayConnections", "Relay2_relayID");
            CreateIndex("dbo.RelayToRelayConnections", "Relay_relayID1");
            AddForeignKey("dbo.RelayToRelayConnections", "Relay2_relayID", "dbo.Relays", "relayID");
            AddForeignKey("dbo.RelayToRelayConnections", "Relay_relayID1", "dbo.Relays", "relayID");
            AddForeignKey("dbo.RelayToRelayConnections", "Relay_relayID", "dbo.Relays", "relayID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RelayToRelayConnections", "Relay_relayID", "dbo.Relays");
            DropForeignKey("dbo.RelayToRelayConnections", "Relay_relayID1", "dbo.Relays");
            DropForeignKey("dbo.RelayToRelayConnections", "Relay2_relayID", "dbo.Relays");
            DropIndex("dbo.RelayToRelayConnections", new[] { "Relay_relayID1" });
            DropIndex("dbo.RelayToRelayConnections", new[] { "Relay2_relayID" });
            DropIndex("dbo.RelayToRelayConnections", new[] { "Relay_relayID" });
            DropColumn("dbo.RelayToRelayConnections", "Relay_relayID1");
            DropColumn("dbo.RelayToRelayConnections", "Relay2_relayID");
            DropColumn("dbo.RelayToRelayConnections", "Relay_relayID");
            DropColumn("dbo.RelayToRelayConnections", "relayID2");
            CreateIndex("dbo.RelayToRelayConnections", "relayID");
            AddForeignKey("dbo.RelayToRelayConnections", "relayID", "dbo.Relays", "relayID");
        }
    }
}
