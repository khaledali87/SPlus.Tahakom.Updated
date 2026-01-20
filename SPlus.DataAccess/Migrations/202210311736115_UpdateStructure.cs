namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStructure : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Perspective", "Strategy_ID", "dbo.Strategy");
            DropIndex("dbo.Perspective", new[] { "Strategy_ID" });
            AlterColumn("dbo.Perspective", "Strategy_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.Perspective", "Strategy_ID");
            AddForeignKey("dbo.Perspective", "Strategy_ID", "dbo.Strategy", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Perspective", "Strategy_ID", "dbo.Strategy");
            DropIndex("dbo.Perspective", new[] { "Strategy_ID" });
            AlterColumn("dbo.Perspective", "Strategy_ID", c => c.Int());
            CreateIndex("dbo.Perspective", "Strategy_ID");
            AddForeignKey("dbo.Perspective", "Strategy_ID", "dbo.Strategy", "ID");
        }
    }
}
