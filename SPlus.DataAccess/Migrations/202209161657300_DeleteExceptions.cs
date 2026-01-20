namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteExceptions : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Exceptions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Exceptions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 255),
                        Message = c.String(),
                        Controller = c.String(maxLength: 255),
                        Function = c.String(maxLength: 255),
                        Modified = c.DateTime(),
                        Created = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
    }
}
