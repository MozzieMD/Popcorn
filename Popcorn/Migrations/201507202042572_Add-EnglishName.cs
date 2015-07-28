namespace Popcorn.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEnglishName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Languages", "LocalizedName", c => c.String(maxLength: 4000));
            AddColumn("dbo.Languages", "EnglishName", c => c.String(maxLength: 4000));
            DropColumn("dbo.Languages", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Languages", "Name", c => c.String(maxLength: 4000));
            DropColumn("dbo.Languages", "EnglishName");
            DropColumn("dbo.Languages", "LocalizedName");
        }
    }
}
