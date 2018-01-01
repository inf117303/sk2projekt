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

        public Form1()
        {
            InitializeComponent();
            string readUserStatus = File.ReadLines("data.txt").ToString();
            if(readUserStatus == "registered")
            {
                userStatusText = "zarejestrowany";
                userRegistered = true;
                button1.Enabled = false;
                button3.Enabled = true;                
            } else {
                userStatusText = "niezarejestrowany";
                userRegistered = false;
                button1.Enabled = true;
                button3.Enabled = false;
            }
            string readUserNumber = File.ReadLines("data.txt").ToString();
            if(readUserNumber != "none" && readUserStatus == "zarejestrowany")
            {
                userNumberText = readUserNumber;
            } else
            {
                userNumberText = "brak";
            }

            button2.Enabled = false;
            label3.Text = userStatusText;
            label4.Text = userNumberText;

            socketStatus = createNewSocket();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (socketStatus == true)
            {
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
                if(rejestracjaOdpowiedz.type == "registration_success")
                {
                    userStatusText = "zarejestrowany";
                    label3.Text = userStatusText;
                    userNumberText = rejestracjaOdpowiedz.content;
                    label4.Text = userNumberText;
                    button1.Enabled = false;
                    MessageBox.Show("Pomyślnie zarejestrowano klienta! Twój nowy numer to: " + userNumberText);
                }
            } else
            {
                MessageBox.Show("Nie udało się zarejestrować klienta! Brak połączenia z serwerem.");
            }
        }

        private bool createNewSocket()
        {
            bool resultVar;
            try
            {
                socket1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                string serverIp = "127.0.0.1";
                int serverPort = 5501;
                System.Net.IPAddress ipAddr = System.Net.IPAddress.Parse(serverIp);
                System.Net.IPEndPoint remoteEP = new IPEndPoint(ipAddr, serverPort);
                socket1.Connect(remoteEP);
            }
            catch (Exception e)
            {
                Console.WriteLine("Creating new socket failed!");
                Console.WriteLine("Exception: {0}", e);
                resultVar = false;
            }

            if (socket1.Connected == false)
            {
                resultVar = false;
            } else
            {
                resultVar = true;
                Console.WriteLine("New socket has been successfully created!");
            }

            return resultVar;
        }
    }
}
