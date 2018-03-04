namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class creditcards : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CreditCards", "cardNumber", c => c.Long(nullable: false));
            AlterColumn("dbo.CreditCards", "cardSecurityCode", c => c.Int(nullable: false));
            AlterColumn("dbo.CreditCards", "cardMaxAllowed", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CreditCards", "cardMaxAllowed", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.CreditCards", "cardSecurityCode", c => c.String(nullable: false, maxLength: 3));
            AlterColumn("dbo.CreditCards", "cardNumber", c => c.String(nullable: false, maxLength: 16));
        }
    }
}
