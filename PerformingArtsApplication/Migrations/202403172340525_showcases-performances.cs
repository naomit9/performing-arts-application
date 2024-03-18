namespace PerformingArtsApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class showcasesperformances : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShowcasePerformances",
                c => new
                    {
                        Showcase_ShowcaseId = c.Int(nullable: false),
                        Performance_PerformanceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Showcase_ShowcaseId, t.Performance_PerformanceId })
                .ForeignKey("dbo.Showcases", t => t.Showcase_ShowcaseId, cascadeDelete: true)
                .ForeignKey("dbo.Performances", t => t.Performance_PerformanceId, cascadeDelete: true)
                .Index(t => t.Showcase_ShowcaseId)
                .Index(t => t.Performance_PerformanceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShowcasePerformances", "Performance_PerformanceId", "dbo.Performances");
            DropForeignKey("dbo.ShowcasePerformances", "Showcase_ShowcaseId", "dbo.Showcases");
            DropIndex("dbo.ShowcasePerformances", new[] { "Performance_PerformanceId" });
            DropIndex("dbo.ShowcasePerformances", new[] { "Showcase_ShowcaseId" });
            DropTable("dbo.ShowcasePerformances");
        }
    }
}
