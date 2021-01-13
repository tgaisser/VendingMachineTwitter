namespace VendingMachine.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Step2SeperateTwitterAccountFromMachines : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.MachineTwitterAccounts", "TwitterAccountId", "dbo.TwitterAccounts", "Id", cascadeDelete: false);
            CreateIndex("dbo.MachineTwitterAccounts", "TwitterAccountId");
            DropColumn("dbo.MachineTwitterAccounts", "ScreenName");
            DropColumn("dbo.MachineTwitterAccounts", "Password");
            DropColumn("dbo.MachineTwitterAccounts", "ConsumerKey");
            DropColumn("dbo.MachineTwitterAccounts", "ConsumerSecret");
            DropColumn("dbo.MachineTwitterAccounts", "AccessToken");
            DropColumn("dbo.MachineTwitterAccounts", "AccessTokenSecret");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MachineTwitterAccounts", "AccessTokenSecret", c => c.String());
            AddColumn("dbo.MachineTwitterAccounts", "AccessToken", c => c.String());
            AddColumn("dbo.MachineTwitterAccounts", "ConsumerSecret", c => c.String());
            AddColumn("dbo.MachineTwitterAccounts", "ConsumerKey", c => c.String());
            AddColumn("dbo.MachineTwitterAccounts", "Password", c => c.String());
            AddColumn("dbo.MachineTwitterAccounts", "ScreenName", c => c.String());
            DropIndex("dbo.MachineTwitterAccounts", new[] { "TwitterAccountId" });
            DropForeignKey("dbo.MachineTwitterAccounts", "TwitterAccountId", "dbo.TwitterAccounts");
        }
    }
}
