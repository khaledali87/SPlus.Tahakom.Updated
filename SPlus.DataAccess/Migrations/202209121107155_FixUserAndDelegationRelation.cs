namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixUserAndDelegationRelation : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Delegation", name: "FromUser", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.Delegation", name: "ToUser", newName: "FromUser");
            RenameColumn(table: "dbo.Delegation", name: "__mig_tmp__0", newName: "ToUser");
            RenameIndex(table: "dbo.Delegation", name: "IX_ToUser", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.Delegation", name: "IX_FromUser", newName: "IX_ToUser");
            RenameIndex(table: "dbo.Delegation", name: "__mig_tmp__0", newName: "IX_FromUser");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Delegation", name: "IX_FromUser", newName: "__mig_tmp__0");
            RenameIndex(table: "dbo.Delegation", name: "IX_ToUser", newName: "IX_FromUser");
            RenameIndex(table: "dbo.Delegation", name: "__mig_tmp__0", newName: "IX_ToUser");
            RenameColumn(table: "dbo.Delegation", name: "ToUser", newName: "__mig_tmp__0");
            RenameColumn(table: "dbo.Delegation", name: "FromUser", newName: "ToUser");
            RenameColumn(table: "dbo.Delegation", name: "__mig_tmp__0", newName: "FromUser");
        }
    }
}
