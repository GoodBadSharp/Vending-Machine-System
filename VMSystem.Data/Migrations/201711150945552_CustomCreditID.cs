namespace VMSystem.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomCreditID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TerminalCashes", "CreditID", "dbo.Credits");
            DropPrimaryKey("dbo.Credits");
            AlterColumn("dbo.Credits", "ID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Credits", "ID");
            AddForeignKey("dbo.TerminalCashes", "CreditID", "dbo.Credits", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TerminalCashes", "CreditID", "dbo.Credits");
            DropPrimaryKey("dbo.Credits");
            AlterColumn("dbo.Credits", "ID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Credits", "ID");
            AddForeignKey("dbo.TerminalCashes", "CreditID", "dbo.Credits", "ID", cascadeDelete: true);
        }
    }
}
