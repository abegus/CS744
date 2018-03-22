namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedManyRelationships : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoresToRelays",
                c => new
                    {
                        storesToRelaysID = c.Int(nullable: false, identity: true),
                        relayID = c.String(maxLength: 50),
                        storeID = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.storesToRelaysID)
                .ForeignKey("dbo.Relays", t => t.relayID)
                .ForeignKey("dbo.Stores", t => t.storeID)
                .Index(t => t.relayID)
                .Index(t => t.storeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoresToRelays", "storeID", "dbo.Stores");
            DropForeignKey("dbo.StoresToRelays", "relayID", "dbo.Relays");
            DropIndex("dbo.StoresToRelays", new[] { "storeID" });
            DropIndex("dbo.StoresToRelays", new[] { "relayID" });
            DropTable("dbo.StoresToRelays");
        }
    }
}
