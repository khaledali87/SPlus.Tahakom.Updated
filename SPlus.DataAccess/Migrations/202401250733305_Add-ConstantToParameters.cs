namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConstantToParameters : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parameters", "IsConstant", c => c.Boolean(nullable: false));
            AddColumn("dbo.Parameters", "ConstantValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Parameters", "ConstantValue");
            DropColumn("dbo.Parameters", "IsConstant");
        }
    }
}
