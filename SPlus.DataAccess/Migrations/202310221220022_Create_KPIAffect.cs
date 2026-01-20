namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_KPIAffect : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KPIAffect",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        KPIID = c.Int(nullable: false),
                        Affected = c.Int(nullable: false),
                        Effecting = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.KPIAffect");
        }
    }
}
