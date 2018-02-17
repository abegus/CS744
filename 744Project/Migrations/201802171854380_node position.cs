namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nodeposition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NodePositions",
                c => new
                    {
                        Ip = c.String(nullable: false, maxLength: 50),
                        x = c.Decimal(nullable: false, precision: 18, scale: 2),
                        y = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Ip);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NodePositions");
        }
    }
}
