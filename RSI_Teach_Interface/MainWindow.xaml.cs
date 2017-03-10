using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Items = new Dictionary<string, RSI_Item>();
        }

        Dictionary<string, RSI_Item> Items;
        bool exit = false;
        bool weld = false;
        bool move = false;
        double moveDist = 100;

        private int port;
        private void UDP() {
            System.Xml.XmlDocument SendXML = new System.Xml.XmlDocument();  // XmlDocument pattern
            SendXML.PreserveWhitespace = true;
            SendXML.Load("ExternalData.xml");

            UdpClient serveur = new UdpClient(port);
            Random rnd = new Random();

            try {
                while (true) {
                    IPEndPoint client = null;
                    byte[] data = serveur.Receive(ref client);
                    //Console.WriteLine("Donnees recues en provenance de {0}:{1}.", client.Address, client.Port);
                    string message = Encoding.ASCII.GetString(data);
                    string strReceive = message;

                    if ((strReceive.LastIndexOf("</Rob>")) == -1) {
                        continue;
                    } else {

                        string strSend;
                        strSend = SendXML.InnerXml;
                        strSend = mirrorIPOC(strReceive, strSend);
                        //
                        strSend = ReWriteStrSend(strReceive, strSend);
                        //
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(strSend);
                        serveur.Send(msg, msg.Length, client);
                    }
                    strReceive = null;
                }
            } catch (Exception ex) {
                Console.WriteLine("Возникло исключение: " + ex.ToString() + "\n  " + ex.Message);
            }
        }
        private string ReWriteStrSend(string strRecive, string strSend) {
            foreach (RSI_Item item in Items.Values) {
                try {
                    Dispatcher.Invoke(() => {
                        item.data.Value = Helper.GetValues(strRecive, item.data._recivePath).ToString();
                        item.ReWriteData();
                    });
                   
                } catch (Exception e) {
                    MessageBox.Show(e.Message);            
                }
            }

            if (weld) {
                //strSend = Helper.SetValue(strSend, new string[] { "Sen", "DX" }, 430.75);
                //strSend = Helper.SetValue(strSend, new string[] { "Sen", "DY" }, 619.68);
                //strSend = Helper.SetValue(strSend, new string[] { "Sen", "DZ" }, 14.49);
                //strSend = Helper.SetValue(strSend, new string[] { "Sen", "DA" }, -6.04);
                //strSend = Helper.SetValue(strSend, new string[] { "Sen", "DB" }, -0.09);
                //strSend = Helper.SetValue(strSend, new string[] { "Sen", "DC" }, 5.29);

                strSend = Helper.SetValue(strSend, new string[] { "Sen", "DX" }, 348.26);
                strSend = Helper.SetValue(strSend, new string[] { "Sen", "DY" }, 650.59);
                strSend = Helper.SetValue(strSend, new string[] { "Sen", "DZ" }, 68.49);
                strSend = Helper.SetValue(strSend, new string[] { "Sen", "DA" }, -55.67);
                strSend = Helper.SetValue(strSend, new string[] { "Sen", "DB" }, 87.63);
                strSend = Helper.SetValue(strSend, new string[] { "Sen", "DC" }, -63.61);
            }

            if (move) {
                if (moveDist > 0) {
                    strSend = Helper.SetValue(strSend, new string[] { "Sen", "RKorr", "X" }, 0.05);
                    moveDist -= 0.05;
                } else {
                    strSend = Helper.SetValue(strSend, new string[] { "Sen", "Exit" }, 1);
                }
            }


            if (exit) {
                strSend = Helper.SetValue(strSend, new string[] { "Sen", "Exit" }, 1);
                strSend = Helper.SetValue(strSend, new string[] { "Sen", "RKorr", "X" }, 0);
            }


            return strSend;
        }
        private string mirrorIPOC(string receive, string send) {
            // separate IPO counter as string
            int startdummy = receive.IndexOf("<IPOC>") + 6;
            int stopdummy = receive.IndexOf("</IPOC>");
            string Ipocount = receive.Substring(startdummy, stopdummy - startdummy);

            // find the insert position		
            startdummy = send.IndexOf("<IPOC>") + 6;
            stopdummy = send.IndexOf("</IPOC>");

            // remove the old value an insert the actualy value
            send = send.Remove(startdummy, stopdummy - startdummy);
            send = send.Insert(startdummy, Ipocount);

            Dispatcher.Invoke(() => { lab_ipoc.Content = Ipocount; });
           // lab_ipoc.Content = Ipocount;


            return send;
        }

        private void bt_addRsiItem_Click(object sender, RoutedEventArgs e) {
            NewItem i = new NewItem(this);
            i.Show();
        }

        public void AddRSIItem() {
            lb_RSI_Items.Items.Add(new RSI_Item());
            foreach (RSI_Item item in lb_RSI_Items.Items) {
                item.tb_Delete.Click += Tb_Delete_Click;
            }
        }
        public bool AddRSIItem(string name, string type, string recivePath, string sendPath) {          
            RSI_Item tempItem = new RSI_Item();
            tempItem.data = new Data(name, type, recivePath, sendPath);
            tempItem.ReWriteData();
            try {
                Items.Add(name, tempItem);
                lb_RSI_Items.Items.Add(tempItem);
            } catch (Exception) {
                MessageBox.Show($"Объект с именем {name} уже существует!");
                return false;
            } 
            
            foreach (RSI_Item item in lb_RSI_Items.Items) {
                item.tb_Delete.Click += Tb_Delete_Click;
            }

            return true;

        }
        private void Tb_Delete_Click(object sender, RoutedEventArgs e) {
            lb_RSI_Items.Items.Remove((((sender as Button).Parent) as Grid).Parent);
        }

        private void bt_startListen_Click(object sender, RoutedEventArgs e) {
            port = Convert.ToInt32(tb_port.Text);
            Thread thrd = new Thread(new ThreadStart(UDP));
            thrd.Start();
            bt_startListen.IsEnabled = false;
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            //System.Xml.XmlDocument SendXML = new System.Xml.XmlDocument();  // XmlDocument pattern
            //SendXML.PreserveWhitespace = true;
            //SendXML.Load("ExternalData.xml");
            //string strSend;
            //strSend = SendXML.InnerXml;
            //strSend = Helper.SetValue(strSend, new string[] { "Sen", "RKorr", "X" }, 1555.5);
            //strSend = Helper.SetValue(strSend, new string[] { "Sen", "DX" }, 1555);
            //Console.WriteLine(strSend);
            exit = true;
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            weld = true;
        }

        private void button2_Click(object sender, RoutedEventArgs e) {
            move = !move;
        }
    }
}
