namespace AirTNG.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAreaCodeToUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AreaCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AreaCode");
        }
    }
}
