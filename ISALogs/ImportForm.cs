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
    public partial class ImportForm : Form
    {
        public ImportForm()
        {
            InitializeComponent();
            Access.ReadSettings();
            textBox1.Text = Access.path;
        }
        
        void btnPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;
            textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                List<FileInfo> listToParse = new List<FileInfo>();
                foreach (FileInfo fi in clb.CheckedItems)
                    listToParse.Add(fi);
                backgroundWorker1.RunWorkerAsync(listToParse);
            }
            catch
            {
                MessageBox.Show("Операция не может быть запущена");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Access.path = textBox1.Text;
                Access.start = dtpStart.Value;
                Access.end = dtpEnd.Value;
                clb.Items.Clear();
                clb.Items.AddRange(Access.SelectLogFilesByDateCriteria());
            }
            catch
            {
                MessageBox.Show("Ошибка поиска");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //try
            {
                List<FileInfo> listToParse = e.Argument as List<FileInfo>;
                long total = 0, sum = 0;
                foreach (FileInfo fi in listToParse)
                    total += fi.Length;
                Access.ReadCache();
                foreach (FileInfo fi in listToParse)
                {
                    Access.ReadLogFile(fi, backgroundWorker1, sum, total);
                    sum += fi.Length;
                    if (backgroundWorker1.CancellationPending)
                        return;
                }
                Access.WriteCache();
            }
            //catch
            {
                //MessageBox.Show("Ошибка импорта");
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 100;
            MessageBox.Show("Импорт файлов завершен");
        }
        
        private void ImportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Access.path = textBox1.Text;
            Access.SaveSettings();
            if (backgroundWorker1.IsBusy)
                e.Cancel = true;
            backgroundWorker1.CancelAsync();
        }
    }
}
