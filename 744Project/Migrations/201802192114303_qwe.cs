namespace _744Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class qwe : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        QuestionID = c.Int(nullable: false, identity: true),
                        QuestionText = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.QuestionID);
            
            CreateIndex("dbo.SecurityQuestions", "QuestionID");
            AddForeignKey("dbo.SecurityQuestions", "QuestionID", "dbo.Questions", "QuestionID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SecurityQuestions", "QuestionID", "dbo.Questions");
            DropIndex("dbo.SecurityQuestions", new[] { "QuestionID" });
            DropTable("dbo.Questions");
        }
    }
}
