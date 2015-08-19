namespace Popcorn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MovieHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MovieFulls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MovieId = c.Int(nullable: false),
                        IsFavorite = c.Boolean(nullable: false),
                        HasBeenSeen = c.Boolean(nullable: false),
                        Url = c.String(maxLength: 4000),
                        ImdbCode = c.String(maxLength: 4000),
                        Title = c.String(maxLength: 4000),
                        TitleLong = c.String(maxLength: 4000),
                        Year = c.Int(nullable: false),
                        Rating = c.String(maxLength: 4000),
                        Runtime = c.Int(nullable: false),
                        Language = c.String(maxLength: 4000),
                        MpaRating = c.String(maxLength: 4000),
                        DownloadCount = c.String(maxLength: 4000),
                        LikeCount = c.String(maxLength: 4000),
                        RtCrtiticsScore = c.String(maxLength: 4000),
                        RtCriticsRating = c.String(maxLength: 4000),
                        RtAudienceScore = c.String(maxLength: 4000),
                        RtAudienceRating = c.String(maxLength: 4000),
                        DescriptionIntro = c.String(maxLength: 4000),
                        DescriptionFull = c.String(maxLength: 4000),
                        YtTrailerCode = c.String(maxLength: 4000),
                        DateUploaded = c.String(maxLength: 4000),
                        DateUploadedUnix = c.Int(nullable: false),
                        Images_Id = c.Int(),
                        MovieHistory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Images", t => t.Images_Id)
                .ForeignKey("dbo.MovieHistories", t => t.MovieHistory_Id)
                .Index(t => t.Images_Id)
                .Index(t => t.MovieHistory_Id);
            
            CreateTable(
                "dbo.Actors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        CharacterName = c.String(maxLength: 4000),
                        SmallImage = c.String(maxLength: 4000),
                        MediumImage = c.String(maxLength: 4000),
                        SmallImagePath = c.String(maxLength: 4000),
                        MovieFull_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MovieFulls", t => t.MovieFull_Id)
                .Index(t => t.MovieFull_Id);
            
            CreateTable(
                "dbo.Directors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        SmallImage = c.String(maxLength: 4000),
                        MediumImage = c.String(maxLength: 4000),
                        SmallImagePath = c.String(maxLength: 4000),
                        MovieFull_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MovieFulls", t => t.MovieFull_Id)
                .Index(t => t.MovieFull_Id);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        MovieFull_Id = c.Int(),
                        MovieShort_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MovieFulls", t => t.MovieFull_Id)
                .ForeignKey("dbo.MovieShorts", t => t.MovieShort_Id)
                .Index(t => t.MovieFull_Id)
                .Index(t => t.MovieShort_Id);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BackgroundImage = c.String(maxLength: 4000),
                        SmallCoverImage = c.String(maxLength: 4000),
                        MediumCoverImage = c.String(maxLength: 4000),
                        LargeCoverImage = c.String(maxLength: 4000),
                        MediumScreenshotImage1 = c.String(maxLength: 4000),
                        MediumScreenshotImage2 = c.String(maxLength: 4000),
                        MediumScreenshotImage3 = c.String(maxLength: 4000),
                        LargeScreenshotImage1 = c.String(maxLength: 4000),
                        LargeScreenshotImage2 = c.String(maxLength: 4000),
                        LargeScreenshotImage3 = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Torrents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(maxLength: 4000),
                        Hash = c.String(maxLength: 4000),
                        Quality = c.String(maxLength: 4000),
                        Framerate = c.String(maxLength: 4000),
                        Resolution = c.String(maxLength: 4000),
                        Seeds = c.Int(nullable: false),
                        Peers = c.Int(nullable: false),
                        Size = c.String(maxLength: 4000),
                        SizeBytes = c.Long(nullable: false),
                        DateUploaded = c.String(maxLength: 4000),
                        DateUploadedMix = c.Int(nullable: false),
                        MovieFull_Id = c.Int(),
                        MovieShort_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MovieFulls", t => t.MovieFull_Id)
                .ForeignKey("dbo.MovieShorts", t => t.MovieShort_Id)
                .Index(t => t.MovieFull_Id)
                .Index(t => t.MovieShort_Id);
            
            CreateTable(
                "dbo.MovieShorts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MovieId = c.Int(nullable: false),
                        IsFavorite = c.Boolean(nullable: false),
                        HasBeenSeen = c.Boolean(nullable: false),
                        Url = c.String(maxLength: 4000),
                        ImdbCode = c.String(maxLength: 4000),
                        Title = c.String(maxLength: 4000),
                        TitleLong = c.String(maxLength: 4000),
                        Year = c.Int(nullable: false),
                        Rating = c.String(maxLength: 4000),
                        Runtime = c.Int(nullable: false),
                        Language = c.String(maxLength: 4000),
                        MpaRating = c.String(maxLength: 4000),
                        SmallCoverImage = c.String(maxLength: 4000),
                        MediumCoverImage = c.String(maxLength: 4000),
                        State = c.String(maxLength: 4000),
                        DateUploaded = c.String(maxLength: 4000),
                        DateUploadedUnix = c.Int(nullable: false),
                        ServerTime = c.Int(nullable: false),
                        ServerTimezone = c.String(maxLength: 4000),
                        ApiVersion = c.Int(nullable: false),
                        ExecutionTime = c.String(maxLength: 4000),
                        CoverImagePath = c.String(maxLength: 4000),
                        MovieHistory_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MovieHistories", t => t.MovieHistory_Id)
                .Index(t => t.MovieHistory_Id);
            
            CreateTable(
                "dbo.Settings",
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
                        LocalizedName = c.String(maxLength: 4000),
                        EnglishName = c.String(maxLength: 4000),
                        Culture = c.String(maxLength: 4000),
                        IsCurrentLanguage = c.Boolean(nullable: false),
                        Settings_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Settings", t => t.Settings_Id)
                .Index(t => t.Settings_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Languages", "Settings_Id", "dbo.Settings");
            DropForeignKey("dbo.MovieShorts", "MovieHistory_Id", "dbo.MovieHistories");
            DropForeignKey("dbo.Torrents", "MovieShort_Id", "dbo.MovieShorts");
            DropForeignKey("dbo.Genres", "MovieShort_Id", "dbo.MovieShorts");
            DropForeignKey("dbo.MovieFulls", "MovieHistory_Id", "dbo.MovieHistories");
            DropForeignKey("dbo.Torrents", "MovieFull_Id", "dbo.MovieFulls");
            DropForeignKey("dbo.MovieFulls", "Images_Id", "dbo.Images");
            DropForeignKey("dbo.Genres", "MovieFull_Id", "dbo.MovieFulls");
            DropForeignKey("dbo.Directors", "MovieFull_Id", "dbo.MovieFulls");
            DropForeignKey("dbo.Actors", "MovieFull_Id", "dbo.MovieFulls");
            DropIndex("dbo.Languages", new[] { "Settings_Id" });
            DropIndex("dbo.MovieShorts", new[] { "MovieHistory_Id" });
            DropIndex("dbo.Torrents", new[] { "MovieShort_Id" });
            DropIndex("dbo.Torrents", new[] { "MovieFull_Id" });
            DropIndex("dbo.Genres", new[] { "MovieShort_Id" });
            DropIndex("dbo.Genres", new[] { "MovieFull_Id" });
            DropIndex("dbo.Directors", new[] { "MovieFull_Id" });
            DropIndex("dbo.Actors", new[] { "MovieFull_Id" });
            DropIndex("dbo.MovieFulls", new[] { "MovieHistory_Id" });
            DropIndex("dbo.MovieFulls", new[] { "Images_Id" });
            DropTable("dbo.Languages");
            DropTable("dbo.Settings");
            DropTable("dbo.MovieShorts");
            DropTable("dbo.Torrents");
            DropTable("dbo.Images");
            DropTable("dbo.Genres");
            DropTable("dbo.Directors");
            DropTable("dbo.Actors");
            DropTable("dbo.MovieFulls");
            DropTable("dbo.MovieHistories");
        }
    }
}
