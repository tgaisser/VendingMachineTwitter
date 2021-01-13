namespace VendingMachine.Repository.Migrations
{
	using System;
	using System.Data.Entity.Migrations;
	
	public partial class InitialMigration : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.Codes",
				c => new
					{
						Id = c.Int(nullable: false, identity: true),
						CodeValue = c.String(nullable: false),
						Description = c.String(),
						TweetId = c.String(),
						TweetMessage = c.String(),
						TweetUserId = c.String(),
						DateTweetCreated = c.DateTime(),
						DateAssigned = c.DateTime(),
						IsActive = c.Boolean(nullable: false, defaultValue: true),
						CreateDate = c.DateTime(defaultValueSql: "GETDATE()"),
						DenormalizedEventId = c.Int(nullable: false),
						MachineTwitterAccountId = c.Int(nullable: false),
					})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.MachineTwitterAccounts", t => t.MachineTwitterAccountId, cascadeDelete: true)
				.Index(t => t.MachineTwitterAccountId);
			
			CreateTable(
				"dbo.MachineTwitterAccounts",
				c => new
					{
						Id = c.Int(nullable: false, identity: true),
						MachineName = c.String(nullable: false),
						MachineDescription = c.String(),
						ScreenName = c.String(nullable: false),
						Password = c.String(),
						ConsumerKey = c.String(nullable: false),
						ConsumerSecret = c.String(nullable: false),
						AccessToken = c.String(nullable: false),
						AccessTokenSecret = c.String(nullable: false),
						HashTag = c.String(nullable: false),
						CreateDate = c.DateTime(defaultValueSql: "GETDATE()"),
						IsActive = c.Boolean(nullable: false, defaultValue: true),
						EventId = c.Int(nullable: false),
					})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
				.Index(t => t.EventId);
			
			CreateTable(
				"dbo.Events",
				c => new
					{
						Id = c.Int(nullable: false, identity: true),
						Name = c.String(nullable: false),
						Description = c.String(),
						StartDate = c.DateTime(),
						EndDate = c.DateTime(),
						CreateDate = c.DateTime(defaultValueSql: "GETDATE()"),
						IsActive = c.Boolean(nullable: false, defaultValue: true),
					})
				.PrimaryKey(t => t.Id);
			
			CreateTable(
				"dbo.TwitterUsers",
				c => new
					{
						Id = c.Int(nullable: false, identity: true),
						TwitterId = c.String(nullable: false),
						ScreenName = c.String(nullable: false),
						Name = c.String(),
						UserProtected = c.Boolean(nullable: false),
						IsActive = c.Boolean(nullable: false, defaultValue: true),
						CreateDate = c.DateTime(defaultValueSql: "GETDATE()"),
					})
				.PrimaryKey(t => t.Id);
			
		}
		
		public override void Down()
		{
			DropIndex("dbo.MachineTwitterAccounts", new[] { "EventId" });
			DropIndex("dbo.Codes", new[] { "MachineTwitterAccountId" });
			DropForeignKey("dbo.MachineTwitterAccounts", "EventId", "dbo.Events");
			DropForeignKey("dbo.Codes", "MachineTwitterAccountId", "dbo.MachineTwitterAccounts");
			DropTable("dbo.TwitterUsers");
			DropTable("dbo.Events");
			DropTable("dbo.MachineTwitterAccounts");
			DropTable("dbo.Codes");
		}
	}
}
