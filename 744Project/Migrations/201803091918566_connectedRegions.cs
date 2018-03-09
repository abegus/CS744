namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class connectedRegions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        regionID = c.Int(nullable: false, identity: true),
                        regionName = c.String(nullable: true),
                        gatewayIP = c.String(nullable: true),
                    })
                .PrimaryKey(t => t.regionID);
            
            AddColumn("dbo.Relays", "regionID", c => c.Int(nullable: false));
            AddColumn("dbo.Relays", "isGateway", c => c.Boolean(nullable: true));
            AddColumn("dbo.Stores", "regionID", c => c.Int(nullable: false));
            CreateIndex("dbo.Relays", "regionID");
            CreateIndex("dbo.Stores", "regionID");
            AddForeignKey("dbo.Relays", "regionID", "dbo.Regions", "regionID", cascadeDelete: true);
            AddForeignKey("dbo.Stores", "regionID", "dbo.Regions", "regionID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stores", "regionID", "dbo.Regions");
            DropForeignKey("dbo.Relays", "regionID", "dbo.Regions");
            DropIndex("dbo.Stores", new[] { "regionID" });
            DropIndex("dbo.Relays", new[] { "regionID" });
            DropColumn("dbo.Stores", "regionID");
            DropColumn("dbo.Relays", "isGateway");
            DropColumn("dbo.Relays", "regionID");
            DropTable("dbo.Regions");
        }
    }
}
