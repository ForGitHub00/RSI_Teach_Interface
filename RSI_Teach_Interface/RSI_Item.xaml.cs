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

namespace RSI_Teach_Interface {
    /// <summary>
    /// Логика взаимодействия для RSI_Item.xaml
    /// </summary>
    /// 

    public class Data {
        public Data(string name, string type, string recivePath, string sendPath) {
            Name = name;
            Type = type;
            RecivePath = recivePath;
            SendPath = sendPath;
        }


        public string Name { get; set; }

        private string _type;
        public string Type { get { return _type; } set { _type = value; } }

        public string[] _recivePath;
        public string RecivePath {
            get {
                string str = "";
                foreach (string item in _recivePath) {
                    if (item!= "") {
                        str += item + @"\";
                    }                 
                }
                return str;
            }
            set {
                _recivePath = value.Split(new char[] { '\\' }).Where(x => x != "").ToArray();

            }
        }

        public string[] _sendPath;
        public string SendPath {
            get {
                string str = "";
                foreach (string item in _sendPath) {
                    str += item + @"\";
                }
                return str;
            }
            set {
                _sendPath = value.Split(new char[] { '\\' }).Where(x => x != "").ToArray();

            }
        }

        public string Value { get; set; }

    }

    public partial class RSI_Item : UserControl {
        public RSI_Item() {
            InitializeComponent();
            Random rnd = new Random();
            BorderBrush = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 255),
                (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255)));
            setInitData();

        }

        public Data data;
        public void ReWriteData() {
            tb_name.Text = data.Name;

            if (data.Type == "BOOL") {
                chB_bool.Visibility = Visibility.Visible;
                tb_value.Visibility = Visibility.Hidden;
                tb_newValue.Visibility = Visibility.Hidden;
            } else {
                chB_bool.Visibility = Visibility.Hidden;
                tb_value.Visibility = Visibility.Visible;
                tb_newValue.Visibility = Visibility.Visible;
            }

            string[] tempMas = Helper.ReadFromFile(@"Data\recive.txt");
            foreach (var item in tempMas) {
                cb_recive.Items.Add(item);
            }          
            cb_recive.SelectedIndex = cb_recive.Items.Count - 1;

            tempMas = Helper.ReadFromFile(@"Data\send.txt");
            foreach (var item in tempMas) {
                cb_send.Items.Add(item);
            }
            cb_send.SelectedIndex = cb_send.Items.Count - 1;

            cb_type.SelectedValue = data.Type;
            tb_value.Text = data.Value;
        }

        private void setInitData() {
            cb_type.Items.Add("BOOL");
            cb_type.Items.Add("INT");
            cb_type.Items.Add("REAL");
            cb_type.Items.Add("CHAR");
            cb_type.SelectedIndex = 0;
        }

        private void tb_Delete_Click(object sender, RoutedEventArgs e) {

        }
    }
}
