namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aftergettingManyErrors : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "answerToSecurity", c => c.String());
            DropColumn("dbo.Transactions", "storeID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "storeID", c => c.String(nullable: false));
            DropColumn("dbo.AspNetUsers", "answerToSecurity");
        }
    }
}
