namespace PerformingArtsApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lessonrooms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lessons", "Room", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lessons", "Room");
        }
    }
}
