namespace Webcam.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhotoMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Bytes = c.String(),
                        ImageType = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(maxLength: 30));
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Photos", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
            DropTable("dbo.Photos");
        }
    }
}
