using KristaEmp.Data;
using Microsoft.Win32;
using System;
using System.Data.Entity.Migrations;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KristaEmp
{
    /// <summary>
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        private Employee _curEmp;
        private string _imgName;

        private bool _isUpdateEmp = false;
        public EmployeeWindow(Employee employee = null)
        {
            InitializeComponent();


            if (employee != null)
            {
                _isUpdateEmp = true;
                TitleLabel.Content = TitleLabel.Content.ToString().Replace("Добавление", "Изменение");

                _curEmp = employee;
                _imgName = employee.ImageName;
                EmpIdTextBox.Text = employee.Id.ToString();
                EmpNameTextBox.Text = employee.NameEmp;
                EmpImage.Source = new BitmapImage(new Uri(employee.ImgPath, UriKind.Absolute));
            }
            else
            {
                _curEmp = new Employee();
                EmpImage.Source = new BitmapImage(new Uri(_curEmp.ImgPath, UriKind.Absolute));
            }

        }

        private void SaveEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            _curEmp.NameEmp = EmpNameTextBox.Text.Trim();
            _curEmp.ImgPath = _imgName;

            Core.GetContext().Employee.AddOrUpdate(_curEmp);
            
            Core.GetContext().SaveChanges();
            Close();
        }

        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                if (!File.Exists($"{Employee.FULL_IMAGES_PATH}{ofd.SafeFileName}"))
                    File.Copy(ofd.FileName, $"{Employee.FULL_IMAGES_PATH}{ofd.SafeFileName}", false);
                _imgName = ofd.SafeFileName;

                EmpImage.Source = new BitmapImage(new Uri($"{Employee.FULL_IMAGES_PATH}{ofd.SafeFileName}", UriKind.Absolute));
            }
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            _imgName = null;

            EmpImage.Source = new BitmapImage(new Uri(Employee.FULL_IMAGES_PATH + _curEmp.defName, UriKind.Absolute));
        }
    }
}
