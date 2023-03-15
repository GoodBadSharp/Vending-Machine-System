using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using VMSystem.Data.Repositories;

namespace VMSystem.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        ObservableCollection<FullProduct> _listViewContent;
        FullProduct _selectedProduct;

        public ProductsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadListView();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            IDBox.Clear(); NameBox.Clear(); PurchasePriceBox.Clear(); SellingPriceBox.Clear();
            EditGrid.IsEnabled = true;
            PriceDatePicker.SelectedDate = DateTime.Today;
            IDBox.IsEnabled = true;
            PriceDatePicker.IsEnabled = false;
            SaveButton.Tag = 1;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {

            if (ProductsListView.SelectedItem != null)
            {
                IDBox.Clear(); NameBox.Clear(); PurchasePriceBox.Clear(); SellingPriceBox.Clear();
                EditGrid.IsEnabled = true;
                _selectedProduct = ProductsListView.SelectedItem as FullProduct;
                IDBox.Text = _selectedProduct.ID;
                PriceDatePicker.SelectedDate = DateTime.Today;
                NameBox.Text = _selectedProduct.Name;
                SellingPriceBox.Text = _selectedProduct.Price.ToString();
                PriceDatePicker.IsEnabled = true;
                PurchasePriceBox.Text = _selectedProduct.Cost.ToString();
                IDBox.IsEnabled = false;
                SaveButton.Tag = 2;

            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Deleting product will remove all related statistics and price change entries. Proceed?", "Warning", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    List<string> assignedProducts = new List<string>();
                    List<string> selectedItemsIds = new List<string>();

                    foreach (var item in ProductsListView.SelectedItems) //getting selected items' IDs because SelectedItems are dynamically modified during deleting
                    {
                        selectedItemsIds.Add(((FullProduct)item).ID);
                    }

                    foreach (var id in selectedItemsIds)
                    {
                        try
                        {
                            Product selectedProduct = new Product { ID = id };
                            using (var unitOfWork = new UnitOfWork())
                            {
                                unitOfWork.Products.Delete(selectedProduct);
                                unitOfWork.Save();
                            }
                            _listViewContent.Remove(_listViewContent.FirstOrDefault(fp => fp.ID == id));
                        }
                        catch (FieldAccessException exc) { assignedProducts.Add(exc.Message); }
                    }
                    MessageBox.Show($"Following products are assigned to terminal(s) and cannot be removed: {assignedProducts.Aggregate((s1, s2) => s1 + ", " + s2)}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                catch { MessageBox.Show("Failed to remove products", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
            }            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ConductUpdate((int)SaveButton.Tag);
        }

        private void ConductUpdate(int operationId)
        {
            switch (operationId)
            {
                case 1:
                    try
                    {
                        if (string.IsNullOrEmpty(NameBox.Text) || string.IsNullOrEmpty(IDBox.Text) || string.IsNullOrEmpty(SellingPriceBox.Text) || string.IsNullOrEmpty(PurchasePriceBox.Text))
                            throw new InvalidOperationException("Fill all fields");

                        Product newProduct = new Product();

                        try
                        {
                            newProduct.ID = IDBox.Text;
                            newProduct.Name = NameBox.Text;
                            //newProduct.ProductPrice.Add(new ProductPrice  // dunno it it works
                            //{
                            //    ProductID = newProduct.ID,
                            //    DateIntroduced = DateTime.Today,
                            //    PurchasePrice = int.Parse(PurchasePriceBox.Text),
                            //    SellingPrice = int.Parse(SellingPriceBox.Text)
                            //});
                            List<ProductPrice> productPrice = new List<ProductPrice>
                            { new ProductPrice
                                {
                                    ProductID = newProduct.ID,
                                    DateIntroduced = DateTime.Today,
                                    PurchasePrice = int.Parse(PurchasePriceBox.Text),
                                    SellingPrice = int.Parse(SellingPriceBox.Text)
                                }
                            };
                            newProduct.ProductPrice = productPrice;
                        }
                        catch { throw new InvalidOperationException("Fill all fields and check their conforminty to required formats"); }

                        using (var unitOfWork = new UnitOfWork())
                        {
                            unitOfWork.Products.Add(newProduct);
                            unitOfWork.Save();
                        }

                        _listViewContent.Add(new FullProduct
                        {
                            ID = newProduct.ID,
                            Name = newProduct.Name,
                            Price = int.Parse(SellingPriceBox.Text),
                            Cost = int.Parse(PurchasePriceBox.Text)
                        });
                        IDBox.Clear(); NameBox.Clear(); PurchasePriceBox.Clear(); SellingPriceBox.Clear();
                        SaveButton.Tag = null;
                        EditGrid.IsEnabled = false;
                    }
                    catch (InvalidOperationException ex) { MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Asterisk); }
                    catch { MessageBox.Show("Failed to add product", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    break;

                case 2:
                    //try
                    //{
                        if (string.IsNullOrEmpty(NameBox.Text) || string.IsNullOrEmpty(SellingPriceBox.Text) || string.IsNullOrEmpty(PurchasePriceBox.Text))
                            throw new InvalidOperationException("Fill all fields");

                        Product edittedProduct = new Product();
                        edittedProduct.ID = IDBox.Text;
                        edittedProduct.Name = NameBox.Text;

                        if (PriceDatePicker.SelectedDate != DateTime.Today)
                        {
                            if (PriceDatePicker.SelectedDate < DateTime.Today || PriceDatePicker.SelectedDate == null)
                            {
                                throw new InvalidOperationException("Specify correct date. Prices may not be introduced earlier than the next day");
                            }
                            else
                            {
                                try
                                {
                                    List<ProductPrice> productPrice = new List<ProductPrice>
                                    { new ProductPrice
                                        {
                                            ProductID = edittedProduct.ID,
                                            DateIntroduced = PriceDatePicker.SelectedDate.Value,
                                            PurchasePrice = int.Parse(PurchasePriceBox.Text),
                                            SellingPrice = int.Parse(SellingPriceBox.Text)
                                        }
                                    };
                                    edittedProduct.ProductPrice = productPrice;
                                }
                                catch { throw new InvalidOperationException("Fill all fields and check their conforminty to required formats"); }
                            }
                        }
                        else if(int.Parse(SellingPriceBox.Text) != _selectedProduct.Price || int.Parse(PurchasePriceBox.Text) != _selectedProduct.Cost)
                        {
                            throw new InvalidOperationException("Specify correct date. Prices may not be introduced earlier than the next day");
                        }

                        using (var unitOfWork = new UnitOfWork())
                        {
                            unitOfWork.Products.Edit(edittedProduct);
                            unitOfWork.Save();
                        }

                        _selectedProduct.Name = NameBox.Text;
                        _selectedProduct.Price = int.Parse(SellingPriceBox.Text);
                        _selectedProduct.Cost = int.Parse(PurchasePriceBox.Text);
                        ProductsListView.Items.Refresh(); //Modifying obsevablecollection's entity doesn't update ListView here 
                        IDBox.Clear(); NameBox.Clear(); PurchasePriceBox.Clear(); SellingPriceBox.Clear();
                        SaveButton.Tag = null;
                        EditGrid.IsEnabled = false;
                    //}
                    //catch (InvalidOperationException exc) { MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    //catch { MessageBox.Show("Failed to update product. Refresh to see if entity is available or correct input", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                    break;

                default:
                    break;
            }
        }

        private void UpdateColumns(IEnumerable<string> headers, IEnumerable<string> bindings)
        {
            PagesContainer.UpdateColumnsHeadersBindings(ref ProductsListView, headers, bindings);
        }

        private void LoadListView()
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    unitOfWork.Products.UpdateColumnsHandler += UpdateColumns;
                    _listViewContent = new ObservableCollection<FullProduct>(unitOfWork.Products.GetExtendedData());
                    ProductsListView.ItemsSource = _listViewContent;
                }
            }
            catch (InvalidOperationException exc){ { MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation); } }
            catch { MessageBox.Show("Failed to load products", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
        }

    }
}
