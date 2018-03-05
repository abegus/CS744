namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinalAccounts : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Accounts", "accountName", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Accounts", "accountZip", c => c.String(nullable: false, maxLength: 5));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Accounts", "accountZip", c => c.Int(nullable: false));
            AlterColumn("dbo.Accounts", "accountName", c => c.String(nullable: false));
        }
    }
}
