namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteTables : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.WF_ChangeRequest");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.WF_ChangeRequest",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RequestID = c.Int(),
                        KPIID = c.Int(),
                        MeasureID = c.Int(),
                        NewTarget = c.Decimal(precision: 18, scale: 2),
                        OldTarget = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID);
            
        }
    }
}
