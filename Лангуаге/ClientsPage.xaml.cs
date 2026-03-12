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

namespace Лангуаге
{
    /// <summary>
    /// Логика взаимодействия для ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        public ClientsPage()
        {
            InitializeComponent();
            List<Gender> allGender = user16Entities4.GetContext().Gender.ToList();
            allGender.Insert(0, new Gender
            {
                Name = "Все типы"
            });
            FiltrComboBox.ItemsSource = allGender;
            FiltrComboBox.DisplayMemberPath = "Name";
            FiltrComboBox.SelectedValuePath = "Code";
            FiltrComboBox.SelectedIndex = 0;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditPage(MainDataGrid.SelectedItem as Client));
        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить данные?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    user16Entities4.GetContext().Client.Remove(MainDataGrid.SelectedItem as Client);
                    user16Entities4.GetContext().SaveChanges();
                    MessageBox.Show("Данные удалены");
                    MainDataGrid.ItemsSource = user16Entities4.GetContext().Client.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MainDataGrid.ItemsSource = user16Entities4.GetContext().Client.ToList();
        }

        private void FiltrComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }

        private void DownRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateClients();
        }

        private void UpRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateClients();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients();
        }

        private void UpdateClients()
        {
            List<Client> currentClients = user16Entities4.GetContext().Client.ToList();

            // Фильтрация по полу
            if (FiltrComboBox != null && FiltrComboBox.SelectedIndex > 0)
            {
                string selectedGender = Convert.ToString(FiltrComboBox.SelectedValue);
                currentClients = currentClients.Where(x => x.GenderCode == selectedGender).ToList();
            }

            // Сортировка
            if (SortComboBox != null)
            {
                switch (SortComboBox.SelectedIndex)
                {
                    case 1: // Сортировка по фамилии
                        {
                            if (UpRadioButton.IsChecked == true)
                                currentClients = currentClients.OrderBy(x => x.LastName).ToList();
                            if (DownRadioButton.IsChecked == true)
                                currentClients = currentClients.OrderByDescending(x => x.LastName).ToList();
                            break;
                        }
                    case 2: // Сортировка по дате регистрации
                        {
                            if (UpRadioButton.IsChecked == true)
                                currentClients = currentClients.OrderBy(x => x.RegistrationDate).ToList();
                            if (DownRadioButton.IsChecked == true)
                                currentClients = currentClients.OrderByDescending(x => x.RegistrationDate).ToList();
                            break;
                        }
                }
            }

            // Поиск
            if (SearchComboBox != null && SearchTextBox != null &&
                SearchComboBox.SelectedIndex >= 0 &&
                !string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string searchText = SearchTextBox.Text.ToLower();

                switch (SearchComboBox.SelectedIndex)
                {
                    case 0: // Поиск по имени (FirstName)
                        currentClients = currentClients
                            .Where(x => x.FirstName != null &&
                                   x.FirstName.ToLower().Contains(searchText))
                            .ToList();
                        break;

                    case 1: // Поиск по фамилии (LastName)
                        currentClients = currentClients
                            .Where(x => x.LastName != null &&
                                   x.LastName.ToLower().Contains(searchText))
                            .ToList();
                        break;

                    case 2: // Поиск по отчеству (Patronymic)
                        currentClients = currentClients
                            .Where(x => x.Patronymic != null &&
                                   x.Patronymic.ToLower().Contains(searchText))
                            .ToList();
                        break;

                    case 3: // Поиск по email
                        currentClients = currentClients
                            .Where(x => x.Email != null &&
                                   x.Email.ToLower().Contains(searchText))
                            .ToList();
                        break;

                    case 4: // Поиск по телефону
                        currentClients = currentClients
                            .Where(x => x.Phone != null &&
                                   x.Phone.ToLower().Contains(searchText))
                            .ToList();
                        break;
                }
            }

            MainDataGrid.ItemsSource = currentClients;

            if (currentClients.Count == 0)
            {
                MessageBox.Show("По вашему запросу ничего не найдено");
                if (SearchTextBox != null)
                    SearchTextBox.Text = "";
            }
        }
    }
}