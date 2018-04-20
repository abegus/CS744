namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transaction2 : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Transactions", "CreditCard_cardID", "dbo.CreditCards");
            //DropIndex("dbo.Transactions", new[] { "CreditCard_cardID" });
            //DropColumn("dbo.Transactions", "CreditCard_cardID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "CreditCard_cardID", c => c.Int());
            CreateIndex("dbo.Transactions", "CreditCard_cardID");
            AddForeignKey("dbo.Transactions", "CreditCard_cardID", "dbo.CreditCards", "cardID");
        }
    }
}
