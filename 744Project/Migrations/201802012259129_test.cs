namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "accountName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "accountName");
        }
    }
}
