namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Regions", "regionName", c => c.String(nullable: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Regions", "regionName", c => c.Int(nullable: false));
        }
    }
}
