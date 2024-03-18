namespace PerformingArtsApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lessons : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Lessons",
                c => new
                    {
                        LessonId = c.Int(nullable: false, identity: true),
                        LessonName = c.String(),
                        TeacherId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LessonId)
                .ForeignKey("dbo.Teachers", t => t.TeacherId, cascadeDelete: true)
                .Index(t => t.TeacherId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lessons", "TeacherId", "dbo.Teachers");
            DropIndex("dbo.Lessons", new[] { "TeacherId" });
            DropTable("dbo.Lessons");
        }
    }
}
