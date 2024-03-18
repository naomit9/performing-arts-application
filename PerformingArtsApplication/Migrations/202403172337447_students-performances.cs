namespace PerformingArtsApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class studentsperformances : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PerformanceStudents",
                c => new
                    {
                        Performance_PerformanceId = c.Int(nullable: false),
                        Student_StudentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Performance_PerformanceId, t.Student_StudentId })
                .ForeignKey("dbo.Performances", t => t.Performance_PerformanceId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.Student_StudentId, cascadeDelete: true)
                .Index(t => t.Performance_PerformanceId)
                .Index(t => t.Student_StudentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PerformanceStudents", "Student_StudentId", "dbo.Students");
            DropForeignKey("dbo.PerformanceStudents", "Performance_PerformanceId", "dbo.Performances");
            DropIndex("dbo.PerformanceStudents", new[] { "Student_StudentId" });
            DropIndex("dbo.PerformanceStudents", new[] { "Performance_PerformanceId" });
            DropTable("dbo.PerformanceStudents");
        }
    }
}
