namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transactions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "cardID", "dbo.CreditCards");
            DropIndex("dbo.Transactions", new[] { "cardID" });
            RenameColumn(table: "dbo.Transactions", name: "cardID", newName: "CreditCard_cardID");
            AddColumn("dbo.Transactions", "cardNumber", c => c.Long(nullable: false));
            AddColumn("dbo.Transactions", "storeIP", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Transactions", "isSelf", c => c.Boolean());
            AlterColumn("dbo.Transactions", "CreditCard_cardID", c => c.Int());
            CreateIndex("dbo.Transactions", "CreditCard_cardID");
            AddForeignKey("dbo.Transactions", "CreditCard_cardID", "dbo.CreditCards", "cardID");
            DropColumn("dbo.Transactions", "storeID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "storeID", c => c.String(nullable: false, maxLength: 50));
            DropForeignKey("dbo.Transactions", "CreditCard_cardID", "dbo.CreditCards");
            DropIndex("dbo.Transactions", new[] { "CreditCard_cardID" });
            AlterColumn("dbo.Transactions", "CreditCard_cardID", c => c.Int(nullable: false));
            DropColumn("dbo.Transactions", "isSelf");
            DropColumn("dbo.Transactions", "storeIP");
            DropColumn("dbo.Transactions", "cardNumber");
            RenameColumn(table: "dbo.Transactions", name: "CreditCard_cardID", newName: "cardID");
            CreateIndex("dbo.Transactions", "cardID");
            AddForeignKey("dbo.Transactions", "cardID", "dbo.CreditCards", "cardID", cascadeDelete: true);
        }
    }
}
