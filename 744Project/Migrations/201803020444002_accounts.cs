namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accounts : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Accounts", "accountBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Accounts", "accountZip", c => c.Int(nullable: false));
            AlterColumn("dbo.Accounts", "accountPhone", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Accounts", "accountPhone", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.Accounts", "accountZip", c => c.String(nullable: false, maxLength: 5));
            AlterColumn("dbo.Accounts", "accountBalance", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
