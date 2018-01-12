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
    public partial class Form3 : Form
    {
        private Form1 mainFormHandle;
        public Form3(Form1 mainForm)
        {
            InitializeComponent();
            mainFormHandle = mainForm;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
