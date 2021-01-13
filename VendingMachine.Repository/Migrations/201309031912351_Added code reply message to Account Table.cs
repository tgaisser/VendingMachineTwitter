namespace VendingMachine.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedcodereplymessagetoAccountTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MachineTwitterAccounts", "CodeReplyMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MachineTwitterAccounts", "CodeReplyMessage");
        }
    }
}
