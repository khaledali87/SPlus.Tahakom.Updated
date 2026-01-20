namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixSponsorTypo : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.KPI", name: "Sponser", newName: "Sponsor");
            RenameIndex(table: "dbo.KPI", name: "IX_Sponser", newName: "IX_Sponsor");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.KPI", name: "IX_Sponsor", newName: "IX_Sponser");
            RenameColumn(table: "dbo.KPI", name: "Sponsor", newName: "Sponser");
        }
    }
}
