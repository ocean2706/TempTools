using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Text;
using CommonEventLib;

namespace WindowsEventCollectorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console.ReadLine();
            LogEntry.OnReadLogEnd += LogEntry_OnReadLogEnd; 
            LogEntry.ReadAllFromFile();
               
            
        }

        private static void LogEntry_OnReadLogEnd(object sender, ThresholdReachedEventArgs e)
        {
            try
            {
                Console.WriteLine("################ Events for " + e.EventLogName);
                if (!Directory.Exists(".\\logs\\"))
                {
                    Directory.CreateDirectory(".\\logs\\");
                }
                String fName = DateTime.Now.ToString("yyyy-MM-dd_HH_") + Path.GetFileName(e.EventLogName);
                
                String fpath = String.Format("{0}\\{1}", ".\\logs", fName.Replace('%','0'));
                Console.WriteLine("1 " + fpath);
                fpath = Path.ChangeExtension(fpath, ".xml");

                Console.WriteLine("Output to: " + fpath);
                StringBuilder strb = new StringBuilder();
                int cnt = 0;
                e.NormalisedEvents.ForEach((s) =>
                {
                    //        Console.WriteLine((s as EventRecord).ToXml());
                    strb.AppendLine((s as EventRecord).ToXml());
                    cnt++;
                    if(cnt > 100)
                    {
                        File.AppendAllText(fpath, strb.ToString());
                        strb = new StringBuilder();
                        cnt = 0;
                    }

                });
                if (cnt>  0)
                {
                    File.AppendAllText(fpath, strb.ToString());

                }

                Console.WriteLine("################ Events for " + e.EventLogName + " ######## end #### ");
            }catch(Exception ex)
            {
                Console.WriteLine(e.EventLogName+" "+ "error "+ex.Message);
            }
        }
    }
}
