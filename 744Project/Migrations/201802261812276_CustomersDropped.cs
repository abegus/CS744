namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomersDropped : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Accounts", "customerID", "dbo.Customers");
            DropForeignKey("dbo.CreditCards", "customerID", "dbo.Customers");
            DropIndex("dbo.Accounts", new[] { "customerID" });
            DropIndex("dbo.CreditCards", new[] { "customerID" });
            AddColumn("dbo.CreditCards", "firstName", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.CreditCards", "lastName", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.Accounts", "customerID");
            DropColumn("dbo.CreditCards", "customerID");
            DropTable("dbo.Customers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        customerID = c.Int(nullable: false, identity: true),
                        customerFirstname = c.String(nullable: false, maxLength: 50),
                        middleName = c.String(nullable: false, maxLength: 50),
                        customerLastname = c.String(nullable: false, maxLength: 50),
                        customerPhone = c.String(nullable: false, maxLength: 50),
                        customerSSN = c.String(nullable: false, maxLength: 50),
                        customerAddress = c.String(nullable: false, maxLength: 100),
                        customerCity = c.String(nullable: false, maxLength: 50),
                        customerState = c.String(nullable: false, maxLength: 50),
                        customerZip = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.customerID);
            
            AddColumn("dbo.CreditCards", "customerID", c => c.Int());
            AddColumn("dbo.Accounts", "customerID", c => c.Int());
            DropColumn("dbo.CreditCards", "lastName");
            DropColumn("dbo.CreditCards", "firstName");
            CreateIndex("dbo.CreditCards", "customerID");
            CreateIndex("dbo.Accounts", "customerID");
            AddForeignKey("dbo.CreditCards", "customerID", "dbo.Customers", "customerID");
            AddForeignKey("dbo.Accounts", "customerID", "dbo.Customers", "customerID");
        }
    }
}
