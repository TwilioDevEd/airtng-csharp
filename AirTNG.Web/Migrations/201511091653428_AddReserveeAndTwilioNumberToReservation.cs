namespace AirTNG.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReserveeAndTwilioNumberToReservation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "UserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Reservations", "TwilioNumber", c => c.String());
            CreateIndex("dbo.Reservations", "UserId");
            AddForeignKey("dbo.Reservations", "UserId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Reservations", "Name");
            DropColumn("dbo.Reservations", "PhoneNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservations", "PhoneNumber", c => c.String());
            AddColumn("dbo.Reservations", "Name", c => c.String());
            DropForeignKey("dbo.Reservations", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Reservations", new[] { "UserId" });
            DropColumn("dbo.Reservations", "TwilioNumber");
            DropColumn("dbo.Reservations", "UserId");
        }
    }
}
