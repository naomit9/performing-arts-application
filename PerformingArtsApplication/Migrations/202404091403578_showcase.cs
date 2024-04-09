namespace PerformingArtsApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class showcase : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Showcases", "ShowcaseDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Showcases", "ShowcaseDate", c => c.DateTime(nullable: false));
        }
    }
}
