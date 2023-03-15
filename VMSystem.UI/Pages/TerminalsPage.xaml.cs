using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMSystem.Data;
using VMSystem.Data.Model;
using VMSystem.Data.QueryLogic;

namespace VMSystem.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для TerminalsPage.xaml
    /// </summary>
    public partial class TerminalsPage : Page
    {
        ObservableCollection<Terminal> _listViewContent;
        Terminal _selectedTerminal;

        public TerminalsPage()
        {
            InitializeComponent();
            StocksComboBox.ItemsSource = new TerminalStocksContainer().Stocks;
            StocksComboBox.DisplayMemberPath = "Description";
            StocksComboBox.SelectedValuePath = "Value";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadListView();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            LocationBox.Clear();
            EditGrid.IsEnabled = true;
            StocksComboBox.IsEnabled = true;
            SaveButton.Tag = 1;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (TerminalsListView.SelectedItem != null)
            {
                LocationBox.Clear();
                EditGrid.IsEnabled = true;
                StocksComboBox.IsEnabled = false;
                SaveButton.Tag = 2;
                _selectedTerminal = TerminalsListView.SelectedItem as Terminal;
                LocationBox.Text = _selectedTerminal.Location;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ConductUpdate((int)SaveButton.Tag);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadListView();
        }

        private void ConductUpdate(int operationId)
        {
            switch (operationId)
            {
                case 1:
                    try
                    {
                        if (string.IsNullOrEmpty(LocationBox.Text) || StocksComboBox.SelectedItem == null)
                            throw new MissingFieldException();

                        Terminal newTerminal = new Terminal { Location = LocationBox.Text };
                        using (var unitOfWork = new UnitOfWork())
                        {
                            unitOfWork.Terminals.StockOptionCallback += GetStockOption;
                            unitOfWork.Terminals.Add(newTerminal);
                            unitOfWork.Save();
                        }

                        _listViewContent.Add(newTerminal);
                        LocationBox.Clear();
                        SaveButton.Tag = null;
                        EditGrid.IsEnabled = false;
                    }
                    catch (MissingFieldException) { MessageBox.Show("Specify location and Stock option", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk); }
                    catch (NotImplementedException) { MessageBox.Show("Stock option is not available", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk); }
                    catch { MessageBox.Show("Failed to add terminal", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    break;

                case 2:
                    try
                    {
                        _selectedTerminal.Location = LocationBox.Text; //changes object in TerminalsListView as well

                        using (var unitOfWork = new UnitOfWork())
                        {
                            unitOfWork.Terminals.Edit(_selectedTerminal);
                            unitOfWork.Save();
                        }

                        TerminalsListView.Items.Refresh(); //Modifying obsevablecollection's entity doesn't update ListView here 
                        LocationBox.Clear();
                        SaveButton.Tag = null;
                        EditGrid.IsEnabled = false;
                        _selectedTerminal = null;
                    }
                    catch { MessageBox.Show("Failed to update terminal. Refresh to see if entity is available", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    break;

                default:
                    break;
            }
        }

        private void UpdateColumns(IEnumerable<string> headers, IEnumerable<string> bindings)
        {
            PagesContainer.UpdateColumnsHeadersBindings(ref TerminalsListView, headers, bindings);
        }

        private void LoadListView()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.Terminals.UpdateColumnsHandler += UpdateColumns;
                _listViewContent = new ObservableCollection<Terminal>(unitOfWork.Terminals.GetData());
                TerminalsListView.ItemsSource = _listViewContent;
            }
        }

        private int GetStockOption()
        {
            return StocksComboBox.SelectedValue != null ? (int)StocksComboBox.SelectedValue : 0;
        }
    }
}
