namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedGatewayIP : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Regions", "gatewayIP", c => c.String(nullable: true));            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Regions", "gatewayIP");
        }
    }
}
