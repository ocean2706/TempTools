using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace CommonEventLib
{
    public delegate void ThresholdReachedEventHandler(object sender, ThresholdReachedEventArgs e);
    public delegate String ParseToSyslog(System.Diagnostics.EventLogEntry e, String logname);
    
    public class ThresholdReachedEventArgs:EventArgs
    {
        public List<Object> NormalisedEvents;
        public String EventLogName;
        
    }
    public class LogEntry
    {
        /*
         * https://github.com/dotnet/dotnet-api-docs/issues/2592

         */

        public static event ThresholdReachedEventHandler OnReadLogEnd;
        private const string EventLogKey = "SYSTEM\\CurrentControlSet\\Services\\EventLog";
        public void ReadLogFromRegistry()
        {
            //internal static RegistryKey GetEventLogRegKey(string machine, bool writable)
            {
                Microsoft.Win32.RegistryKey lmkey = null;

                try
                {
                   //f (machine.Equals("."))
                    {
                        lmkey = Microsoft.Win32.Registry.LocalMachine;
                    }
                    //else
                    //{
                    //    lmkey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, machine);

                    //}
                    if (lmkey != null)
                    {
                        var kl = lmkey.OpenSubKey(EventLogKey, true);
                    }
                }
                finally
                {
                    if (lmkey != null) lmkey.Close();
                }

                //return null;
            }
        }
        public static String GetEntryValue(System.Diagnostics.EventLogEntryType e)
        {
            return e.ToString();
        }
        

        public static ParseToSyslog Normalise = new ParseToSyslog((a,b) =>
          {
              String c = String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} ",a.TimeGenerated, a.TimeWritten,a.UserName, 
                  a.InstanceId, a.MachineName,b, 
                  a.Message, a.Source, a.EntryType,a.Category  );

              return c;
          });
        public static List<string> ReadAll()
        {
            new System.Diagnostics.EventLogPermission(System.Diagnostics.EventLogPermissionAccess.Administer, ".").Demand();
            List<string> parsed = new List<string>();
            List<String> track = new List<string>();
            List<String> LogNames = new List<string>();
            var Logs = System.Diagnostics.EventLog.GetEventLogs(".").ToList();//.Select((e)=> { return e.Log != "Parameters"; });// bug in c#enumeration
            try
            {
              /*  Logs.ForEach((e) => { LogNames.Add(String.Format("Log: {0} Site: {1} Container:{2} Display: {3} ", e.Log, " ", " ", e.LogDisplayName)); });
                */
                //List<System.Diagnostics.EventLog> n = new List<System.Diagnostics.EventLog>(){
                //new System.Diagnostics.EventLog(){Log="Application",MachineName="." },
                //};
                
                Logs.ForEach((e) =>
            {
                try
                {
                    track.Add(e.Log);
                    var Entries = e.Entries.Cast<System.Diagnostics.EventLogEntry>().ToList();
                    Entries.ForEach((f) =>
                    {
                        parsed.Add(Normalise(f, e.Log));
                    });
                }catch(System.Security.SecurityException ex)
                {

                }
                catch (Exception ex)
                {

                }
            });
                return parsed;
            }catch(Exception ex)
            {
                return new List<String>() { ex.Message };
            }
        }
        /**
         * 
         * 
         * 
         */
        public class LogImpersonation
        {
            public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
            {
                private SafeTokenHandle()
                    : base(true)
                {
                }

                [DllImport("kernel32.dll")]
                [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
                [SuppressUnmanagedCodeSecurity]
                [return: MarshalAs(UnmanagedType.Bool)]
                private static extern bool CloseHandle(IntPtr handle);

                protected override bool ReleaseHandle()
                {
                    return CloseHandle(handle);
                }
            }

        }
        

        public static List<Object> ReadAllFromFile()
        {
            
             AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
            //PrincipalPermission perm = new PrincipalPermission("wheel1", "Administrators");
            //perm.Demand();
            var ret=new List<object>();
            if(OnReadLogEnd==null)
            {
                return ret;
            }
            /**
             *              * 
             * wevtutil el
             * C:\Windows\System32\winevt\Logs
             */
            String logPath = String.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.Windows), "system32\\winevt\\Logs");
            Directory.EnumerateFiles(logPath).ToList().ForEach((s) =>
            {
                //if (Path.GetFileName(s).ToLower().StartsWith("param"))
                {
                    try
                    {
                        //string pass = "1a23456Q";//.ToSecureString()
                        //SecureString passcode = new SecureString();// pass.ToSecureString();// new SecureString(pass.Length);
                        //pass.ToCharArray().ToList().ForEach((c) =>
                        //{
                        //    passcode.AppendChar(c);
                        //});
                        //EventLogSession sess = new EventLogSession(".", "","wheel1",passcode,SessionAuthentication.Default);
                        //var d = sess.GetLogNames();

                        //System.Diagnostics.Eventing.Reader.EventLogInformation evtx =  
                        int cnt = 0;
                        using (EventLogReader rdr = new EventLogReader(s, PathType.FilePath))
                        {
                            /** use 1000 events at a time */
                            var r1 = rdr.ReadEvent();
                            while (r1 != null)
                            {
                                cnt++;
                                 ret.Add(r1);
                                r1 = rdr.ReadEvent();
                                if (cnt > 1000)
                                {
                                    OnReadLogEnd(null, new ThresholdReachedEventArgs() { EventLogName = s, NormalisedEvents = ret });
                                    ret.Clear();
                                    cnt = 0;
                                }
                            }
                            if (cnt > 0)
                            {
                                //daca au ramas evenimente pana in 1000
                                OnReadLogEnd(null, new ThresholdReachedEventArgs() { EventLogName = s, NormalisedEvents = ret });
                            }
                            
                        }
                        Console.WriteLine(s + " was here ");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(s + " " + ex.Message+" "+ex.ToString());
                    }
                }
               
            });
            return ret;
        }

    }
}
