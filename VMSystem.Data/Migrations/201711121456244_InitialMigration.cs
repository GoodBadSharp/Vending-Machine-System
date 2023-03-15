namespace VMSystem.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Credits",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsCoin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TerminalCashes",
                c => new
                    {
                        TerminalID = c.Int(nullable: false),
                        CreditID = c.Int(nullable: false),
                        CreditQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TerminalID, t.CreditID })
                .ForeignKey("dbo.Credits", t => t.CreditID, cascadeDelete: true)
                .ForeignKey("dbo.Terminals", t => t.TerminalID, cascadeDelete: true)
                .Index(t => t.TerminalID)
                .Index(t => t.CreditID);
            
            CreateTable(
                "dbo.Terminals",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Location = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TerminalStats",
                c => new
                    {
                        TerminalID = c.Int(nullable: false),
                        ProductID = c.String(nullable: false, maxLength: 4),
                        PurchaseDate = c.DateTime(nullable: false),
                        QuantitySold = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TerminalID, t.ProductID, t.PurchaseDate })
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .ForeignKey("dbo.Terminals", t => t.TerminalID, cascadeDelete: true)
                .Index(t => t.TerminalID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 4),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ProductPrices",
                c => new
                    {
                        DateIntroduced = c.DateTime(nullable: false),
                        ProductID = c.String(nullable: false, maxLength: 4),
                        SellingPrice = c.Int(nullable: false),
                        PurchasePrice = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DateIntroduced, t.ProductID })
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.TerminalStocks",
                c => new
                    {
                        TerminalID = c.Int(nullable: false),
                        ProductID = c.String(nullable: false, maxLength: 4),
                        ProductQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TerminalID, t.ProductID })
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .ForeignKey("dbo.Terminals", t => t.TerminalID, cascadeDelete: true)
                .Index(t => t.TerminalID)
                .Index(t => t.ProductID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TerminalCashes", "TerminalID", "dbo.Terminals");
            DropForeignKey("dbo.TerminalStats", "TerminalID", "dbo.Terminals");
            DropForeignKey("dbo.TerminalStats", "ProductID", "dbo.Products");
            DropForeignKey("dbo.TerminalStocks", "TerminalID", "dbo.Terminals");
            DropForeignKey("dbo.TerminalStocks", "ProductID", "dbo.Products");
            DropForeignKey("dbo.ProductPrices", "ProductID", "dbo.Products");
            DropForeignKey("dbo.TerminalCashes", "CreditID", "dbo.Credits");
            DropIndex("dbo.TerminalStocks", new[] { "ProductID" });
            DropIndex("dbo.TerminalStocks", new[] { "TerminalID" });
            DropIndex("dbo.ProductPrices", new[] { "ProductID" });
            DropIndex("dbo.TerminalStats", new[] { "ProductID" });
            DropIndex("dbo.TerminalStats", new[] { "TerminalID" });
            DropIndex("dbo.TerminalCashes", new[] { "CreditID" });
            DropIndex("dbo.TerminalCashes", new[] { "TerminalID" });
            DropTable("dbo.TerminalStocks");
            DropTable("dbo.ProductPrices");
            DropTable("dbo.Products");
            DropTable("dbo.TerminalStats");
            DropTable("dbo.Terminals");
            DropTable("dbo.TerminalCashes");
            DropTable("dbo.Credits");
        }
    }
}
