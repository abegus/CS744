namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedCardLimit : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.CreditCards", "cardMaxAllowed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CreditCards", "cardMaxAllowed", c => c.Int(nullable: false));
        }
    }
}
