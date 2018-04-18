namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editedAccountsAndcreditCards : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Accounts", "accountName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Accounts", "accountName", c => c.String(nullable: false, maxLength: 150));
        }
    }
}
