namespace VendingMachine.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StepOneAddTwitterAccountLogins : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TwitterAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ScreenName = c.String(nullable: false),
                        Password = c.String(),
                        ConsumerKey = c.String(nullable: false),
                        ConsumerSecret = c.String(nullable: false),
                        AccessToken = c.String(nullable: false),
                        AccessTokenSecret = c.String(nullable: false),
                        CreateDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.MachineTwitterAccounts", "TwitterAccountId", c => c.Int(nullable: false));
            AlterColumn("dbo.MachineTwitterAccounts", "ScreenName", c => c.String());
            AlterColumn("dbo.MachineTwitterAccounts", "ConsumerKey", c => c.String());
            AlterColumn("dbo.MachineTwitterAccounts", "ConsumerSecret", c => c.String());
            AlterColumn("dbo.MachineTwitterAccounts", "AccessToken", c => c.String());
            AlterColumn("dbo.MachineTwitterAccounts", "AccessTokenSecret", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MachineTwitterAccounts", "AccessTokenSecret", c => c.String(nullable: false));
            AlterColumn("dbo.MachineTwitterAccounts", "AccessToken", c => c.String(nullable: false));
            AlterColumn("dbo.MachineTwitterAccounts", "ConsumerSecret", c => c.String(nullable: false));
            AlterColumn("dbo.MachineTwitterAccounts", "ConsumerKey", c => c.String(nullable: false));
            AlterColumn("dbo.MachineTwitterAccounts", "ScreenName", c => c.String(nullable: false));
            DropColumn("dbo.MachineTwitterAccounts", "TwitterAccountId");
            DropTable("dbo.TwitterAccounts");
        }
    }
}
