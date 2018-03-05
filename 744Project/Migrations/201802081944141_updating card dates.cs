namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatingcarddates : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CreditCards", "cardExpirationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CreditCards", "cardExpirationDate", c => c.String(maxLength: 50));
        }
    }
}
