namespace AirTNG.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameTwilioNumberToAnonymousPhoneNumberAtReservations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "AnonymousPhoneNumber", c => c.String());
            DropColumn("dbo.Reservations", "TwilioNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservations", "TwilioNumber", c => c.String());
            DropColumn("dbo.Reservations", "AnonymousPhoneNumber");
        }
    }
}
