namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedatabase : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "accountID", "dbo.Accounts");
            DropForeignKey("dbo.Transactions", "cardID", "dbo.CreditCards");
            DropIndex("dbo.Transactions", new[] { "cardID" });
            DropIndex("dbo.Transactions", new[] { "accountID" });
            AddColumn("dbo.Transactions", "encryptedFlag", c => c.Boolean());
            AlterColumn("dbo.Transactions", "transactionTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Transactions", "transactionAmount", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Transactions", "transactionType", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Transactions", "transactionMerchant", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Transactions", "cardID", c => c.Int(nullable: false));
            CreateIndex("dbo.Transactions", "cardID");
            AddForeignKey("dbo.Transactions", "cardID", "dbo.CreditCards", "cardID", cascadeDelete: true);
            DropColumn("dbo.Transactions", "connectionID");
            DropColumn("dbo.Transactions", "accountID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "accountID", c => c.Int());
            AddColumn("dbo.Transactions", "connectionID", c => c.Int());
            DropForeignKey("dbo.Transactions", "cardID", "dbo.CreditCards");
            DropIndex("dbo.Transactions", new[] { "cardID" });
            AlterColumn("dbo.Transactions", "cardID", c => c.Int());
            AlterColumn("dbo.Transactions", "transactionMerchant", c => c.String(maxLength: 100));
            AlterColumn("dbo.Transactions", "transactionType", c => c.String(maxLength: 50));
            AlterColumn("dbo.Transactions", "transactionAmount", c => c.String(maxLength: 50));
            AlterColumn("dbo.Transactions", "transactionTime", c => c.DateTime());
            DropColumn("dbo.Transactions", "encryptedFlag");
            CreateIndex("dbo.Transactions", "accountID");
            CreateIndex("dbo.Transactions", "cardID");
            AddForeignKey("dbo.Transactions", "cardID", "dbo.CreditCards", "cardID");
            AddForeignKey("dbo.Transactions", "accountID", "dbo.Accounts", "accountID");
        }
    }
}
