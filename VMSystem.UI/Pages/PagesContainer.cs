using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace VMSystem.UI.Pages
{
    static class PagesContainer
    {
        private static StatsPage _statsPage = new StatsPage();
        private static TerminalsPage _terminalsPage = new TerminalsPage();
        private static ProductsPage _productsPage = new ProductsPage();

        public static StatsPage StatsPage { get { return _statsPage; } }
        public static TerminalsPage TerminalsPage { get { return _terminalsPage; } }
        public static ProductsPage ProductsPage { get { return _productsPage; } }

        public static void UpdateColumnsHeadersBindings(ref ListView listView, IEnumerable<string> headers, IEnumerable<string> bindings)
        {
            try
            {
                var gridView = new GridView();
                listView.View = gridView;
                var headerParameters = headers.Zip(bindings, (h, b) => new { Header = h, Binding = b });

                foreach (var p in headerParameters)
                    gridView.Columns.Add(new GridViewColumn { Width = p.Header.Length * 12 + 20, Header = p.Header, DisplayMemberBinding = new Binding(p.Binding) });
            }
            catch { throw new InvalidOperationException("Number of headers must match number of bindings"); }
        }
    }
}
