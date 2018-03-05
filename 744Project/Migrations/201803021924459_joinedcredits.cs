namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class joinedcredits : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CreditCards", "accountID", "dbo.Accounts");
            DropIndex("dbo.CreditCards", new[] { "accountID" });
            AlterColumn("dbo.CreditCards", "accountID", c => c.Int(nullable: false));
            CreateIndex("dbo.CreditCards", "accountID");
            AddForeignKey("dbo.CreditCards", "accountID", "dbo.Accounts", "accountID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CreditCards", "accountID", "dbo.Accounts");
            DropIndex("dbo.CreditCards", new[] { "accountID" });
            AlterColumn("dbo.CreditCards", "accountID", c => c.Int());
            CreateIndex("dbo.CreditCards", "accountID");
            AddForeignKey("dbo.CreditCards", "accountID", "dbo.Accounts", "accountID");
        }
    }
}
