using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSystem.Data
{
    // Used to populate ComboBox on statistics page.
    public class ReportContainer
    {
        private IEnumerable<ComboBoxItemTemplate> _reports;

        public IEnumerable<ComboBoxItemTemplate> Reports { get { return _reports; } }

        public ReportContainer()
        {
            _reports = new List<ComboBoxItemTemplate>
            {
                new ComboBoxItemTemplate { Value = 1, Description = "Terminals with out of stock products"},
                new ComboBoxItemTemplate { Value = 2, Description = "5 least sold products (specify month)"},
                new ComboBoxItemTemplate { Value = 3, Description = "Terminals by profit made (specify month)"},
            };
        }
    }
}
