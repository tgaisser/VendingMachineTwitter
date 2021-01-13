namespace VendingMachine.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingRandomResponses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReplyTweets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tweet = c.String(nullable: false),
                        CreateDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        EventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .Index(t => t.EventId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ReplyTweets", new[] { "EventId" });
            DropForeignKey("dbo.ReplyTweets", "EventId", "dbo.Events");
            DropTable("dbo.ReplyTweets");
        }
    }
}
