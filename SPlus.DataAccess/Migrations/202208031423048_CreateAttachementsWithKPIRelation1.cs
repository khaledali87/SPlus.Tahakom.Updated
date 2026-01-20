namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateAttachementsWithKPIRelation1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Attachements");
            AddColumn("dbo.Attachements", "Name", c => c.String(nullable: false));
            AddColumn("dbo.Attachements", "FileName", c => c.String(nullable: false));
            AddColumn("dbo.Attachements", "Size", c => c.Int(nullable: false));
            AddColumn("dbo.Attachements", "Type", c => c.String(nullable: false));
            AddColumn("dbo.Attachements", "Content", c => c.Binary(nullable: false));
            AddColumn("dbo.Attachements", "KPI_ID", c => c.Int());
            AddColumn("dbo.Perspective", "Attachement_ID", c => c.Int());
            AddColumn("dbo.Theme", "AttachementID", c => c.String());
            AddColumn("dbo.Theme", "Attachement_ID", c => c.Int());
            AlterColumn("dbo.Attachements", "ID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Attachements", "Created", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Attachements", "Modified", c => c.DateTime(nullable: false));
            AddPrimaryKey("dbo.Attachements", "ID");
            CreateIndex("dbo.Attachements", "KPI_ID");
            CreateIndex("dbo.Perspective", "Attachement_ID");
            CreateIndex("dbo.Theme", "Attachement_ID");
            AddForeignKey("dbo.Attachements", "KPI_ID", "dbo.KPI", "ID");
            AddForeignKey("dbo.Perspective", "Attachement_ID", "dbo.Attachements", "ID");
            AddForeignKey("dbo.Theme", "Attachement_ID", "dbo.Attachements", "ID");
            DropColumn("dbo.Attachements", "AttachementID");
            DropColumn("dbo.Attachements", "AttachementName");
            DropColumn("dbo.Attachements", "AttachementSize");
            DropColumn("dbo.Attachements", "AttachementType");
            DropColumn("dbo.Attachements", "AttachementURL");
            DropColumn("dbo.Attachements", "RelatedItemID");
            DropColumn("dbo.Attachements", "KPIID");
            DropColumn("dbo.Attachements", "AttachementDate");
            DropColumn("dbo.Attachements", "ItemType");

            //DropColumn("dbo.Perspective", "AttachementID");
            //DropColumn("dbo.Theme", "AttachementID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Attachements", "ItemType", c => c.String(maxLength: 255));
            AddColumn("dbo.Attachements", "AttachementDate", c => c.String(maxLength: 255));
            AddColumn("dbo.Attachements", "KPIID", c => c.String(maxLength: 255));
            AddColumn("dbo.Attachements", "RelatedItemID", c => c.String(maxLength: 255));
            AddColumn("dbo.Attachements", "AttachementURL", c => c.String(maxLength: 255));
            AddColumn("dbo.Attachements", "AttachementType", c => c.String(maxLength: 255));
            AddColumn("dbo.Attachements", "AttachementSize", c => c.String(maxLength: 255));
            AddColumn("dbo.Attachements", "AttachementName", c => c.String(maxLength: 255));
            AddColumn("dbo.Attachements", "AttachementID", c => c.String(nullable: false, maxLength: 255));
            DropForeignKey("dbo.Theme", "Attachement_ID", "dbo.Attachements");
            DropForeignKey("dbo.Perspective", "Attachement_ID", "dbo.Attachements");
            DropForeignKey("dbo.Attachements", "KPI_ID", "dbo.KPI");
            DropIndex("dbo.Theme", new[] { "Attachement_ID" });
            DropIndex("dbo.Perspective", new[] { "Attachement_ID" });
            DropIndex("dbo.Attachements", new[] { "KPI_ID" });
            DropPrimaryKey("dbo.Attachements");
            AlterColumn("dbo.Attachements", "Modified", c => c.DateTime());
            AlterColumn("dbo.Attachements", "Created", c => c.DateTime());
            AlterColumn("dbo.Attachements", "ID", c => c.Int(nullable: false));
            DropColumn("dbo.Theme", "Attachement_ID");
            DropColumn("dbo.Theme", "AttachementID");
            DropColumn("dbo.Perspective", "Attachement_ID");
            DropColumn("dbo.Perspective", "AttachementID");
            DropColumn("dbo.Attachements", "KPI_ID");
            DropColumn("dbo.Attachements", "Content");
            DropColumn("dbo.Attachements", "Type");
            DropColumn("dbo.Attachements", "Size");
            DropColumn("dbo.Attachements", "FileName");
            DropColumn("dbo.Attachements", "Name");
            AddPrimaryKey("dbo.Attachements", "ID");


            AddColumn("dbo.Theme", "AttachementID", c => c.String());
            AddColumn("dbo.Perspective", "AttachementID", c => c.String());
        }
    }
}
