namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveStringConstraintFromStrategy : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Strategy", "EnglishName", c => c.String(nullable: false));
            AlterColumn("dbo.Strategy", "ArabicName", c => c.String(nullable: false));
            AlterColumn("dbo.Strategy", "EnglishVision", c => c.String());
            AlterColumn("dbo.Strategy", "ArabicVision", c => c.String());
            AlterColumn("dbo.Strategy", "EnglishMission", c => c.String());
            AlterColumn("dbo.Strategy", "ArabicMission", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Strategy", "ArabicMission", c => c.String(maxLength: 255));
            AlterColumn("dbo.Strategy", "EnglishMission", c => c.String(maxLength: 255));
            AlterColumn("dbo.Strategy", "ArabicVision", c => c.String(maxLength: 255));
            AlterColumn("dbo.Strategy", "EnglishVision", c => c.String(maxLength: 255));
            AlterColumn("dbo.Strategy", "ArabicName", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Strategy", "EnglishName", c => c.String(nullable: false, maxLength: 255));
        }
    }
}
