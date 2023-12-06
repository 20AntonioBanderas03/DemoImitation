using KristaEmp.Data;
using Microsoft.Win32;
using System;
using System.Data.Entity.Migrations;
using System.IO;
using System.Windows;
using System.Windows.Controls;
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
                EmpSalaryTextBox.Text = employee.Salary.ToString();

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

            float tempSalary;
            

            Core.GetContext().Employee.AddOrUpdate(_curEmp);

            try 
            {
                Core.GetContext().SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка...\n{ex.Message}","Ошибка!!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            Close();
        }

        private bool GetValueOrDefaultFromTextBox(float? _default, TextBox tb)
        {
            float? salary = null;

            try
            {
                if(string.IsNullOrWhiteSpace(tb.Text))

                salary = float.Parse(tb.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"При сохранении поля 'Salary' произошла ошибка:\n{ex.Message}\nДанные не сохранены...");
                return false;
            }

            return true;
        }

        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = "ИЗОБРАЖЕНИЯ | *.png; *.bmp"};//{ Filter = "ИЗОБРАЖЕНИЯ | *.png; *.jpg"};

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
