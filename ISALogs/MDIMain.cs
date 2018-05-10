using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

namespace ISALogs
{
    public partial class MDIMain : Form
    {
        public MDIMain()
        {
            InitializeComponent();
            tbStart.Text = "0:00 1.1.2009";
            tbEnd.Text = "0:00 1.1.2010";
        }

        private void NewCustomReport(object sender, EventArgs e)
        {
            try
            {
                DateTime dtStart = DateTime.Parse(tbStart.Text), dtEnd = DateTime.Parse(tbEnd.Text);
                ReportDocument rep = Report.MakeCustomRep(dtStart, dtEnd);
                if (rep == null)
                    return;
                ReportForm rf = new ReportForm(rep, "Пользовательский отчет");
                rf.MdiParent = this;
                rf.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка создания отчета: " + ex.Message);
            }
        }

        private void OpenFile(object sender, EventArgs e)
        {
            ImportForm imf = new ImportForm();
            imf.ShowDialog();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Выполнил Ланько Евгений");
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                (ActiveMdiChild as ReportForm).crv.ExportReport();
            }
            catch
            {
                MessageBox.Show("Ошибка экспорта");
            }
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                (ActiveMdiChild as ReportForm).crv.PrintReport();
            }
            catch
            {
                MessageBox.Show("Ошибка печати");
            }
        }
        

        private void clientRepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dtStart = DateTime.Parse(tbStart.Text), dtEnd = DateTime.Parse(tbEnd.Text);
                ReportDocument rep = Report.MakeClientRep(dtStart, dtEnd);
                ReportForm rf = new ReportForm(rep, "Журнал клиентов");
                rf.MdiParent = this;
                rf.Show();
            }
            catch
            {
                MessageBox.Show("Ошибка создания отчета");
            }
        }

        private void urlRepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dtStart = DateTime.Parse(tbStart.Text), dtEnd = DateTime.Parse(tbEnd.Text);
                ReportDocument rep = Report.MakeHostRep(dtStart, dtEnd);
                ReportForm rf = new ReportForm(rep, "Посещаемость сайтов");
                rf.MdiParent = this;
                rf.Show();
            }
            catch
            {
                MessageBox.Show("Ошибка создания отчета");
            }
        }

        

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsForm of = new OptionsForm();
            of.ShowDialog();
        }

        private void brownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Analyze.Optimize();
            }
            catch
            {
                MessageBox.Show("Ошибка прогноза");
            }
        }

        private void topUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dtStart = DateTime.Parse(tbStart.Text), dtEnd = DateTime.Parse(tbEnd.Text);
                ReportDocument rep = Report.MakeTopUserRep(dtStart, dtEnd);
                ReportForm rf = new ReportForm(rep, "Максимальный трафик пользователей");
                rf.MdiParent = this;
                rf.Show();
            }
            catch
            {
                MessageBox.Show("Ошибка создания отчета");
            }
        }

        private void topURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dtStart = DateTime.Parse(tbStart.Text), dtEnd = DateTime.Parse(tbEnd.Text);
                ReportDocument rep = Report.MakeTopHostRep(dtStart, dtEnd);
                ReportForm rf = new ReportForm(rep, "Максимальный трафик сайтов");
                rf.MdiParent = this;
                rf.Show();
            }
            catch
            {
                MessageBox.Show("Ошибка создания отчета");
            }
        }

        private void regToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dtStart = DateTime.Parse(tbStart.Text), dtEnd = DateTime.Parse(tbEnd.Text);
                ReportDocument rep = Report.MakeRegRep(dtStart, dtEnd);
                ReportForm rf = new ReportForm(rep, "Журнал");
                rf.MdiParent = this;
                rf.Show();
            }
            catch
            {
                MessageBox.Show("Ошибка создания отчета");
            }
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm hf = new HelpForm();
            hf.ShowDialog();
        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            HelpForm hf = new HelpForm();
            hf.ShowDialog();
        }

        private void userToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dtStart = DateTime.Parse(tbStart.Text), dtEnd = DateTime.Parse(tbEnd.Text);
                ReportDocument rep = Report.MakeUserHostRep(dtStart, dtEnd);
                ReportForm rf = new ReportForm(rep, "Пользователи");
                rf.MdiParent = this;
                rf.Show();
            }
            catch
            {
                MessageBox.Show("Ошибка создания отчета");
            }
        }
    }
}
