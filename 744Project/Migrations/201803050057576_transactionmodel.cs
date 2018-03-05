namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transactionmodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "storeID", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transactions", "storeID");
        }
    }
}
