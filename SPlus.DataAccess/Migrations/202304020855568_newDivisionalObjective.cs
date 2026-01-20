namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newDivisionalObjective : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DivisionalObjective",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnglishName = c.String(nullable: false),
                        ArabicName = c.String(nullable: false),
                        ArabicDescription = c.String(),
                        EnglishDescription = c.String(),
                        Owner = c.String(),
                        Order = c.Int(nullable: false),
                        Code = c.String(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgStructure_ID = c.Int(),
                        OwnerModel_UserName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OrgStructure", t => t.OrgStructure_ID)
                .ForeignKey("dbo.Users", t => t.OwnerModel_UserName)
                .Index(t => t.OrgStructure_ID)
                .Index(t => t.OwnerModel_UserName);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DivisionalObjective", "OwnerModel_UserName", "dbo.Users");
            DropForeignKey("dbo.DivisionalObjective", "OrgStructure_ID", "dbo.OrgStructure");
            DropIndex("dbo.DivisionalObjective", new[] { "OwnerModel_UserName" });
            DropIndex("dbo.DivisionalObjective", new[] { "OrgStructure_ID" });
            DropTable("dbo.DivisionalObjective");
        }
    }
}
