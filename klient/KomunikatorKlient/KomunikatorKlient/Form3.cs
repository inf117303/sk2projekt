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

        public Form3(Form1 mainForm)
        {
            InitializeComponent();
            mainFormHandle = mainForm;
            kontakty = new HashSet<string>();
            rozmowyDict = new Dictionary<string, List<string>>();
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
                    rozmowyDict.Add("10", rozmowa);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e) {
            string nowyNumer = textBox3.Text;
            if(kontakty.Contains(nowyNumer) == false) {
                comboBox1.Items.Add(nowyNumer);
                kontakty.Add(nowyNumer);
                MessageBox.Show("Pomyślnie dodano nowy kontakt.");
                using (StreamWriter sw = File.AppendText("contacts.txt")) {
                    sw.WriteLine(nowyNumer);
                }
            } else {
                MessageBox.Show("Taki numer już jest na Twojej liście kontaktów..");
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
        }
    }
}
