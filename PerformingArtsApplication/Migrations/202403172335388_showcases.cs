namespace PerformingArtsApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class showcases : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Showcases",
                c => new
                    {
                        ShowcaseId = c.Int(nullable: false, identity: true),
                        ShowcaseName = c.String(),
                        ShowcaseDate = c.DateTime(nullable: false),
                        Location = c.String(),
                    })
                .PrimaryKey(t => t.ShowcaseId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Showcases");
        }
    }
}
