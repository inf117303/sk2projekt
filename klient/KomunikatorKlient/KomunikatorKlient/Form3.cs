using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KomunikatorKlient
{
    public partial class Form3 : Form
    {
        private Form1 mainFormHandle;
        private HashSet<string> kontakty;
        private Dictionary<string, List<string>> rozmowyDict;
        private List<string> rozmowa;
        private Dictionary<string, int> nieodczytane;
        private bool dropdownUsed = false;
        public Form3(Form1 mainForm)
        {
            InitializeComponent();
            mainFormHandle = mainForm;
            kontakty = new HashSet<string>();
            rozmowyDict = new Dictionary<string, List<string>>();
            nieodczytane = new Dictionary<string, int>();
            string plikHistoria;
            foreach (string line in File.ReadLines("contacts.txt")) {
                comboBox1.Items.Add(line);
                kontakty.Add(line);
                rozmowa = new List<string>();
                plikHistoria = "history_" + line + ".txt";
                if(File.Exists(plikHistoria)) {
                    foreach (string lineh in File.ReadLines(plikHistoria)) {
                        rozmowa.Add(lineh);
                    }
                }
                rozmowyDict.Add(line, rozmowa);
            }
            button1.Enabled = false;
            button3.Enabled = false;
            ActiveControl = textBox2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string selectedUserNumber = comboBox1.SelectedItem.ToString();
            string messageContent = "[ Ja ] " + textBox2.Text;
            textBox2.Text = "";
            textBox1.AppendText(messageContent);
            textBox1.AppendText(Environment.NewLine);
            string senderNumber = mainFormHandle.getUserNumber();
            mainFormHandle.sendDataToSocket(selectedUserNumber, senderNumber, "message", messageContent);
            List<string> biezacaRozmowa;
            if (rozmowyDict.TryGetValue(selectedUserNumber, out biezacaRozmowa)) {
                biezacaRozmowa.Add(messageContent);
                rozmowyDict[senderNumber] = biezacaRozmowa;
            }
            writeToHistoryFile(selectedUserNumber, messageContent);
            Console.WriteLine("Message passed to server.");
        }

        private void button2_Click(object sender, EventArgs e) {
            string nowyNumer = textBox3.Text;
            if(kontakty.Contains(nowyNumer) == false) {
                comboBox1.Items.Add(nowyNumer);
                kontakty.Add(nowyNumer);                
                using (StreamWriter sw = File.AppendText("contacts.txt")) {
                    sw.WriteLine(nowyNumer);
                }
                List<string> nowaRozmowa = new List<string>();
                string plikHistoria = "history_" + nowyNumer + ".txt";
                if (File.Exists(plikHistoria)) {
                    foreach (string lineh in File.ReadLines(plikHistoria)) {
                        nowaRozmowa.Add(lineh);
                    }
                }
                rozmowyDict.Add(nowyNumer, nowaRozmowa);
                MessageBox.Show("Pomyślnie dodano nowy kontakt.");
            } else {
                MessageBox.Show("Podany numer znajduje się już na Twojej liście kontaktów.");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            string currentUserNumber = comboBox1.SelectedItem.ToString();
            List<string> rozm = new List<string>();
            textBox1.Clear();
            if(rozmowyDict.TryGetValue(currentUserNumber, out rozm)) {
                foreach (string wiadom in rozm) {
                    textBox1.AppendText(wiadom);
                    textBox1.AppendText(Environment.NewLine);
                }                
            }
            int tempValue;
            if (nieodczytane.TryGetValue(currentUserNumber, out tempValue)) {
                nieodczytane[currentUserNumber] = 0;
                updateUnreadLabel();
            }
            button1.Enabled = true;
            button3.Enabled = true;
            dropdownUsed = true;
        }

        private void writeToHistoryFile(string userNumber, string message) {
            string historyFileName = "history_" + userNumber + ".txt";
            using (StreamWriter sw = File.AppendText(historyFileName)) {                
                sw.WriteLine(message);
            }
        }

        public void parseIncomingMessage(string senderNumber, string message) {
            MethodInvoker inv = delegate {
                string currentUserNumber;
                if (dropdownUsed) {
                    currentUserNumber = comboBox1.SelectedItem.ToString();
                } else {
                    currentUserNumber = "";
                }                
                int tempNieodczytane;
                message = "[ Użytkownik " + senderNumber + " ] " + message;
                if (senderNumber == currentUserNumber) {
                    textBox1.AppendText(message);
                    textBox1.AppendText(Environment.NewLine);
                } else {
                    if (nieodczytane.TryGetValue(senderNumber, out tempNieodczytane)) {
                        nieodczytane[senderNumber] = tempNieodczytane + 1;
                    } else {
                        nieodczytane.Add(senderNumber, 1);
                    }
                    updateUnreadLabel();
                }
                List<string> biezacaRozmowa;
                if (rozmowyDict.TryGetValue(senderNumber, out biezacaRozmowa)) {
                    biezacaRozmowa.Add(message);
                    rozmowyDict[senderNumber] = biezacaRozmowa;
                }
                writeToHistoryFile(senderNumber, message);
            };
            Invoke(inv);
        }

        private void button3_Click(object sender, EventArgs e) {
            string currentUserNumber = comboBox1.SelectedItem.ToString();
            comboBox1.Items.Remove(currentUserNumber);
            comboBox1.SelectedIndex = -1;
            button1.Enabled = false;
            button3.Enabled = false;
            dropdownUsed = false;
            textBox1.Text = "";
            textBox2.Text = "";
            kontakty.Remove(currentUserNumber);
            using (StreamWriter sw = File.CreateText("contacts.txt")) {
                foreach(string tempNumer in kontakty) {
                    sw.WriteLine(tempNumer);
                }                
            }
            Console.WriteLine("Contact removed from list.");
            MessageBox.Show("Pomyślnie usunięto kontakt z listy.");
        }

        private void updateUnreadLabel() {
            string tooltipText = "";
            int allUnread = 0;
            foreach (KeyValuePair<string, int> entry in nieodczytane) {
                if(entry.Value > 0) {
                    tooltipText = tooltipText + "(" + entry.Value.ToString() + ") wiadomości od użytkownika nr " + entry.Key + "\n";
                    allUnread += entry.Value;
                }                
            }
            if(allUnread > 0) {
                toolTip1.SetToolTip(label5, tooltipText);
                label5.Visible = true;
            } else {
                label5.Visible = false;
            }            
        }

    }
}
