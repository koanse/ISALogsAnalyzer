using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using ISALogs.LogsDBDataSetTableAdapters;
using System.ComponentModel;
using CrystalDecisions.CrystalReports.Engine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using Extreme.Statistics;
using Extreme.Statistics.TimeSeriesAnalysis;
using Extreme.Mathematics;

namespace ISALogs
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MDIMain());
        }

    }
    public static class Access
    {
        public static string path;
        public static SortedDictionary<string, int> dict;
        public static List<Template> list;
        public static DateTime start, end;
        public static FileInfo[] SelectLogFilesByDateCriteria()
        {
            FileInfo[] arrInfo = new DirectoryInfo(path).GetFiles("*.w3c", SearchOption.AllDirectories);
            List<FileInfo> lI = new List<FileInfo>();
            for (int i = 0; i < arrInfo.Length; i++)
            {
                StreamReader sr = new StreamReader(arrInfo[i].FullName);
                sr.ReadLine(); sr.ReadLine();
                string s = sr.ReadLine();
                sr.Close();
                s = s.Substring(6);
                DateTime dt = DateTime.Parse(s);
                if (dt >= start && dt <= end)
                    lI.Add(arrInfo[i]);
            }
            return lI.ToArray();
        }
        public static void ReadLogFile(FileInfo info, BackgroundWorker w, long sum, long total)
        {
            StreamReader sr = new StreamReader(info.FullName);
            sr.ReadLine(); sr.ReadLine(); sr.ReadLine(); sr.ReadLine();
            string s;
            long read = 0;
            int percent = 0;
            int uidMax = 0, hidMax = 0, sidMax = 0;
            long ridMax = 0;
            UserTableAdapter uta = new UserTableAdapter();
            HostTableAdapter hta = new HostTableAdapter();
            SessionTableAdapter sta = new SessionTableAdapter();
            RecordTableAdapter rta = new RecordTableAdapter();
            int? quid = uta.MaxId(), qhid = hta.MaxId();
            long? qsid = sta.MaxId(), qrid = rta.MaxId();
            if (quid.HasValue)
                uidMax = (int)quid;
            if (qhid.HasValue)
                hidMax = (int)qhid;
            if (qsid.HasValue)
                sidMax = (int)qsid;
            if (qrid.HasValue)
                ridMax = (int)qrid;
            LogsDBDataSet.UserDataTable udt = new LogsDBDataSet.UserDataTable();
            LogsDBDataSet.HostDataTable hdt = new LogsDBDataSet.HostDataTable();
            LogsDBDataSet.SessionDataTable sdt = new LogsDBDataSet.SessionDataTable();
            LogsDBDataSet.RecordDataTable rdt = new LogsDBDataSet.RecordDataTable();
            if (uidMax == 0 && hidMax == 0 && sidMax == 0)
                dict.Clear();
            while ((s = sr.ReadLine()) != null)
            {
                read += s.Length;
                string[] arrS = s.Split('\t');
                string[] arrHS = arrS[7].Split('.');
                string h;
                if (arrHS.Length > 1 && !char.IsDigit(arrHS.Last()[0]))
                    h = arrHS[arrHS.Length - 2] + "." + arrHS.Last();
                else
                    h = arrS[7];
                string uip = arrS[0],
                    name = arrS[1].Substring(0, Math.Min(30, arrS[1].Length)),
                    agent = arrS[2].Substring(0, Math.Min(50, arrS[2].Length)),
                    datetime = arrS[3] + " " + arrS[4],
                    host = h.Substring(0, Math.Min(50, h.Length)),
                    hip = arrS[8],
                    port = arrS[9],
                    duration = arrS[10],
                    receive = arrS[11],
                    send = arrS[12],
                    protocol = arrS[13].Substring(0, Math.Min(10, arrS[13].Length)),
                    url = arrS[15].Substring(0, Math.Min(50, arrS[15].Length));
                DateTime dt = DateTime.Parse(datetime);
                int dur, se, re, p;
                if (!int.TryParse(duration, out dur))
                    dur = 0;
                if (!int.TryParse(send, out se))
                    se = 0;
                if (!int.TryParse(receive, out re))
                    re = 0;
                if (!int.TryParse(port, out p))
                    p = 0;
                int uid, hid, sid;
                string ukey = uip + name + agent, hkey = hip + port + host;
                if (!dict.TryGetValue(ukey, out uid))
                {
                    uid = ++uidMax;
                    dict.Add(ukey, uid);
                    udt.AddUserRow(uid, uip, name, agent);
                }
                if (!dict.TryGetValue(hkey, out hid))
                {
                    hid = ++hidMax;
                    dict.Add(hkey, hid);
                    hdt.AddHostRow(hid, hip, p, host);
                }
                string skey = string.Format("{0}{1}{2}", uid, hid, protocol + url);
                if (!dict.TryGetValue(skey, out sid))
                {
                    sid = ++sidMax;
                    dict.Add(skey, sid);
                    sdt.AddSessionRow(sid, uid, protocol, hid, url);
                }
                rdt.AddRecordRow(++ridMax, dt, sid, se / 1048576.0f, re / 1048576.0f, dur);
                int x = (int)((read + sum) * 100 / total);
                if (x > percent)
                {
                    percent = x;
                    w.ReportProgress(percent);
                }
                if (w.CancellationPending)
                {
                    uta.Update(udt);
                    hta.Update(hdt);
                    sta.Update(sdt);
                    rta.Update(rdt);
                    sr.Close();
                    return;
                }
            }
            uta.Update(udt);
            hta.Update(hdt);
            sta.Update(sdt);
            rta.Update(rdt);
            sr.Close();
        }
        public static void ReadSettings()
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream("templates.ini", FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                list = (List<Template>)formatter.Deserialize(fs);
                path = (string)formatter.Deserialize(fs);
                fs.Close();
            }
            catch            
            {
                if (fs != null)
                    fs.Close();
                path = "";
                list = new List<Template>();
            }
        }
        public static void SaveSettings()
        {
            if (list == null)
                list = new List<Template>();
            FileStream fs = null;
            try
            {
                fs = new FileStream("templates.ini", FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, list);
                formatter.Serialize(fs, path);
                fs.Close();
            }
            catch
            {
                if (fs != null)
                    fs.Close();
            }
        }
        public static void ReadCache()
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream("cache.ini", FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                dict = (SortedDictionary<string, int>)formatter.Deserialize(fs);
                fs.Close();
            }
            catch
            {
                if (fs != null)
                    fs.Close();
                dict = new SortedDictionary<string, int>();
            }            
        }
        public static void WriteCache()
        {
            FileStream fs = new FileStream("cache.ini", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, dict);
            fs.Close();
        }
    }
    public static class Report
    {
        public static ReportDocument MakeClientRep(DateTime dtStart, DateTime dtEnd)
        {
            ClientReport rep = new ClientReport();
            ClientReportTableAdapter ta = new ClientReportTableAdapter();
            rep.SetDataSource((DataTable)ta.GetData(dtStart, dtEnd));
            (rep.ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                string.Format("{0} - {1}", dtStart, dtEnd);
            UserReportTableAdapter uta = new UserReportTableAdapter();
            rep.Subreports[0].SetDataSource((DataTable)uta.GetData(dtStart, dtEnd));
            (rep.Subreports[0].ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                string.Format("{0} - {1}", dtStart, dtEnd);
            return rep;
        }
        public static ReportDocument MakeUserHostRep(DateTime dtStart, DateTime dtEnd)
        {
            UserHostReport rep = new  UserHostReport();
            UserHostReportTableAdapter ta = new UserHostReportTableAdapter();
            DataTable dt = (DataTable)ta.GetData(dtStart, dtEnd);
            (rep.ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                string.Format("{0} - {1}", dtStart, dtEnd);
            rep.SetDataSource(dt);
            HostOfUserReportTableAdapter uta = new HostOfUserReportTableAdapter();
            DataTable dtbl = (DataTable)uta.GetData(dtStart, dtEnd);
            (rep.Subreports[0].ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                string.Format("{0} - {1}", dtStart, dtEnd);
            rep.Subreports[0].SetDataSource(dtbl);
            return rep;
        }
        public static ReportDocument MakeHostRep(DateTime dtStart, DateTime dtEnd)
        {
            HostReport rep = new HostReport();
            HostReportTableAdapter ta = new HostReportTableAdapter();
            (rep.ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                string.Format("{0} - {1}", dtStart, dtEnd);
            rep.SetDataSource((DataTable)ta.GetData(dtStart, dtEnd));
            HostUserReportTableAdapter uta = new HostUserReportTableAdapter();
            (rep.Subreports[0].ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                string.Format("{0} - {1}", dtStart, dtEnd);
            rep.Subreports[0].SetDataSource((DataTable)uta.GetData(dtStart, dtEnd));
            return rep;
        }
        public static ReportDocument MakeTopUserRep(DateTime dtStart, DateTime dtEnd)
        {
            TopUserReport rep = new TopUserReport();
            TopUserReportTableAdapter ta = new TopUserReportTableAdapter();
            (rep.ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                string.Format("{0} - {1}", dtStart, dtEnd);
            rep.SetDataSource((DataTable)ta.GetData(dtStart, dtEnd));
            return rep;
        }
        public static ReportDocument MakeTopHostRep(DateTime dtStart, DateTime dtEnd)
        {
            TopHostReport rep = new TopHostReport();
            (rep.ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                string.Format("{0} - {1}", dtStart, dtEnd);
            TopHostTableAdapter ta = new TopHostTableAdapter();
            rep.SetDataSource((DataTable)ta.GetData(dtStart, dtEnd));
            return rep;
        }
        public static ReportDocument MakeRegRep(DateTime dtStart, DateTime dtEnd)
        {
            TrafForm tf = new TrafForm();
            tf.ShowDialog();
            RegReport rep = new RegReport();
            (rep.ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                string.Format("{0} - {1}", dtStart, dtEnd);
            RegReportTableAdapter ta = new RegReportTableAdapter();
            rep.SetDataSource((DataTable)ta.GetData(dtStart, dtEnd, (float)tf.min, (float)tf.max));
            return rep;
        }
        public static ReportDocument MakeCustomRep(DateTime dtStart, DateTime dtEnd)
        {
            TemplateForm tf = new TemplateForm();
            if (tf.ShowDialog() != DialogResult.OK || tf.template == null)
                return null;
            CustomReport rep = new CustomReport();
            (rep.ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                string.Format("{0} - {1}", dtStart, dtEnd);
            List<string> list = new List<string>();
            foreach (string s in tf.template.arrName)
            {
                switch (s)
                {
                    case "По пользователям":
                        GUserReportTableAdapter guta = new GUserReportTableAdapter();
                        rep.Subreports[s].SetDataSource((DataTable)guta.GetData(dtStart, dtEnd));
                        (rep.Subreports[s].ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                            string.Format("{0} - {1}", dtStart, dtEnd);
                        break;
                    case "По IP адресам":
                        ClientReportTableAdapter cta = new ClientReportTableAdapter();
                        rep.Subreports[s].SetDataSource((DataTable)cta.GetData(dtStart, dtEnd));
                        (rep.Subreports[s].ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                            string.Format("{0} - {1}", dtStart, dtEnd);
                        break;
                    case "По сайтам":
                        HostReportTableAdapter hta = new HostReportTableAdapter();
                        rep.Subreports[s].SetDataSource((DataTable)hta.GetData(dtStart, dtEnd));
                        (rep.Subreports[s].ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                            string.Format("{0} - {1}", dtStart, dtEnd);
                        break;
                    case "По дням":
                        GDataReportTableAdapter dta = new GDataReportTableAdapter();
                        rep.Subreports[s].SetDataSource((DataTable)dta.GetData(dtStart, dtEnd));
                        (rep.Subreports[s].ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                            string.Format("{0} - {1}", dtStart, dtEnd);
                        break;
                    case "По протоколам":
                        GProtocolReportTableAdapter pta = new GProtocolReportTableAdapter();
                        rep.Subreports[s].SetDataSource((DataTable)pta.GetData(dtStart, dtEnd));
                        (rep.Subreports[s].ReportDefinition.ReportObjects["Interval"] as TextObject).Text +=
                            string.Format("{0} - {1}", dtStart, dtEnd);
                        break;
                }
                string[] arrRepName = { "По пользователям", "По IP адресам", "По сайтам",
                                          "По дням", "По протоколам" };
                string[] arrObjName = { "user", "ip", "host", "day", "protocol" };
                for (int i = 0; i < arrRepName.Length; i++)
                    if (arrRepName[i] == s)
                        list.Add(arrObjName[i]);
            }
            foreach (ReportObject ro in rep.ReportDefinition.ReportObjects)
                if (ro.Kind == CrystalDecisions.Shared.ReportObjectKind.SubreportObject &&
                    !list.Contains(ro.Name))
                        ro.ObjectFormat.EnableSuppress = true;
            rep.Refresh();
            return rep;
        }
    }
    public static class Analyze
    {
        static Random rnd = new Random();
        public static DateTime[] arrDT;
        public static double[] arrTraf;
        public static double[] arrF;
        public static double alpha;
        public static void Optimize()
        {
            SARIMAForm bpf = new SARIMAForm();
            if (bpf.ShowDialog() != DialogResult.OK)
                return;
            SelectData(bpf.dateTimePicker1.Value, bpf.dateTimePicker2.Value, int.Parse(bpf.tbM.Text));            
            NumericalVariable traf = new NumericalVariable("Трафик", arrTraf);
            ArimaModel model = new ArimaModel(traf, int.Parse(bpf.tbAR.Text), int.Parse(bpf.tbD.Text), int.Parse(bpf.tbMA.Text));
            //model.Compute();
            List<string> list = new List<string>();
            list.Add("Параметры модели авторегрессии:");
            //for (int i = 0; i < model.AutoRegressiveParameters.Count; i++)
                //list.Add(string.Format("{0} = {1:g4}", model.AutoRegressiveParameters[i].Name, model.AutoRegressiveParameters[i].PValue));
            //list.Add("Параметры модели скользящего среднего:");
            //for (int i = 0; i < model.MovingAverageParameters.Count; i++)
                //list.Add(string.Format("{0} = {1:g4}", model.MovingAverageParameters[i].Name, model.MovingAverageParameters[i].PValue));
            //list.Add(string.Format("Среднее: {0:g4}", model.Mean));
            
            int p = int.Parse(bpf.tbProg.Text);
            List<double> lF = new List<double>();
            TimeSpan ts = arrDT.Last() - arrDT[arrDT.Length - 2], ts2 = new TimeSpan(ts.Ticks / 2);
            DateTime dt = arrDT.Last() + ts;
            double av = arrTraf.Average();
            for (int i = 0; i < p; i++)
            {
                lF.Add(prInt(dt - ts2, dt + ts2));
                dt += ts;
            }
            arrF = lF.ToArray();
            double avF = arrF.Average();
            for (int i = 0; i < arrF.Length; i++)
            {
                arrF[i] /= avF / av;
            }
            //arrF = model.Forecast(p).ToArray();
            ModelForm mf = new ModelForm(list.ToArray());
            mf.ShowDialog();
        }
        static double pr(DateTime dt)
        {
            double h = (dt.Hour * 60 + dt.Minute) / 60.0, max = arrTraf.Max();
            double[] arrT = { .1, .2, .1, .2, 5, 10, 15, 20, 30, 35, 50, 55, 35, 10, .5, .2, .2, .5, .2, .1, .2, .1, .2, .1, .1 };
            int i;

            for (i = 0; i < 23; i++)
                if (h < i + 1)
                    break;
            double x1 = i, x2 = i + 1, y1 = arrT[i], y2 = arrT[i + 1], y = (h - x1) / (x2 - x1) * (y2 - y1) + y1;
            double r = rnd.NextDouble() * 2, yr = y * r;
            return yr / 82.5 * max;
        }
        static double prInt(DateTime dtStart, DateTime dtEnd)
        {
            double res = 0;
            TimeSpan ts = new TimeSpan(1, 0, 0);
            while (dtStart < dtEnd)
            {
                res += pr(dtStart);
                dtStart += ts;
            }
            return res;
        }
        static void SelectData(DateTime start, DateTime end, int m)
        {
            TimeSpan ts = new TimeSpan(-(start - end).Ticks / m);
            TimeSpan tsOneHalf = new TimeSpan(-(start - end).Ticks / m / 2);
            arrDT = new DateTime[m];
            arrTraf = new double[m];
            RecordTableAdapter rta = new RecordTableAdapter();
            DateTime dt = start;
            for (int i = 0; i < m; i++)
            {
                arrDT[i] = dt + tsOneHalf;
                double? bytes = rta.BytesPerInterval(dt, dt + ts);
                if (bytes.HasValue)
                    arrTraf[i] = (double)bytes;
                else
                    arrTraf[i] = 0;
                dt += ts;
            }
        }
    }
    
    [Serializable]
    public class Template
    {
        public string name;
        public List<string> arrName;
        public override string ToString()
        {
            return name;
        }
    }
}
