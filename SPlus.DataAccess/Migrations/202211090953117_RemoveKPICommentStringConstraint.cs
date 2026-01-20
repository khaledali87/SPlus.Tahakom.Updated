namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveKPICommentStringConstraint : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.KPIComments", "Comment", c => c.String(nullable: false));
            AlterColumn("dbo.KPIComments", "CreatedBy", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KPIComments", "CreatedBy", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.KPIComments", "Comment", c => c.String(nullable: false, maxLength: 255));
        }
    }
}
