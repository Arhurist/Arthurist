using ServiceAuto.Models;
using ServiceAuto.Utils;
using ServiceAuto.Views.Windows;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace ServiceAuto.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        public ServicePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SortBox.SelectedIndex = 0;
            FilterBox.SelectedIndex = 0;
            GetServices(SortBox.Text);
        }
        private void GetServices(string sort = "По возрастанию", string filter = "Все", String search = "")
        {
            int allRec = 0; //Все записи
            int conRec = 0; //Какие-то записи, не успел :(

            List<Service> listService = AppData.db.Services.ToList();
            allRec = listService.Count;
            if (sort == "По возрастанию")
            {
                listService = listService.OrderBy(c => c.Price).ToList();
            }
            else
            {
                listService = listService.OrderByDescending(c => c.Price).ToList();
            }
            switch (filter)
            {
                case "От 0 до 5%":
                    listService = listService.Where(c => 0 <= c.Discount && c.Discount < 5).ToList();
                    break;
                case "От 5 до 15%":
                    listService = listService.Where(c => 5 <= c.Discount && c.Discount < 15).ToList();
                    break;
                case "От 15 до 30%":
                    listService = listService.Where(c => 15 <= c.Discount && c.Discount < 30).ToList();
                    break;
                case "От 30 до 70%":
                    listService = listService.Where(c => 30 <= c.Discount && c.Discount < 70).ToList();
                    break;
                case "От 70 до 100%":
                    listService = listService.Where(c => 70 <= c.Discount && c.Discount < 100).ToList();
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(search) && !String.IsNullOrWhiteSpace(search))
            {
                listService = listService.Where(c => c.Title.ToLower().Contains(search.ToLower()) || c.Description.ToLower().Contains(search.ToLower())).ToList();
            }
            ServiceGrid.ItemsSource = listService;
            conRec = listService.Count;
            Recording.Text = $"{conRec} из {allRec}";
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EditServiceWindow editServiceWindow = new EditServiceWindow();
            editServiceWindow.ShowDialog(); /*Добавляем открытие окна editServiceWindow*/
        }

        private void ShowRecButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow editServiceWindow = new AdminWindow();
            editServiceWindow.ShowDialog();
        }

        private void FilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetServices(SortBox.Text, ((ComboBoxItem)FilterBox.SelectedValue).Content.ToString(), SearchBox.Text);
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetServices(((ComboBoxItem)SortBox.SelectedValue).Content.ToString(), FilterBox.Text, SearchBox.Text);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetServices(SortBox.Text, FilterBox.Text, SearchBox.Text);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var SelectedItem = ServiceGrid.SelectedItem as Service; /*взяли выбранную строку в датагриде и присвоили сервайс, поскольку у нас там загружены наши услуги*/
                if (SelectedItem != null)
                {
                    EditServiceWindow editServiceWindow = new EditServiceWindow(SelectedItem);
                    editServiceWindow.ShowDialog();
                    GetServices(SortBox.Text, FilterBox.Text, SearchBox.Text);
                }
                else
                {
                    throw new Exception("Не выбрана запись");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var SelectedItem = ServiceGrid.SelectedItem as Service; /*взяли выбранную строку в датагриде и присвоили сервайс, поскольку у нас там загружены наши услуги*/
                if (SelectedItem != null)
                {
                    if (MessageBox.Show("Вы действительно хотите удалить запись?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var listPhotoes = AppData.db.ServicePhotoes.Where(c => c.ServiceID == SelectedItem.ID).ToList();
                        foreach (var item in listPhotoes)
                        {
                            if (File.Exists(item.PhotoPath.Trim()))
                            {
                                File.Delete(item.PhotoPath.Trim());
                            }
                        }
                        AppData.db.ServicePhotoes.RemoveRange(listPhotoes);
                        AppData.db.Services.Remove(SelectedItem);
                        AppData.db.SaveChanges();
                        GetServices(SortBox.Text, FilterBox.Text, SearchBox.Text);
                    }
                }
                else
                {
                    throw new Exception("Не выбрана запись");
                }
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2146233087)
                {
                    MessageBox.Show("На услугу есть записи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RecButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var SelectedItem = ServiceGrid.SelectedItem as Service; /*взяли выбранную строку в датагриде и присвоили сервайс, поскольку у нас там загружены наши услуги*/
                if (SelectedItem != null)
                {
                    AddClientWindow editServiceWindow = new AddClientWindow(SelectedItem);
                    editServiceWindow.ShowDialog();
                    GetServices(SortBox.Text, FilterBox.Text, SearchBox.Text);
                }
                else
                {
                    throw new Exception("Не выбрана запись");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
