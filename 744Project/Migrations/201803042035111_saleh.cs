namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class saleh : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "numFailedAttempts");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "numFailedAttempts", c => c.Int(nullable: false));
        }
    }
}
