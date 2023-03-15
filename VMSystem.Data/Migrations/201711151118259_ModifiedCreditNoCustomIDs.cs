namespace VMSystem.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedCreditNoCustomIDs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TerminalCashes", "CreditID", "dbo.Credits");
            DropPrimaryKey("dbo.Credits");
            AddColumn("dbo.Credits", "Denomination", c => c.Int(nullable: false));
            AlterColumn("dbo.Credits", "ID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Credits", "ID");
            AddForeignKey("dbo.TerminalCashes", "CreditID", "dbo.Credits", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TerminalCashes", "CreditID", "dbo.Credits");
            DropPrimaryKey("dbo.Credits");
            AlterColumn("dbo.Credits", "ID", c => c.Int(nullable: false));
            DropColumn("dbo.Credits", "Denomination");
            AddPrimaryKey("dbo.Credits", "ID");
            AddForeignKey("dbo.TerminalCashes", "CreditID", "dbo.Credits", "ID", cascadeDelete: true);
        }
    }
}
