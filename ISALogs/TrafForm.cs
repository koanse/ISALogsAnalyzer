using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ISALogs
{
    public partial class TrafForm : Form
    {
        public double min = 0, max = 1000000000;
        public TrafForm()
        {
            InitializeComponent();
            textBox1.Text = "0";
            textBox2.Text = "1000000000";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                min = double.Parse(textBox1.Text);
                max = double.Parse(textBox2.Text);
                Close();
            }
            catch
            {
                MessageBox.Show("Ошибка ввода");
                min = 0;
                max = 1000000000;
            }            
        }
    }
}
