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

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string lang; // язык приложения (пока только на 2 языка рассчитано)
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            MinHeight = 100; // TODO
            MinWidth = 100;
            lang = "en-US"; // Unites States English язык по умолчанию
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            built_tree(); // строим TreeView

            int curYear = DateTime.Now.Year; // запишем текущий год
            //добавим в combobox список 200 годов для выбора
            for (int i = curYear-100; i < curYear+100; i++)
            {
                ComboboxYear.Items.Add(i); 
            }
            //установим выбранным по умолчанию текущий год
            ComboboxYear.SelectedItem = curYear;

            //добавляем языки в combobox
            ComboboxLanguage.Items.Add("en-US");
            ComboboxLanguage.Items.Add("ru-RU");
            //выбранный по умолчанию будет 0
            ComboboxLanguage.SelectedIndex = 0;
        }
        void built_tree(bool rebuild = false)
        {
            //создаю массив сезонов для дальнейшего добавление месяцев
            TreeViewItem[] seasons = { Winter, Spring, Summer, Autumn };

            string[] russ_seasons = { "Зима", "Весна", "Лето", "Осень" }; //ррусские названия сезонов
            for (int i = 0; i < seasons.Length; i++)
            {
                seasons[i].Header = lang == "ru-RU" ? russ_seasons[i] : seasons[i].Name; // пока так)
                if (rebuild)//в случае смены языка дерево нужно очистить
                    seasons[i].Items.Clear();
            }
         

            int counter = 0; // счетчик сезонов
            TreeViewItem month;
            for (int i = 0; i < 12; i++)
            {
                if (i > 0 && i % 3 == 0) // каждый третий месяц
                    counter++;           // осуществляется переход в следующий сезон 

                month = new TreeViewItem();
                month.Tag = i == 0 ? 12 : i; // месяца начинаются с зимы, поэтому первый месяц декабрь - 12
                month.Selected += TreeViewItem_Selected; // подписываем каждый месяц на обработчик клика (выбора)
                month.Header = new DateTime(1, i == 0 ? 12 : i, 1).ToString("MMMM",
                    System.Globalization.CultureInfo.CreateSpecificCulture(lang)); // название месяца
                seasons[counter].Items.Add(month);
            }

        }
        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (listViewField.Items.Count > 0)
                listViewField.Items.Clear();

            DateTime dayDate; // дата конкретного дня 
            ListViewItem item; // обьект добавляемый в listview
            Thickness thickness = new Thickness(); // граница отделяющая названия дней
            thickness.Bottom = 2;
            //выводим названия дней недели
            for (int i = 1; i <= 7; i++)
            {
                item = new ListViewItem();
                dayDate = new DateTime(1, 1, i);

                //граница
                item.BorderBrush = Brushes.Bisque;
                item.BorderThickness = thickness;

                //полное название дня при наведении
                item.ToolTip = dayDate.ToString("dddd",
                    System.Globalization.CultureInfo.CreateSpecificCulture(lang)); 

                //выходные дни цвет фона
                if (i >= 6)
                    item.Background = Brushes.Bisque;

                //текст ячейки день недели сокращенно
                item.Content = dayDate.ToString("ddd",
                    System.Globalization.CultureInfo.CreateSpecificCulture(lang));

                listViewField.Items.Add(item);
            }
            
            //выбранный месяц приводим к дате
            DateTime date = new DateTime((int)ComboboxYear.SelectedItem, (int)(sender as TreeViewItem).Tag, 1); 
            //добавляем пробелы если месяц не начинается с понедельника
            int dayofweek = (int)date.DayOfWeek;
            for (int i = 1; i < (dayofweek == 0 ? 7 : dayofweek); i++)
            {
                listViewField.Items.Add("");
            }

            //выводим дни
            for (int i = 1; i <= DateTime.DaysInMonth(date.Year,date.Month); i++)
            {
                item = new ListViewItem();
                dayDate = new DateTime(date.Year, date.Month, i);
                item.Content = $"{i}";
                item.ToolTip = dayDate.ToShortDateString(); //дата при наведении на ячейку дня
                if(dayDate.DayOfWeek == DayOfWeek.Saturday || dayDate.DayOfWeek == DayOfWeek.Sunday)
                    item.Foreground = Brushes.OrangeRed; //выходные другим цветом
                listViewField.Items.Add(item);
            }
        }

        private void ComboboxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lang = ComboboxLanguage.SelectedItem.ToString();
            listViewField.Items.Clear();
            built_tree(true);
            
        }

        private void ComboboxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listViewField.Items.Clear();
        }
    }
}
