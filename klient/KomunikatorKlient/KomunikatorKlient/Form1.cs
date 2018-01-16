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
using System.Threading;

namespace KomunikatorKlient
{
    public partial class Form1 : Form
    {
        private bool connectionActive = false;
        private bool userRegistered;
        private bool userLoggedIn;
        private string userNumberText;
        private Socket socket1;
        private bool socketStatus;
        private string serverIP;
        private Form3 form3handle = null;

        public Form1()
        {
            InitializeComponent();

            StreamReader sr = null;
            string readUserStatus = "";
            string readUserNumber = "";

            try {
                sr = new StreamReader("data.txt");
                readUserStatus = sr.ReadLine();
                readUserNumber = sr.ReadLine();
            } catch {
                // exceptions
            } finally {
                if (sr != null) {
                    sr.Close();
                }
            }

            if(readUserStatus == "registered") {
                userRegistered = true;
                
            } else {
                userRegistered = false;
                
            }

            if(readUserNumber != "none" && readUserStatus == "registered") {
                userNumberText = readUserNumber;
            } else {
                userNumberText = "brak";
            }

            labelUserNumber.Text = userNumberText;
            serverIP = "127.0.0.1"; // domyślny adres IP
            updateClientState();
        }

        public void setConnectionActive(bool state) {
            connectionActive = state;
        }

        public void setUserLoggedIn(bool state) {
            userLoggedIn = state;
        }

        public string getUserNumber() {
            return userNumberText;
        }

        private void updateClientState() {            
            if(InvokeRequired) {
                Console.WriteLine("updateClientState method invoked by thread");
                MethodInvoker inv = delegate {
                    labelUserNumber.Text = userNumberText;
                    if (userRegistered) {
                        buttonRejestracja.Enabled = false;
                        if (userLoggedIn) {
                            labelUserStatus.Text = "zarejestrowany, zalogowany";
                            buttonLogowanie.Enabled = false;
                            buttonRozmowy.Enabled = true;
                            textBox1.Text = "*****";
                            textBox1.Enabled = false;
                        } else {
                            labelUserStatus.Text = "zarejestrowany, niezalogowany";
                            buttonLogowanie.Enabled = true;
                            buttonRozmowy.Enabled = false;
                            textBox1.Text = "";
                            textBox1.Enabled = true;
                        }
                    } else {
                        buttonRejestracja.Enabled = true;
                        labelUserStatus.Text = "niezarejestrowany";
                    }

                    if (!connectionActive) {
                        buttonPolacz.Enabled = true;
                        buttonRejestracja.Enabled = false;
                        buttonLogowanie.Enabled = false;
                        buttonRozmowy.Enabled = false;
                        labelConnectionStatus.Text = "nieaktywne";
                        labelConnectionStatus.ForeColor = System.Drawing.Color.Red;
                    } else {
                        buttonPolacz.Enabled = false;
                        labelConnectionStatus.Text = "aktywne";
                        labelConnectionStatus.ForeColor = System.Drawing.Color.Green;
                    }
                };
                Invoke(inv);
            } else {
                Console.WriteLine("updateClientState method executed normally");

                labelUserNumber.Text = userNumberText;
                if (userRegistered) {
                    buttonRejestracja.Enabled = false;
                    if(userLoggedIn) {
                        labelUserStatus.Text = "zarejestrowany, zalogowany";
                        buttonLogowanie.Enabled = false;
                        buttonRozmowy.Enabled = true;
                        textBox1.Text = "*****";
                        textBox1.Enabled = false;
                    } else {
                        labelUserStatus.Text = "zarejestrowany, niezalogowany";
                        buttonLogowanie.Enabled = true;
                        buttonRozmowy.Enabled = false;
                        textBox1.Text = "";
                        textBox1.Enabled = true;
                    }
                } else {
                    buttonRejestracja.Enabled = true;
                    labelUserStatus.Text = "niezarejestrowany";
                }

                if(!connectionActive) {
                    buttonPolacz.Enabled = true;
                    buttonRejestracja.Enabled = false;
                    buttonLogowanie.Enabled = false;
                    buttonRozmowy.Enabled = false;
                    labelConnectionStatus.Text = "nieaktywne";
                    labelConnectionStatus.ForeColor = System.Drawing.Color.Red;
                } else {
                    buttonPolacz.Enabled = false;
                    labelConnectionStatus.Text = "aktywne";
                    labelConnectionStatus.ForeColor = System.Drawing.Color.Green;
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string haslo = textBox1.Text;
            sendDataToSocket("0", null, "registration_request", haslo);
            Console.WriteLine("Registration request has been sent.");
        }

        public void sendDataToSocket(string recipient, string sender, string type, string content) {
            Komunikat komunikat1 = new Komunikat(recipient, sender, type, content);
            string tempjson = JsonConvert.SerializeObject(komunikat1);
            byte[] byData = Encoding.UTF8.GetBytes(tempjson + "\r\n");
            socket1.Send(byData);
            Console.WriteLine("Data has been sent to socket.");
        }

        private void startListeningFromSocket() {
            var socketThread = new Thread(() =>
            {
                bool connectionProblem = false;
                Console.WriteLine("Dedicated socket listening thread started!");
                while (true) {
                    Thread.Sleep(1000);
                    try {
                        byte[] buffer = new byte[512];
                        int size = socket1.Receive(buffer);
                        if (size > 0) {
                            Console.WriteLine("Recieved data: ");
                            for (int i = 0; i < size; i++) {
                                Console.Write(Convert.ToChar(buffer[i]));
                            }
                            char[] chars = new char[size];
                            Decoder d = Encoding.UTF8.GetDecoder();
                            int charLen = d.GetChars(buffer, 0, size, chars, 0);
                            string recv = new String(chars);
                            Komunikat komunikatSerwera = JsonConvert.DeserializeObject<Komunikat>(recv);
                            if (komunikatSerwera.type == "registration_success") {
                                userRegistered = true;
                                userNumberText = komunikatSerwera.content;
                                updateClientState();
                                using (StreamWriter sw = File.CreateText("data.txt")) {
                                    sw.WriteLine("registered");
                                    sw.WriteLine(userNumberText);
                                }
                                MessageBox.Show("Pomyślnie zarejestrowano klienta! Twój nowy numer to: " + userNumberText);
                            } else if(komunikatSerwera.type == "login_success") {
                                setUserLoggedIn(true);
                                updateClientState();
                                MessageBox.Show("Logowanie udane.");
                            } else if (komunikatSerwera.type == "login_failed") {
                                setUserLoggedIn(false);
                                updateClientState();
                                MessageBox.Show("Logowanie nieudane. Upewnij się, że hasło jest wpisane poprawnie i spróbuj jeszcze raz.");
                            } else if (komunikatSerwera.type == "message") {
                                openConversationsWindow();
                                form3handle.parseIncomingMessage(komunikatSerwera.sender, komunikatSerwera.content);
                            }
                        } else {
                            throw new SocketException(System.Convert.ToInt32(SocketError.ConnectionReset));
                        }
                    } catch (SocketException e1) {
                        Console.WriteLine("Connection problem.");
                        Console.WriteLine("Exception details: {0}", e1);
                        connectionProblem = true;
                    } catch (JsonReaderException e2) {
                        Console.WriteLine("JSON parse error.");
                        Console.WriteLine("Exception details: {0}", e2);
                    } catch (JsonSerializationException e3) {
                        Console.WriteLine("JSON parse error.");
                        Console.WriteLine("Exception details: {0}", e3);
                    }
                    if (connectionProblem == true) {
                        setConnectionActive(false);
                        updateClientState();
                        MessageBox.Show("Utracono połączenie z serwerem.");
                        break;
                    }
                }
                socket1.Close();
                Console.WriteLine("Thread stopped.");
            });
            socketThread.Start();
        }

        private bool createNewSocket()
        {
            try {
                socket1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                int serverPort = 5501;
                IPAddress ipAddr = IPAddress.Parse(serverIP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddr, serverPort);
                socket1.Connect(remoteEP);
            } catch (Exception e) {
                Console.WriteLine("Creating new socket failed!");
                Console.WriteLine("Exception: {0}", e);
                socket1.Close();
                return false;
            }

            if (socket1.Connected == false) {
                socket1.Close();
                Console.WriteLine("Socket appears to be created but is not responding!");
                return false;
            } else {                
                Console.WriteLine("New socket has been successfully created!");
                connectionActive = true;
                updateClientState();
                return true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openSettingssWindow();   
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
            buttonPolacz.Text = "Łączenie, czekaj...";
            socketStatus = createNewSocket();
            if (socketStatus == true) {
                buttonPolacz.Enabled = false;
                labelConnectionStatus.Text = "aktywne";
                startListeningFromSocket();
                MessageBox.Show("Pomyślnie nawiązano z serwerem.");
            } else {
                labelConnectionStatus.Text = "nieaktywne";
                MessageBox.Show("Nie udało nawiązać połączenia z serwerem.");
            }
            buttonPolacz.Text = "Połącz";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openConversationsWindow();
        }

        private void openConversationsWindow() {
            MethodInvoker inv = delegate {
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
                    form3handle = childForm;
                }
            };
            Invoke(inv);
        }

        private void openSettingssWindow() {
            MethodInvoker inv = delegate {
                bool foundForm = false;
                foreach (Form f in Application.OpenForms) {
                    if (f is Form2) {
                        f.Focus();
                        foundForm = true;
                    }
                }
                if (foundForm == false) {
                    Form2 settingsForm = new Form2(this);
                    settingsForm.Show();
                }
            };
            Invoke(inv);
        }

        private void buttonLogowanie_Click(object sender, EventArgs e) {
            string haslo = textBox1.Text;
            sendDataToSocket("0", userNumberText, "login_request", haslo);
            Console.WriteLine("Login request has been sent.");
        }
    }
}
