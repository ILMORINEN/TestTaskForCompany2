namespace PTMK.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestTask : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Human",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        Birthdate = c.DateTime(nullable: false),
                        Sex = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Human");
        }
    }
}
