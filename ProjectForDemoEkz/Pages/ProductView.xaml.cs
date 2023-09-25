using System;
using System.Collections.Generic;
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
using ProjectForDemoEkz.Models;

namespace ProjectForDemoEkz.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductView.xaml
    /// </summary>
    public partial class ProductView : Page
    {
        private int PagesCount;
        private int NumberOfPage = 0;
        private int maxItemShow = 20;
        List<Product> products = new List<Product>();
        public ProductView()
        {
            InitializeComponent();

            var productTypes = App.Context.ProductType.ToList();
            productTypes.Insert(0, new ProductType
            {
                Title = "Без сортировки"
            });
            cboxOrdByProductType.ItemsSource = productTypes;

            cboxSortBy.SelectedIndex = 0;

            UpdateProduct();
            UpdateComboBoxes();
        }

        // Updating GridView on Events
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
            UpdateComboBoxes();
        }
        private void CBoxSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }
        private void CBoxOrdByProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
            UpdateComboBoxes();
        }
        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProduct();
            UpdateComboBoxes();
        }

        // Add + Edit + Delete buttons controls
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProductAddEdit(null));
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var elemsToDelete = LViewProduct.SelectedItems.Cast<Product>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить следующие {elemsToDelete.Count()} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    App.Context.Product.RemoveRange(elemsToDelete);
                    App.Context.SaveChanges();
                    MessageBox.Show("Данные удалены!");
                    UpdateProduct();
                    UpdateComboBoxes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        // Edit by double click on record
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ProductAddEdit((sender as ListViewItem).Content as Product));
        }

        // Function of GridView update + Sorting
        private void UpdateProduct()
        {
            var product = App.Context.Product.ToList();
            switch (cboxSortBy.SelectedIndex)
            {
                case 1:
                    product = product.OrderBy(p => p.Title).ToList();
                    break;
                case 2:
                    product = product.OrderByDescending(p => p.Title).ToList();
                    break;
                case 3:
                    product = product.OrderBy(p => p.ProductionWorkshopNumber).ToList();
                    break;
                case 4:
                    product = product.OrderByDescending(p => p.ProductionWorkshopNumber).ToList();
                    break;
                case 5:
                    product = product.OrderBy(p => p.MinCostForAgent).ToList();
                    break;
                case 6:
                    product = product.OrderByDescending(p => p.MinCostForAgent).ToList();
                    break;
                default:
                    product = product.OrderBy(p => p.ID).ToList();
                    break;
            }
            if (cboxOrdByProductType.SelectedIndex != 0)
            {
                product = product.Where(p => p.ProductType == cboxOrdByProductType.SelectedValue).ToList();
            }
            product = product.Where(p => p.Title.ToLower().Contains(tbSearch.Text.ToLower()) ||
            p.ProductType.Title.ToLower().Contains(tbSearch.Text.ToLower()) ||
            p.ArticleNumber.ToLower().Contains(tbSearch.Text.ToLower()) ||
            p.MinCostForAgent.ToString().ToLower().Contains(tbSearch.Text.ToLower())
            ).ToList();

            tbkItemCounter.Text = product.Count.ToString() + " из " + App.Context.Product.Count().ToString();

            if (product.Count % maxItemShow == 0)
            {
                PagesCount = product.Count / maxItemShow;
            }
            else
            {
                PagesCount = (product.Count / maxItemShow) + 1;
            }

            LViewProduct.ItemsSource = product.Skip(maxItemShow * NumberOfPage).Take(maxItemShow).ToList();
            CheckPages();
            if (product.Count < 1)
                tbkItemCounter.Text += "\nПо вашему запросу ничего не найдено. Измените фильтры.";
        }

        // Paging controls buttons
        private void BtnPagePrev_Click(object sender, RoutedEventArgs e)
        {
            NumberOfPage--;
            cboxCurrentPageSelection.Text = (NumberOfPage + 1).ToString();
        }
        private void BtnPageNext_Click(object sender, RoutedEventArgs e)
        {
            NumberOfPage++;
            cboxCurrentPageSelection.Text = (NumberOfPage + 1).ToString();
        }
        private void CBoxCurrentPageSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NumberOfPage = cboxCurrentPageSelection.SelectedIndex;
            UpdateProduct();
        }

        // Turning ON/OFF paging controls
        private void CheckPages()
        {
            if (NumberOfPage > 0)
            {
                btnPagePrev.Visibility = Visibility.Visible;
            }
            else
            {
                btnPagePrev.Visibility = Visibility.Hidden;
            }
            if (NumberOfPage < PagesCount - 1)
            {
                btnPageNext.Visibility = Visibility.Visible;
            }
            else
            {
                btnPageNext.Visibility = Visibility.Hidden;
            }
        }
        private void UpdateComboBoxes()
        {
            cboxCurrentPageSelection.Items.Clear();
            for (int i = 1; i <= PagesCount; i++)
            {
                cboxCurrentPageSelection.Items.Add(i.ToString());
            }
            cboxCurrentPageSelection.SelectedIndex = 0;
        }
    }
}
