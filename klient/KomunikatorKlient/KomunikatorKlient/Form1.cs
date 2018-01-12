using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace KomunikatorKlient
{
    public partial class Form1 : Form
    {
        private string userStatusText = "";
        private bool userRegistered;
        private bool userLoggedIn;
        private string userNumberText;
        private Socket socket1;
        private bool socketStatus;
        private string serverIP;

        public Form1()
        {
            InitializeComponent();
            string readUserStatus = File.ReadLines("data.txt").ToString();
            if(readUserStatus == "registered") {
                userStatusText = "zarejestrowany";
                userRegistered = true;
                button1.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = true;                
            } else {
                userStatusText = "niezarejestrowany";
                userRegistered = false;
                button1.Enabled = true;
                button2.Enabled = false;
                button3.Enabled = false;
            }
            string readUserNumber = File.ReadLines("data.txt").ToString();
            if(readUserNumber != "none" && readUserStatus == "registered") {
                userNumberText = readUserNumber;
            } else {
                userNumberText = "brak";
            }

            label3.Text = userStatusText;
            label4.Text = userNumberText;

            serverIP = "127.0.0.1"; // domyślny adres IP

            socketStatus = createNewSocket();
            if (socketStatus == true) {
                button5.Enabled = false;
                label6.Text = "aktywne";
            } else {
                label6.Text = "nieaktywne";
            }            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (socketStatus == true) {
                // wyślij żądanie rejestracji
                Komunikat rejestracjaZadanie = new Komunikat("0", null, "registration_request", "TrudneHaslo123");
                string tempjson = JsonConvert.SerializeObject(rejestracjaZadanie);
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(tempjson);
                socket1.Send(byData);
                // odczytaj odpowiedź serwera
                byte[] buffer = new byte[1024];
                int iRx = socket1.Receive(buffer);
                char[] chars = new char[iRx];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                string recv = new String(chars);
                Komunikat rejestracjaOdpowiedz = JsonConvert.DeserializeObject<Komunikat>(recv);
                if(rejestracjaOdpowiedz.type == "registration_success") {
                    userStatusText = "zarejestrowany";
                    label3.Text = userStatusText;
                    userNumberText = rejestracjaOdpowiedz.content;
                    label4.Text = userNumberText;
                    button1.Enabled = false;
                    button2.Enabled = true;
                    MessageBox.Show("Pomyślnie zarejestrowano klienta! Twój nowy numer to: " + userNumberText);
                }
            } else {
                MessageBox.Show("Brak połączenia z serwerem!\nNie można zarejestrować klienta.");
            }
        }

        private bool createNewSocket()
        {
            try {
                socket1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                int serverPort = 5501;
                System.Net.IPAddress ipAddr = System.Net.IPAddress.Parse(serverIP);
                System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAddr, serverPort);
                socket1.Connect(remoteEP);
            } catch (Exception e) {
                Console.WriteLine("Creating new socket failed!");
                Console.WriteLine("Exception: {0}", e);
                return false;
            }

            if (socket1.Connected == false) {
                Console.WriteLine("New socket could not be created!");
                return false;
            } else {                
                Console.WriteLine("New socket has been successfully created!");
                return true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bool foundForm = false;
            foreach (Form f in Application.OpenForms) {
                if (f is Form2)
                {
                    f.Focus();
                    foundForm = true;
                }
            }
            if(foundForm == false)
            {
                Form2 settingsForm = new Form2(this);
                settingsForm.Show();
            }            
        }

        public void setServerIP(string newServerIP)
        {
            serverIP = newServerIP;
        }

        public string getServerIP()
        {
            return serverIP;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            socketStatus = createNewSocket();
            if (socketStatus == true) {
                button5.Enabled = false;
                label6.Text = "aktywne";
                MessageBox.Show("Pomyślnie nawiązano z serwerem.");
            } else {
                label6.Text = "nieaktywne";
                MessageBox.Show("Nie udało nawiązać połączenia z serwerem.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool foundForm = false;
            foreach (Form f in Application.OpenForms) {
                if (f is Form3) {
                    f.Focus();
                    foundForm = true;
                }
            }
            if (foundForm == false) {
                Form3 childForm = new Form3(this);
                childForm.Show();
            }
        }
    }
}
