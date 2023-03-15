namespace VMSystem.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using VMSystem.Data.Model;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class Configuration : DbMigrationsConfiguration<VMSystem.Data.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(VMSystem.Data.DataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            //debugger for packages console
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            Assembly assembly = Assembly.GetExecutingAssembly();

            GetDataFromJson<Credit>("VMSystem.Data.Model.SeedData.credits.json", assembly, context, c => c.ID);
            GetDataFromJson<Product>("VMSystem.Data.Model.SeedData.products.json", assembly, context, p => p.ID);
            GetDataFromJson<Terminal>("VMSystem.Data.Model.SeedData.terminals.json", assembly, context, t => t.ID);
            GetDataFromJson<TerminalCash>("VMSystem.Data.Model.SeedData.terminalscash.json", assembly, context, tc => new { tc.TerminalID, tc.CreditID });
            GetDataFromJson<TerminalStock>("VMSystem.Data.Model.SeedData.terminalsstock.json", assembly, context, ts => new { ts.TerminalID, ts.ProductID });
            GetDataFromJson<TerminalStats>("VMSystem.Data.Model.SeedData.terminalsstats.json", assembly, context, tst => new { tst.PurchaseDate, tst.TerminalID, tst.ProductID });
            GetDataFromJson<ProductPrice>("VMSystem.Data.Model.SeedData.productprices.json", assembly, context, pp => new { pp.DateIntroduced, pp.ProductID });
        }

        private void GetDataFromJson<T>(string resourceName, Assembly assembly, DataContext context, Expression<Func<T, object>> KeyIdentifier) where T: class
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                {
                    var dataArray = JsonConvert.DeserializeObject<T[]>(reader.ReadToEnd());
                    context.Set<T>().AddOrUpdate(KeyIdentifier, dataArray);
                }
            }
        }


    }
}
