using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KomunikatorKlient
{
    public partial class Form2 : Form
    {
        private Form1 mainFormHandle;
        public Form2(Form1 mainForm)
        {
            InitializeComponent();
            mainFormHandle = mainForm;
            textBox1.Text = mainFormHandle.getServerIP();
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            string newServerIP = textBox1.Text;
            mainFormHandle.setServerIP(newServerIP);
            MessageBox.Show("Adres IP serwera został pomyślnie zmieniony na " + newServerIP);
            Close();
        }
    }
}
