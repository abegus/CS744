namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomersDropped2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CreditCards", "firstName", c => c.String(maxLength: 50));
            AlterColumn("dbo.CreditCards", "lastName", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CreditCards", "lastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.CreditCards", "firstName", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
