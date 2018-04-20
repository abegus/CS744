namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transactionsEdited : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Transactions", "transactionMerchant", c => c.String(nullable: false));
            AlterColumn("dbo.Transactions", "isSelf", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Transactions", "isSelf", c => c.Boolean());
            AlterColumn("dbo.Transactions", "transactionMerchant", c => c.String());
        }
    }
}
