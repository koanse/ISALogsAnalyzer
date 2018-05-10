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
    public partial class CustomForm : Form
    {

        public CustomForm(Template t)
        {
            InitializeComponent();
            if (t == null)
                return;
            textBox1.Text = t.name;
            foreach (string s in t.arrName)
                for (int i = 0; i < clb.Items.Count; i++)
                    if ((string)clb.Items[i] == s)
                        clb.SetItemChecked(i, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
