namespace PerformingArtsApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lessontimes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lessons", "LessonTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lessons", "LessonTime");
        }
    }
}
