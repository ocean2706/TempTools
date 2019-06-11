using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonEventLib
{
    public class LogEntry
    {
        public static String GetEntryValue(System.Diagnostics.EventLogEntryType e)
        {
            return e.ToString();
        }
        public delegate String ParseToSyslog(System.Diagnostics.EventLogEntry e, String logname);

        public static ParseToSyslog Normalise = new ParseToSyslog((a,b) =>
          {
              String c = String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} ",a.TimeGenerated, a.TimeWritten,a.UserName, 
                  a.InstanceId, a.MachineName,b, 
                  a.Message, a.Source, a.EntryType,a.Category  );

              return c;
          });
        public static List<string> ReadAll()
        {
            List<string> parsed = new List<string>();
            System.Diagnostics.EventLog.GetEventLogs().ToList().ForEach((e) =>
            {
                
                var Entries=e.Entries.Cast<System.Diagnostics.EventLogEntry>().ToList();
                Entries.ForEach((f) =>
                {
                    parsed.Add(Normalise(f, e.LogDisplayName));
                });
            });
                return parsed;
        }


    }
}
