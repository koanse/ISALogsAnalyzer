using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ISALogs
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            try
            {
                StreamReader sr = new StreamReader("help.htm", Encoding.Default);
                webBrowser1.DocumentText = sr.ReadToEnd();
                sr.Close();
                webBrowser1.Refresh();
            }
            catch { }
        }
    }
}
