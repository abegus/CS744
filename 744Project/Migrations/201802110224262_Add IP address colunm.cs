namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIPaddresscolunm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProcessCenters", "processCenterIP", c => c.String(maxLength: 50));
            AlterColumn("dbo.CreditCards", "cardExpirationDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.ProcessCenters", "processIP");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProcessCenters", "processIP", c => c.String(maxLength: 50));
            AlterColumn("dbo.CreditCards", "cardExpirationDate", c => c.String(maxLength: 50));
            DropColumn("dbo.ProcessCenters", "processCenterIP");
        }
    }
}
