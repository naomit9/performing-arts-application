namespace PerformingArtsApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class performances : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Performances",
                c => new
                    {
                        PerformanceId = c.Int(nullable: false, identity: true),
                        PerformanceName = c.String(),
                    })
                .PrimaryKey(t => t.PerformanceId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Performances");
        }
    }
}
