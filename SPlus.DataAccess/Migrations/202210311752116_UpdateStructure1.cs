namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStructure1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Perspective", name: "Strategy_ID", newName: "StrategyID");
            RenameIndex(table: "dbo.Perspective", name: "IX_Strategy_ID", newName: "IX_StrategyID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Perspective", name: "IX_StrategyID", newName: "IX_Strategy_ID");
            RenameColumn(table: "dbo.Perspective", name: "StrategyID", newName: "Strategy_ID");
        }
    }
}
