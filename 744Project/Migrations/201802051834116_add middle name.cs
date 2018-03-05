namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmiddlename : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "middleName", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "middleName");
        }
    }
}
