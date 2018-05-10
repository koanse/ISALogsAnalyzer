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
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
            Access.ReadSettings();
            textBox1.Text = Access.path;
            listBox1.Items.AddRange(Access.list.ToArray());
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Access.path = textBox1.Text;
            List<Template> list = new List<Template>();
            for (int i = 0; i < listBox1.Items.Count; i++)
                list.Add((Template)listBox1.Items[i]);
            Access.list = list;
            Access.SaveSettings();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
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
            listBox1.Items.Add(t);           
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;
            Template t = (Template)listBox1.SelectedItem;
            CustomForm cf = new CustomForm(t);
            if (cf.ShowDialog() != DialogResult.OK)
                return;
            List<string> l = new List<string>();
            foreach (string s in cf.clb.CheckedItems)
                l.Add(s);
            Template t2 = new Template() { name = cf.textBox1.Text, arrName = l };
            listBox1.SelectedItem = t2;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }
    }
}
