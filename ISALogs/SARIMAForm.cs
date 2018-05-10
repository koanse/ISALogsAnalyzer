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
    public partial class SARIMAForm : Form
    {
        public SARIMAForm()
        {
            InitializeComponent();
            tbM.Text = "10";
            tbAR.Text = "2";
            tbMA.Text = "2";
            tbProg.Text = "3";
            tbD.Text = "2";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
