using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ISALogs.LogsDBDataSetTableAdapters;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace ISALogs
{
    public partial class ReportForm : Form
    {
        public ReportForm(ReportDocument rep, string text)
        {
            InitializeComponent();
            //GrouIPRep rep = new GrouIPRep();
            //GroupUserRep rep = new GroupUserRep();
            //CrystalReport3 rep = new CrystalReport3();
            //ClientRep rep = new ClientRep();
            //URLReport rep = new URLReport();
            //URLDetRep rep = new URLDetRep();
            //LogsDBDataSet ds = new LogsDBDataSet();
            //HostTableAdapter hta = new HostTableAdapter();
            //UserTableAdapter uta = new UserTableAdapter();
            //SessionTableAdapter sta = new SessionTableAdapter();
            //RecordTableAdapter rta = new RecordTableAdapter();
            //hta.Fill(ds.Host);
            //uta.Fill(ds.User);
            //sta.Fill(ds.Session);
            //rta.Fill(ds.Record);
            //rep.SetDataSource(ds);
            //int i = rep.Rows.Count;
            //DateTime dt = (DateTime)rep.Rows;
            //MessageBox.Show(dt.ToString());
            //rep.Subreports[0].SetDataSource(ds);
            //rep.Refresh();
            /*Section section;
            ReportObject reportObject;
            SubreportObject subreportObject;
            for (int i = 0; i < rep.ReportDefinition.Sections.Count; i++)
            {
                section = rep.ReportDefinition.Sections[i];
                for (int j = 0; j < section.ReportObjects.Count; j++)
                {
                    reportObject = section.ReportObjects[j];
                    if (reportObject.Kind == ReportObjectKind.SubreportObject)
                    {
                        subreportObject = reportObject as SubreportObject;
                        
                        rd.SetDataSource(ds);
                        rd.Refresh();
                        //.ObjectFormat.EnableSuppress = true;
                    }
                }
            }*/


            crv.ReportSource = rep;
            Text = text;
            //crystalReportViewer1.Zoom(50);
            //crystalReportViewer1.ReportSource = rep;

        }
    }
}
