namespace Popcorn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Version = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        Culture = c.String(maxLength: 4000),
                        IsCurrentLanguage = c.Boolean(nullable: false),
                        ApplicationSettings_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationSettings", t => t.ApplicationSettings_Id)
                .Index(t => t.ApplicationSettings_Id);
            
            CreateTable(
                "dbo.UserDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MovieHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImdbCode = c.String(maxLength: 4000),
                        Liked = c.Boolean(nullable: false),
                        Seen = c.Boolean(nullable: false),
                        UserData_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserDatas", t => t.UserData_Id)
                .Index(t => t.UserData_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MovieHistories", "UserData_Id", "dbo.UserDatas");
            DropForeignKey("dbo.Languages", "ApplicationSettings_Id", "dbo.ApplicationSettings");
            DropIndex("dbo.MovieHistories", new[] { "UserData_Id" });
            DropIndex("dbo.Languages", new[] { "ApplicationSettings_Id" });
            DropTable("dbo.MovieHistories");
            DropTable("dbo.UserDatas");
            DropTable("dbo.Languages");
            DropTable("dbo.ApplicationSettings");
        }
    }
}
