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
using System.Windows.Shapes;

namespace RSI_Teach_Interface {
    /// <summary>
    /// Логика взаимодействия для NewItem.xaml
    /// </summary>
    public partial class NewItem : Window {
        public NewItem(MainWindow m) {
            InitializeComponent();
            this.m = m;
        }
        MainWindow m;

        private void bt_cancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void bt_add_Click(object sender, RoutedEventArgs e) {
            if (tb_name.Text == "") {
                MessageBox.Show($"Введите имя элемента!");
            } else if (cb_type.SelectedItem == null) {
                MessageBox.Show($"Выберите тип элемента!");
            } else if (tb_recive.Text == "" || tb_send.Text == "") {
                MessageBox.Show($"Введите данные о XML элемента!");
            }
            else if (m.AddRSIItem(tb_name.Text, cb_type.SelectedItem.ToString(), tb_recive.Text, tb_send.Text)) {
                Helper.WriteToFile(@"Data\recive.txt", tb_recive.Text);
                Helper.WriteToFile(@"Data\send.txt", tb_send.Text);
                Close();
            }          
        }
    }
}
