namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedivision_V4 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Activity", new[] { "StrategicObjectiveID" });
            AddColumn("dbo.Activity", "DivisionalObjectiveID", c => c.Int());
            AlterColumn("dbo.Activity", "StrategicObjectiveID", c => c.Int());
            CreateIndex("dbo.Activity", "StrategicObjectiveID");
            CreateIndex("dbo.Activity", "DivisionalObjectiveID");
            AddForeignKey("dbo.Activity", "DivisionalObjectiveID", "dbo.DivisionalObjective", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Activity", "DivisionalObjectiveID", "dbo.DivisionalObjective");
            DropIndex("dbo.Activity", new[] { "DivisionalObjectiveID" });
            DropIndex("dbo.Activity", new[] { "StrategicObjectiveID" });
            AlterColumn("dbo.Activity", "StrategicObjectiveID", c => c.Int(nullable: false));
            DropColumn("dbo.Activity", "DivisionalObjectiveID");
            CreateIndex("dbo.Activity", "StrategicObjectiveID");
        }
    }
}
