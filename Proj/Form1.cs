using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proj
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Excel Log|*.csv";
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            List<KeyValuePair<string, List<string>>> newLog = ConvertCsvToDictionary(openFileDialog1.FileName);

            string newFile = "output.csv";
            WriteListToCSV(newLog, newFile);
            Dictionary<string, int> countDict = ListToDictionaryForCount(newLog);
            string newFile2 = "output2.csv";
            WriteDictToCSV(countDict, newFile2);
            //MessageBox.Show(LogidCount + ", " + dateCount + ", " + WafLogCount);
        }
        static Dictionary<string, int> ListToDictionaryForCount(List<KeyValuePair<string, List<string>>> log)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (KeyValuePair<string, List<string>> item in log)
            {
                if (!dict.ContainsKey(item.Key))
                {
                    dict[item.Key] = 1;
                }
                else
                {
                    dict[item.Key]++;
                }
            }
            return dict;
        }
        static void WriteDictToCSV(Dictionary<string, int> dict, string csvFile)
        {
            using (var writer = new StreamWriter(csvFile))
            {
                foreach (var item in dict)
                {
                    string line = item.Key + "," + string.Join(",", item.Value);
                    writer.WriteLine(line);
                }
            }
        }
        static void WriteListToCSV(List<KeyValuePair<string, List<string>>> log, string csvFile)
        {
            using (var writer = new StreamWriter(csvFile))
            {
                foreach (var item in log)
                {
                    string line = item.Key + ",";
                    foreach (var listItem in item.Value)
                    {
                        line += listItem + ",";
                    }
                    line = line.TrimEnd(',');
                    writer.WriteLine(line);
                }
            }
        }
        static List<KeyValuePair<string, List<string>>> ConvertCsvToDictionary(string csvFile)
        {
            
            List<KeyValuePair<string, List<string>>> result = new List<KeyValuePair<string, List<string>>>();

            using (var reader = new StreamReader(csvFile))
            {
                string lineFirst = reader.ReadLine();
                string[] valuesFirst = lineFirst.Split(',');
                int count = 0;
                int LogidCount = 0;
                int dateCount = 0;
                int WafLogCount = 0;
                foreach (string s in valuesFirst)
                {
                    if (s == "report_timestamp")
                    {
                        dateCount = count;
                    }
                    else if (s == "log_id")
                    {
                        LogidCount = count;
                    }
                    else if (s == "waf_log")
                    {
                        WafLogCount = count;
                    }
                    count++;
                }
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    int valuesCount = 0;
                    string WafLog = "";
                    for (int i = WafLogCount + 1; i < values.Count(); i++)
                    {
                        WafLog += values[i] + ',';
                    }
                    string[] LogDetail = WafLog.Split(',');

                    foreach (string s in LogDetail)
                    {
                        if (s.Contains("rule_id"))
                        {
                            List<string> DetailList = new List<string>();
                            DetailList.Add(LogDetail[valuesCount + 1]); //add value into list
                            DetailList.Add(values[LogidCount]);
                            DetailList.Add(values[dateCount]);
                            KeyValuePair<string, List<string>> kvp = new KeyValuePair<string, List<string>>(s, DetailList);
                            result.Add(kvp);
                            
                        }
                        valuesCount++;
                    }
                }
            }
            return result;
        }
    }
}
