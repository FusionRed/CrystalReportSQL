using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportAppServer.ClientDoc;
using CrystalDecisions.ReportAppServer.Controllers;
using CrystalDecisions.ReportAppServer.DataDefModel;
using CrystalDecisions;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;

namespace CrystalReportSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(String.Format("Enter .rpt source file(s) pathway:"));
            string pathway = Console.ReadLine();
            if (Directory.Exists(pathway))
            {
                int cnt = 0;
                foreach (string file in Directory.GetFiles(pathway, "*.rpt"))
                {
                    cnt++;
                }
                if (cnt > 0)
                {
                    if (!Directory.Exists(String.Concat(pathway, "\\SQL\\")))
                    {
                        try
                        {
                            Directory.CreateDirectory(String.Concat(pathway, "\\Report\\"));
                            Directory.CreateDirectory(String.Concat(pathway, "\\SubReport\\"));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Unable to create output folders.  Create folders for \\Report\\ and \\SubReport\\ in the file pathway, then try again. Exception: { 0}", e.ToString());
                            Console.WriteLine("\nPress any key to exit...");
                            Console.ReadKey();
                            System.Environment.Exit(0);
                        }
                    }
                    foreach (string file in Directory.GetFiles(pathway, "*.rpt"))
                    {
                        var doc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                        Console.WriteLine(String.Format("Processing started for report: {0}...", file));
                        doc.Load(file);
                        int start = file.LastIndexOf("\\");
                        int end = file.LastIndexOf(".");
                        var length = (end - start) - 1;
                        string filename = file.Substring(start + 1, length);
                        using (var sw = new StreamWriter(String.Concat(pathway, "\\Report\\", filename, ".txt"), true, Encoding.Unicode))
                        {
                            foreach (dynamic table in doc.ReportClientDocument.DatabaseController.Database.Tables)
                            {
                                if (table.ClassName == "CrystalReports.CommandTable")
                                {
                                    string commandSql = table.CommandText;
                                    Console.WriteLine(String.Format("Writing SQL for {0}...", filename));
                                    sw.WriteLine(commandSql);
                                }
                                else
                                {
                                    string tableSql = table.Name;
                                    Console.WriteLine(String.Format("Writing tables for {0}...", filename));
                                    sw.WriteLine(tableSql);
                                }
                            }
                            sw.Close();
                        }
                        Console.WriteLine(String.Format("Processing {0} for sub-reports...", file));
                        foreach (string subName in doc.ReportClientDocument.SubreportController.GetSubreportNames())
                        {
                            CrystalDecisions.ReportAppServer.Controllers.SubreportClientDocument subRCD = doc.ReportClientDocument.SubreportController.GetSubreport(subName);
                            using (var sw = new StreamWriter(String.Concat(pathway, "\\SubReport\\", filename, "-", subName, ".txt"), true, Encoding.Unicode))
                            {
                                var boDatabase = subRCD.DataDefController.Database;
                                foreach (dynamic subtable in boDatabase.Tables)
                                {
                                    if (subtable.ClassName == "CrystalReports.CommandTable")
                                    {
                                        string commandSql = subtable.CommandText;
                                        Console.WriteLine(String.Format("Writing SQL for {0} sub-report...", subName));
                                        sw.WriteLine(commandSql);
                                    }
                                    else
                                    {
                                        string tableSql = subtable.Name;
                                        Console.WriteLine(String.Format("Writing tables for {0} sub-report...", subName));
                                        sw.WriteLine(tableSql);
                                    }
                                }
                                sw.Close();
                            }
                        }
                        doc.Close();
                    }
                }
                else
                {
                    Console.WriteLine(String.Format("No .rpt files in pathway: {0}", pathway));
                    Console.WriteLine("\nPress any key to exit...");
                    Console.ReadKey();
                    System.Environment.Exit(0);
                }
            }
            else
            {
                Console.WriteLine(String.Format("The pathway: {0} does not exist.  Please check your pathway and try again.", pathway));
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                System.Environment.Exit(0);
            }
            Console.WriteLine("\nDone processing report files.  Press any key to exit...");
            Console.ReadKey();
        }
    }
}
