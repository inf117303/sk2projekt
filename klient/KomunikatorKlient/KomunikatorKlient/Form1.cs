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
        private string userNumberText = "none";
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
                serverIP = sr.ReadLine();
            } catch {
                // exceptions
            } finally {
                if (sr != null) {
                    sr.Close();
                }
            }

            if (readUserStatus == "registered") {
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
                    textBox1.Text = "";
                    textBox1.Enabled = true;
                }

                if(!connectionActive) {
                    buttonPolacz.Enabled = true;
                    buttonRejestracja.Enabled = false;
                    buttonLogowanie.Enabled = false;
                    buttonRozmowy.Enabled = false;
                    labelConnectionStatus.Text = "nieaktywne";
                    labelConnectionStatus.ForeColor = System.Drawing.Color.Red;
                    textBox1.Text = "";
                    textBox1.Enabled = true;
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
            if(haslo.Length > 0) {
                sendDataToSocket("0", null, "registration_request", haslo);
                Console.WriteLine("Registration request has been sent.");
            } else {
                Console.WriteLine("Password not provided.");
                MessageBox.Show("Aby zarejestrować klienta, musisz nadać hasło.");
            }            
        }

        public void sendDataToSocket(string recipient, string sender, string type, string content) {
            Komunikat komunikat1 = new Komunikat(recipient, sender, type, content);
            string tempjson = JsonConvert.SerializeObject(komunikat1);
            byte[] byData = Encoding.UTF8.GetBytes(tempjson + "\r\n");
            socket1.Send(byData);            
            Console.WriteLine("Data has been sent to socket.");
            for (int i = 0; i < byData.Length; i++) {
                Console.Write(Convert.ToChar(byData[i]));
            }
        }

        private void startListeningFromSocket() {
            var socketThread = new Thread(() =>
            {
                bool connectionProblem = false;
                Console.WriteLine("Dedicated socket listening thread started!");
                while (true) {
                    Thread.Sleep(1000);
                    try {
                        byte[] buffer = new byte[1024];
                        int size = socket1.Receive(buffer);
                        if (size > 0) {
                            Console.WriteLine("Recieved data: ");
                            for (int i = 0; i < size; i++) {
                                Console.Write(Convert.ToChar(buffer[i]));
                            }
                            Console.WriteLine();
                            char[] chars = new char[size];
                            Decoder d = Encoding.UTF8.GetDecoder();
                            int charLen = d.GetChars(buffer, 0, size, chars, 0);
                            string recv = new String(chars);
                            Komunikat komunikatSerwera = JsonConvert.DeserializeObject<Komunikat>(recv);
                            //MessageBox.Show("DEBUG\nOdebrano komunikat serwera:\n" + recv);
                            if (komunikatSerwera.type == "registration_success") {
                                if(userRegistered == false) {
                                    userRegistered = true;
                                    userNumberText = komunikatSerwera.content;
                                    updateClientState();
                                    using (StreamWriter sw = File.CreateText("data.txt")) {
                                        sw.WriteLine("registered");
                                        sw.WriteLine(userNumberText);
                                        sw.WriteLine(serverIP);
                                        if (sw != null) {
                                            sw.Close();
                                        }
                                    }
                                    MessageBox.Show("Rejestracja udana.\nTwój nowy numer to: " + userNumberText);
                                } else {
                                    Console.WriteLine("registration_success type erroneous directed data skipped, client already registered");
                                }                                
                            } else if (komunikatSerwera.type == "registration_failed") {
                                if(userRegistered == false) {
                                    MessageBox.Show("Rejestracja nieudana. Szczegóły błędu:\n" + komunikatSerwera.content);
                                } else {
                                    Console.WriteLine("registration_failed type erroneous directed data skipped, client already registered");
                                }                                
                            } else if(komunikatSerwera.type == "login_success") {
                                if(komunikatSerwera.recipient == userNumberText) {
                                    setUserLoggedIn(true);
                                    updateClientState();
                                    MessageBox.Show("Logowanie udane.");
                                } else {
                                    Console.WriteLine("login_succes type erroneous directed data skipped, wrong recipient");
                                }                                
                            } else if (komunikatSerwera.type == "login_failed") {
                                if(komunikatSerwera.recipient == userNumberText) {
                                    setUserLoggedIn(false);
                                    updateClientState();
                                    MessageBox.Show("Logowanie nieudane. Upewnij się, że hasło jest wpisane poprawnie i spróbuj jeszcze raz.");
                                } else {
                                    Console.WriteLine("login_failed type erroneous directed data skipped, wrong recipient");
                                }                                
                            } else if (komunikatSerwera.type == "message") {
                                if(komunikatSerwera.recipient == userNumberText) {
                                    if (komunikatSerwera.sender == "0") {
                                        MessageBox.Show("Wiadomość serwera\n:" + komunikatSerwera.content);
                                    } else {
                                        openConversationsWindow();
                                        form3handle.parseIncomingMessage(komunikatSerwera.sender, komunikatSerwera.content);
                                    }
                                } else {
                                    Console.WriteLine("message type erroneous directed data skippe, wrong recipientd");
                                }                                                            
                            } else {
                                Console.WriteLine("unsupported data type message skipped");
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
            socketThread.IsBackground = true;
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
            using (StreamWriter sw = File.CreateText("data.txt")) {
                if(userRegistered) {
                    sw.WriteLine("registered");
                } else {
                    sw.WriteLine("unregistered");
                }                
                sw.WriteLine(userNumberText);
                sw.WriteLine(serverIP);
                if (sw != null) {
                    sw.Close();
                }
            }
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
            if(haslo.Length > 0) {
                sendDataToSocket("0", userNumberText, "login_request", haslo);
                Console.WriteLine("Login request has been sent.");
            } else {
                Console.WriteLine("Password not provided.");
                MessageBox.Show("Aby się zalogować, musisz podać hasło.");
            }            
        }
    }
}
