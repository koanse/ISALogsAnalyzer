using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ISALogs
{
    public partial class TemplateForm : Form
    {
        public Template template = null;
        public TemplateForm()
        {
            InitializeComponent();
            Access.ReadSettings();
            foreach (Template t in Access.list)
                listBox1.Items.Add(t);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            CustomForm cf = new CustomForm(null);
            if (cf.ShowDialog() != DialogResult.OK)
                return;
            List<string> l = new List<string>();
            foreach (string s in cf.clb.CheckedItems)
                l.Add(s);
            Template t = new Template() { name = cf.textBox1.Text, arrName = l };
            Access.list.Add(t);
            Access.SaveSettings();            
            template = t;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
                template = (Template)listBox1.SelectedItem;
            DialogResult = DialogResult.OK;
            Close();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
