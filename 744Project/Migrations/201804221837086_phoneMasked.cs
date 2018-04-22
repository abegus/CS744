namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class phoneMasked : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Accounts", "accountPhone", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Accounts", "accountPhone", c => c.String(nullable: false, maxLength: 10));
        }
    }
}
