using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VMSystem.Data;

namespace VMSystem.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для StatsPage.xaml
    /// </summary>
    public partial class StatsPage : Page
    {
        public StatsPage()
        {
            InitializeComponent();
            StatsComboBox.ItemsSource = new ReportContainer().Reports;
            StatsComboBox.DisplayMemberPath = "Description";
            StatsComboBox.SelectedValuePath = "Value";
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (StatsComboBox.SelectedValue != null)
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.StatsQueries.UpdateColumnsHandler += UpdateColumns;
                    unitOfWork.StatsQueries.TerminalReportHandler += FillTable;
                    unitOfWork.StatsQueries.ProductReportHandler += FillTable;
                    unitOfWork.StatsQueries.GetSpecifiedMonthCallback += GetSpecifiedMonth;
                    unitOfWork.StatsQueries.GetSpecifiedYearCallback += GetSpecifiedYear;

                    try { unitOfWork.StatsQueries.ConductQuery((int)StatsComboBox.SelectedValue); }
                    catch (NotImplementedException) { MessageBox.Show("Report is not implemented", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk); }
                    catch (NullReferenceException) { MessageBox.Show("Both month and year have to be specified", "Error", MessageBoxButton.OK, MessageBoxImage.Information); }
                    catch { MessageBox.Show("Failed to conduct report", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation); }

                }

        }

        public void UpdateColumns(IEnumerable<string> headers, IEnumerable<string> bindings)
        {
            PagesContainer.UpdateColumnsHeadersBindings(ref StatsListView, headers, bindings);
            StatsListView.ItemsSource = null;
        }

        public void FillTable<T>(IEnumerable<T> content)
        {
            var listViewContent = new ObservableCollection<T>(content);
            StatsListView.ItemsSource = listViewContent;
        }

        public int GetSpecifiedMonth()
        {
            return int.Parse(((ComboBoxItem)MonthComboBox.SelectedItem).Content.ToString());
        }

        public int GetSpecifiedYear()
        {
            return int.Parse(((ComboBoxItem)YearComboBox.SelectedItem).Content.ToString());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
