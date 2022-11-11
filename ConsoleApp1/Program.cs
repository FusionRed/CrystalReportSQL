using System;
using System.IO;
using System.Text;
using System.Threading;

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
                            Directory.CreateDirectory(String.Concat(pathway, "\\SQL\\"));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Unable to create output pathway.  Create \\SQL\\ folder in filepath, then try again. Exception: { 0}", e.ToString());
                            Console.WriteLine("\n Press any key to exit...");
                            Console.ReadKey();
                            System.Environment.Exit(0);
                        }
                    }
                    foreach (string file in Directory.GetFiles(pathway, "*.rpt"))
                    {
                        var doc = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                        Console.WriteLine(String.Format("Processing For {0}...", file));
                        doc.Load(file);
                        int start = file.LastIndexOf("\\");
                        int end = file.LastIndexOf(".");
                        var length = (end - start) - 1;
                        string filename = file.Substring(start + 1, length);
                        using (var sw = new StreamWriter(String.Concat(pathway, "\\SQL\\", filename, ".sql"), true, Encoding.Unicode))
                        {
                            foreach (dynamic table in doc.ReportClientDocument.DatabaseController.Database.Tables)
                            {
                                if (table.ClassName == "CrystalReports.CommandTable")
                                {
                                    string commandSql = table.CommandText;
                                    Console.WriteLine(String.Format("Writing SQL for {0}", filename));
                                    sw.WriteLine(commandSql);
                                }
                                else
                                {
                                    string tableSql = table.Name;
                                }
                            }
                            sw.Close();
                        }
                        doc.Close();
                    }
                }
                else
                {
                    Console.WriteLine(String.Format("No .rpt files in pathway: {0}", pathway));
                    Console.WriteLine("\n Press any key to exit...");
                    Console.ReadKey();
                    System.Environment.Exit(0);
                }
            }
            else
            {
                Console.WriteLine(String.Format("The pathway: {0} does not exist.  Please check your pathway and try again.", pathway));
                Console.WriteLine("\n Press any key to exit...");
                Console.ReadKey();
                System.Environment.Exit(0);
            }
        }
    }
}
