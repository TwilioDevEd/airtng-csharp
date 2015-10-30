namespace AirTNG.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateVacationProperties : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VacationProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Description = c.String(),
                        ImageUrl = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Reservations", "VactionPropertyId", c => c.Int(nullable: false));
            CreateIndex("dbo.Reservations", "VactionPropertyId");
            AddForeignKey("dbo.Reservations", "VactionPropertyId", "dbo.VacationProperties", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VacationProperties", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Reservations", "VactionPropertyId", "dbo.VacationProperties");
            DropIndex("dbo.VacationProperties", new[] { "UserId" });
            DropIndex("dbo.Reservations", new[] { "VactionPropertyId" });
            DropColumn("dbo.Reservations", "VactionPropertyId");
            DropTable("dbo.VacationProperties");
        }
    }
}
