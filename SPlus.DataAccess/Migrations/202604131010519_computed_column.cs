namespace SPlus.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class computed_column : DbMigration
    {
        public override void Up()
        {
            Sql(@"
            ALTER TABLE Request
            ADD RelatedID AS TRY_CAST(JSON_VALUE(Form, '$.RelatedID') AS INT) PERSISTED
           ");
        }

        public override void Down()
        {
            Sql("ALTER TABLE Request DROP COLUMN RelatedID");
        }
    }
}
