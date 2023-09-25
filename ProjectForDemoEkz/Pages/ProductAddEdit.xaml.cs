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

using System.Text.RegularExpressions;
using ProjectForDemoEkz.Models;
using Microsoft.Win32;
using System.IO;

namespace ProjectForDemoEkz.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductAddEdit.xaml
    /// </summary>
    public partial class ProductAddEdit : Page
    {
        string pathToImage = string.Empty;
        string pathToImageShort = string.Empty;
        string directory = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"), @"Resources")); // Directory
        BitmapImage imagePreviewBit = new BitmapImage();
        private Product currentElem = new Product();
        public ProductAddEdit(Product elemData)
        {
            MessageBox.Show(directory);
            InitializeComponent();

            if (elemData != null)
            {
                Title = "Продукты. Редактирование";
                currentElem = elemData;
            }
            DataContext = currentElem;

            cboxProductType.ItemsSource = App.Context.ProductType.ToList();

            if (currentElem.Image != null && currentElem.Image != "")
            {
                pathToImage = directory + currentElem.Image;
                MessageBox.Show(pathToImage);
                pathToImageShort = currentElem.Image;
                ImagePreview.Source = new BitmapImage(new Uri(pathToImage, UriKind.Absolute));
            }
            else
            {
                pathToImage = System.IO.Path.Combine(directory, @"products\picture.png");
                MessageBox.Show(pathToImage);
                ImagePreview.Source = new BitmapImage(new Uri(pathToImage, UriKind.Absolute));
            }
        }
        // Regexes
        private void TextValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[а-яА-ЯёЁa-zA-Z0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]");
            e.Handled = !regex.IsMatch(e.Text);
        }
        private void ArticleValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]");
            e.Handled = !regex.IsMatch(e.Text);
        }
        private void DoubleValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^(0|([1-9][0-9]*))(\\.[0-9]+)?$");
            e.Handled = !regex.IsMatch(e.Text);
        }
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Image | *.png; *.jpg; *.jpeg" };
            if (ofd.ShowDialog() == true)
            {
                string filename = ofd.SafeFileName;
                string filepath = ofd.FileName;
                pathToImage = directory + @"\products\" + filename;
                pathToImageShort = @"\products\" + filename;
                File.Copy(filepath, pathToImage, true);
                ImagePreview.Source = new BitmapImage(new Uri(pathToImage, UriKind.Absolute));
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            currentElem.Image = pathToImageShort;
            // Check if textboxes is filled
            StringBuilder err = new StringBuilder();
            if (string.IsNullOrWhiteSpace(currentElem.Title))
                err.AppendLine("Укажите название");
            if (cboxProductType.SelectedItem == null)
                err.AppendLine("Укажите тип продукта");
            if (string.IsNullOrWhiteSpace(currentElem.ArticleNumber) || currentElem.ArticleNumber.Length < 6)
                err.AppendLine("Укажите артикул (не менее 6 символов)");
            if (currentElem.ProductionPersonCount == 0 || currentElem.ProductionPersonCount.ToString() == " ")
                err.AppendLine("Укажите количество персонала");
            if (currentElem.ProductionWorkshopNumber == 0 || currentElem.ProductionPersonCount.ToString() == " ")
                err.AppendLine("Укажите номер мастерской");
            if (currentElem.MinCostForAgent == 0 || currentElem.ProductionPersonCount.ToString() == "")
                err.AppendLine("Укажите минимальную стоимость");

            if (err.Length > 0)
            {
                MessageBox.Show(err.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            currentElem.Title = currentElem.Title.Trim();
            currentElem.Title = Regex.Replace(currentElem.Title, @"\s+", " ");
            if (currentElem.Description != null)
            {
                currentElem.Description = currentElem.Description.Trim();
                currentElem.Description = Regex.Replace(currentElem.Description, @"\s+", " ");
            }

            if (currentElem.ID == 0)
            {
                App.Context.Product.Add(currentElem);
            }

            try
            {
                App.Context.SaveChanges();
                MessageBox.Show("Данные сохранены");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
