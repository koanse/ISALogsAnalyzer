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
    public partial class ModelForm : Form
    {
        public ModelForm(string[] arrS)
        {
            InitializeComponent();
            listBox1.Items.AddRange(arrS);
            TimeSpan ts = Analyze.arrDT[1] - Analyze.arrDT[0], ts2 = new TimeSpan(ts.Ticks / 2);
            Text += " (" + (Analyze.arrDT[0] - new TimeSpan(ts.Ticks / 2)).ToString() + "; ";
            Text += (Analyze.arrDT.Last() + new TimeSpan(ts.Ticks / 2)).ToString() + ")";
            //listBox3.SelectedIndex = 1;
            DrawGraph();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        void DrawGraph()
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            //chart1.
            /*double scale = 1.0 / 10000000;
            switch (listBox3.SelectedIndex)
            {
                case 1:
                    scale /= 3600 * 24;
                    break;
                case 2:
                    scale /= 3600 * 24 * 7;
                    break;
                default:
                    scale /= 3600;
                    break;
            }
            long dt = Analyze.arrDT[1].Ticks - Analyze.arrDT[0].Ticks;
            TimeSpan ts = new TimeSpan(dt), ts2 = new TimeSpan(dt / 2);
            long start = Analyze.arrDT[0].Ticks;*/
            //List<double> list = new List<double>();
            for (int i = 0; i < Analyze.arrTraf.Length; i++)
            {
                //double x = (Analyze.arrDT[i].Ticks - start + dt / 2) * scale;
                //list.Add(x);
                chart1.Series[0].Points.AddXY(Analyze.arrDT[i], Analyze.arrTraf[i]);
                //chart1.Series[0].Points.AddXY(Analyze.arrDT[i] + ts2, Analyze.arrTraf[i]);
            }
            TimeSpan ts = Analyze.arrDT[1] - Analyze.arrDT[0], ts2 = new TimeSpan(ts.Ticks / 2);
            for (int i = 0; i < Analyze.arrF.Length; i++)
            {
                //chart1.Series[1].Points.AddXY((dt * (Analyze.arrTraf.Length + i + 0.5)) * scale, Analyze.arrF[i]);
                chart1.Series[1].Points.AddXY(Analyze.arrDT.Last() + new TimeSpan((i + 1) * ts.Ticks), Analyze.arrF[i]);
                //chart1.Series[1].Points.AddXY(Analyze.arrDT.Last() + ts2 + new TimeSpan((i + 1) * ts.Ticks), Analyze.arrF[i]);
            }

            listBox2.Items.Clear();
            //double dx = (list[1] - list[0]) / 2;
            for (int i = 0; i < Analyze.arrTraf.Length; i++)
            {
                listBox2.Items.Add(string.Format("{0}: {1}\t[{2}; {3})\t{4:f3}", i + 1, Analyze.arrDT[i],
                    Analyze.arrDT[i] - ts2, Analyze.arrDT[i] + ts2, Analyze.arrTraf[i]));
            }
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawGraph();
        }

    }
}
