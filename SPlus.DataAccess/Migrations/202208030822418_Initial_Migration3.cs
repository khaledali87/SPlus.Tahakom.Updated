namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Migration3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LookupValue", "LookupValue2_ID", "dbo.LookupValue");
            DropIndex("dbo.LookupValue", new[] { "LookupValue2_ID" });
            AddColumn("dbo.LookupValue", "LookupID", c => c.Int());
            CreateIndex("dbo.LookupValue", "LookupID");
            AddForeignKey("dbo.LookupValue", "LookupID", "dbo.Lookup", "ID");
            DropColumn("dbo.LookupValue", "Lookup");
            DropColumn("dbo.LookupValue", "LookupValue2_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LookupValue", "LookupValue2_ID", c => c.Int(nullable: false));
            AddColumn("dbo.LookupValue", "Lookup", c => c.Int());
            DropForeignKey("dbo.LookupValue", "LookupID", "dbo.Lookup");
            DropIndex("dbo.LookupValue", new[] { "LookupID" });
            DropColumn("dbo.LookupValue", "LookupID");
            CreateIndex("dbo.LookupValue", "LookupValue2_ID");
            AddForeignKey("dbo.LookupValue", "LookupValue2_ID", "dbo.LookupValue", "ID");
        }
    }
}
