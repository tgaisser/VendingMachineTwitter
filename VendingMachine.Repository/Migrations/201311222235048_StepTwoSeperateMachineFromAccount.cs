namespace VendingMachine.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StepTwoSeperateMachineFromAccount : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.MachineTwitterAccounts", "MachineId", "dbo.Machines", "Id", cascadeDelete: true);
            CreateIndex("dbo.MachineTwitterAccounts", "MachineId");
            DropColumn("dbo.MachineTwitterAccounts", "MachineName");
            DropColumn("dbo.MachineTwitterAccounts", "MachineDescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MachineTwitterAccounts", "MachineDescription", c => c.String());
            AddColumn("dbo.MachineTwitterAccounts", "MachineName", c => c.String());
            DropIndex("dbo.MachineTwitterAccounts", new[] { "MachineId" });
            DropForeignKey("dbo.MachineTwitterAccounts", "MachineId", "dbo.Machines");
        }
    }
}
