namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nonrequiredmerchantforform : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Transactions", "transactionMerchant", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Transactions", "transactionMerchant", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
