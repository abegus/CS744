namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newRequiredFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Accounts", "accountNumber", c => c.String(nullable: false, maxLength: 50, unicode: false));
            AlterColumn("dbo.Accounts", "accountBalance", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Accounts", "accountName", c => c.String(nullable: false));
            AlterColumn("dbo.CreditCards", "cardNumber", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.CreditCards", "cardSecurityCode", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.CreditCards", "cardMaxAllowed", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "customerFirstname", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "middleName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "customerLastname", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "customerPhone", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "customerSSN", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "customerAddress", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Customers", "customerCity", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "customerState", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customers", "customerZip", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "customerZip", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customers", "customerState", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customers", "customerCity", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customers", "customerAddress", c => c.String(maxLength: 100));
            AlterColumn("dbo.Customers", "customerSSN", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customers", "customerPhone", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customers", "customerLastname", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customers", "middleName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customers", "customerFirstname", c => c.String(maxLength: 50));
            AlterColumn("dbo.CreditCards", "cardMaxAllowed", c => c.String(maxLength: 50));
            AlterColumn("dbo.CreditCards", "cardSecurityCode", c => c.String(maxLength: 50));
            AlterColumn("dbo.CreditCards", "cardNumber", c => c.String(maxLength: 50));
            AlterColumn("dbo.Accounts", "accountName", c => c.String());
            AlterColumn("dbo.Accounts", "accountBalance", c => c.String(maxLength: 50));
            AlterColumn("dbo.Accounts", "accountNumber", c => c.String(maxLength: 50, unicode: false));
        }
    }
}
