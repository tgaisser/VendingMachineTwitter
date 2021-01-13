namespace VendingMachine.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class STEPOneSeperateMachineFromAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Machines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MachineName = c.String(nullable: false),
                        MachineDescription = c.String(),
                        CreateDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.MachineTwitterAccounts", "MachineId", c => c.Int(nullable: false));
            AlterColumn("dbo.MachineTwitterAccounts", "MachineName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MachineTwitterAccounts", "MachineName", c => c.String(nullable: false));
            DropColumn("dbo.MachineTwitterAccounts", "MachineId");
            DropTable("dbo.Machines");
        }
    }
}
