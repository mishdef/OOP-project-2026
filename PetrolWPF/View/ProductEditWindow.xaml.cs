using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using gsst.Model; 

namespace PetrolWPF.View 
{
    public partial class ProductEditWindow : Window
    {
        public Good CurrentProduct { get; private set; }

        private byte[] _imageBytes;

        public ProductEditWindow(Good product)
        {
            InitializeComponent();

            CurrentProduct = new Good();
            CurrentProduct.Id = product.Id;

            if (product.Id != 0)
            {
                CurrentProduct.Name = product.Name;
                CurrentProduct.Price = product.Price;
                CurrentProduct.BarCode = product.BarCode;
                _imageBytes = product.Image;

                NameTextBox.Text = CurrentProduct.Name;
                PriceTextBox.Text = CurrentProduct.Price.ToString();
                BarcodeTextBox.Text = CurrentProduct.BarCode;

                if (_imageBytes != null && _imageBytes.Length > 0)
                {
                    ProductImagePreview.Source = LoadImage(_imageBytes);
                }
            }

            Title = CurrentProduct.Id == 0 ? "Add New Product" : "Edit Product";
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    ProductImagePreview.Source = LoadImage(_imageBytes);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(BarcodeTextBox.Text))
                {
                    MessageBox.Show("Name and Barcode are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!double.TryParse(PriceTextBox.Text.Replace(".", ","), out double price) || price <= 0)
                {
                    MessageBox.Show("Please enter a valid positive price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                CurrentProduct.Name = NameTextBox.Text;
                CurrentProduct.Price = price;
                CurrentProduct.BarCode = BarcodeTextBox.Text;
                CurrentProduct.Image = _imageBytes ?? new byte[0];

                DialogResult = true;
                Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            _imageBytes = null;
            ProductImagePreview.Source = null;
        }
    }
}