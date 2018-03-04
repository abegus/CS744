namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class securityquestions : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "answerToSecurity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "answerToSecurity", c => c.String());
        }
    }
}
