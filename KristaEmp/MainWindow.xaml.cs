using KristaEmp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KristaEmp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void Sorts();
        private Sorts[] _sorts;

        private delegate void Filter();
        private Filter[] _filters;

        private List<Employee> _namesEmp;
        private List<Employee> CurPageContent
        {
            get => _namesEmp
                .Skip(_curPageIndex * PAGE_LEN)
                .Take(PAGE_LEN)
                .ToList();
        }


        private int _curPageIndex = 0;
        private const int PAGE_LEN = 2;
        public MainWindow()
        {
            InitializeComponent();

            _namesEmp = Core.GetContext().Employee.ToList();

            CreateFooter();
            EmpListView.ItemsSource = CurPageContent;

            CreateDelegateSorts();
            CreateDelegateFilters();
            SortComboBox.SelectedIndex = 0;
            FilterComboBox.SelectedIndex = 0;
        }

        private void CreateDelegateSorts()
        {
            _sorts = new Sorts[]
            {
                SortOne,
                SortTwo,
                SortThree
            };
        }

        private void CreateDelegateFilters()
        {
            _filters = new Filter[]
            {
                FilterOne,
                FilterTwo,
                FilterThree
            };
        }

        private void SortOne() => _namesEmp = Core.GetContext().Employee.ToList();
        private void SortTwo() => _namesEmp = Core.GetContext().Employee.ToList().OrderBy(i => i.NameEmp).ToList();
        private void SortThree() => _namesEmp = Core.GetContext().Employee.ToList().OrderByDescending(i => i.NameEmp).ToList();

        private void FilterOne() => _namesEmp = _namesEmp.ToList();
        private void FilterTwo() => _namesEmp = _namesEmp.Where(item => !item.ImgPath.Contains(item.defName)).ToList();
        private void FilterThree() => _namesEmp = _namesEmp.Where(item => item.ImgPath.Contains(item.defName)).ToList();

        private void CreateFooter()
        {
            _curPageIndex = 0;
            PaginatorStackPanel.Children.Clear();
            int pagesCount = _namesEmp.Count / PAGE_LEN;
            pagesCount += _namesEmp.Count % PAGE_LEN > 0 ? 1 : 0;

            PaginatorStackPanel.Children.Add(
                new Label
                {
                    Content = "Страницы:",
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(10, 0, 10, 10)
                }
            );
            for (int i = 0; pagesCount > i; i++)
            {
                var btn = new Button
                {
                    TabIndex = i,
                    Width = 10 * ((i / 10)) + 20,
                    Height = 30,
                    Content = i,
                    FontSize = 20,
                    Margin = new Thickness(5),
                    IsEnabled = i == 0 ? false : true
                };
                btn.Click += PageBtn_Click;

                PaginatorStackPanel.Children.Add(btn);
            }
        }

        private void PageBtn_Click(object sender, RoutedEventArgs e)
        {
            _curPageIndex = ((Button)sender).TabIndex;
            EmpListView.ItemsSource = CurPageContent;

            foreach (var child in PaginatorStackPanel.Children)
                if (child is Button && (child as Control).TabIndex == _curPageIndex)
                    ((Control)child).IsEnabled = false;
                else
                    ((Control)child).IsEnabled = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateContent();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateContent();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text.Equals("Поиск..."))
            {
                SearchTextBox.Foreground = Brushes.Black;
                SearchTextBox.Text = String.Empty;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Foreground = Brushes.LightGray;
                SearchTextBox.Text = "Поиск...";
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateContent();
        }

        private void UpdateContent()
        {
            if (SortComboBox.SelectedIndex >= 0 && FilterComboBox.SelectedIndex >= 0)
            {
                _sorts[SortComboBox.SelectedIndex].Invoke();
                _filters[FilterComboBox.SelectedIndex].Invoke();


                if (SearchTextBox?.Foreground == Brushes.Black)
                {
                    _namesEmp = _namesEmp.Where(item => item.NameEmp.ToLowerInvariant().Contains(SearchTextBox.Text.ToLowerInvariant())).ToList();
                }

                CreateFooter();
                _curPageIndex = 0;
                EmpListView.ItemsSource = CurPageContent;
            }
        }

        private void EmpWindow_Closed(object sender, EventArgs e)
        {
            Show();
            UpdateContent();
        }

        private void EmpAddButton_Click(object sender, RoutedEventArgs e)
        {
            var wind = new EmployeeWindow();
            wind.Closed += EmpWindow_Closed;
            wind.Show();
        }

        private void EmpDelButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmpListView.SelectedIndex >= 0)
            {
                Core.GetContext().Employee.Remove(Core.GetContext().Employee.ToList().First(item => item.Id == (EmpListView.SelectedItem as Employee).Id));
                Core.GetContext().SaveChanges();
            }

            UpdateContent();
        }

        private void EmpListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EmpListView.SelectedIndex >= 0)
            {
                var wind = new EmployeeWindow((Employee)EmpListView.SelectedItem);
                wind.Closed += EmpWindow_Closed;
                Hide();
                wind.Show();

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Core.GetContext().SaveChanges();
        }
    }

}