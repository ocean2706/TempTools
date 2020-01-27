using Microsoft.Win32;
using SyslogNet.Client;
using SyslogNet.Client.Serialization;
using SyslogNet.Client.Transport;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LogListener
{
    public class LogListener
    {
        static List<EventLogWatcher> w = null;
        static List<EventLogQuery> q = null;
        private static string syslogServerHostname;

        public static ISyslogMessageSender client { get; private set; }

        public static void SetupAll()
        {
            try
            {
                if (q == null)
                {
                    q = new List<EventLogQuery>();
                    q.Add(new EventLogQuery("Application", PathType.LogName));
                    q.Add(new EventLogQuery("Security", PathType.LogName));
                    q.Add(new EventLogQuery("Setup", PathType.LogName));
                    q.Add(new EventLogQuery("System", PathType.LogName));

                }
                if (w == null)
                {
                    w = new List<EventLogWatcher>();
                    foreach (EventLogQuery z in q)
                    {
                        var t = new EventLogWatcher(z);


                        t.EventRecordWritten += W_EventRecordWritten;
                        t.Enabled = true;
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine("Init error "+e.Message);
            }
        }

        public static void SetupServer(string SyslogServerHostname, String SyslogServerPort)
        {
            //@todo ip or hostname
            syslogServerHostname = SyslogServerHostname;

            try
            {
                port = Int32.Parse(SyslogServerPort);
                // client = new TcpClient(destinationServer, port);
                

            }
            catch (Exception ex)
            {
                Console.WriteLine("Setup server " + ex.Message);
                client = null;
            }
        }
        static Int32 port = 514;
        static String monitor = "1234";
        static void SendMessage(EventRecordWrittenEventArgs e)
        {

            lock (monitor)
            {
                try { 
                String message = e.EventRecord.ToXml();
                ISyslogMessageSerializer serializer = (ISyslogMessageSerializer)new SyslogRfc5424MessageSerializer();
                //: options.SyslogVersion == "3164"
                //    ? (ISyslogMessageSerializer)new SyslogRfc3164MessageSerializer()
                //    : (ISyslogMessageSerializer)new SyslogLocalMessageSerializer();
                SyslogMessage msg1 = CreateSyslogMessage(e);
                
                    // System.Diagnostics.Trace.WriteLine(e.EventRecord.ToXml());
                    Console.WriteLine("New Event "+e.EventRecord.Id + "\n");
                    //msg1=
                    if (client == null)
                    {
                        client = (ISyslogMessageSender)new SyslogTcpSender(syslogServerHostname, port);
                    }
                        client.Send(msg1, serializer);

                    
                }
                catch (Exception ex)
                {
                   // monitor = "0";
                    Console.WriteLine("Eroare 10 " + ex.Message);
                }
            }

        }



        private static SyslogMessage CreateSyslogMessage(EventRecordWrittenEventArgs e)
        {
            var evt = e.EventRecord;

            Facility f = Facility.SystemDaemons; //log facility on server

            //if (evt.ProviderName.Contains("Service"))
            //{
                
            //}else if (evt.ProviderName.Contains("Securit"))
            //{
            //    f = Facility.SecurityOrAuthorizationMessages1;
            //}
            //else
            //{
            //    f = Facility.LogAudit;
            //}


            Severity s = Severity.Critical;
            String evtLevelDisplayName = "Critical";
            try
            {

                evtLevelDisplayName = evt.LevelDisplayName;
            } catch(Exception ey)
            {

            }
                if (evtLevelDisplayName.Contains("Aler"))
                {
                    s = Severity.Alert;
                }
                else if (evtLevelDisplayName.Contains("Critic"))
                {
                    s = Severity.Critical;
                }
                else if (evtLevelDisplayName.Contains("Debug"))
                {
                    s = Severity.Debug;
                }
                else if (evtLevelDisplayName.Contains("Emerg"))
                {
                    s = Severity.Emergency;
                }
                else if (evtLevelDisplayName.Contains("Warni"))
                {
                    s = Severity.Warning;
                }
                else if (evtLevelDisplayName.Contains("Err"))
                {
                    s = Severity.Error;
                }
                else if (evtLevelDisplayName.Contains("Info"))
                {
                    s = Severity.Informational;
                }

            

            return new SyslogMessage(
             new DateTimeOffset(evt.TimeCreated.HasValue ? evt.TimeCreated.Value : DateTime.Now),
               f,
               s,
               evt.MachineName,
               evt.LevelDisplayName+" "+evt.TaskDisplayName,
               (evt.ProcessId.HasValue? evt.ProcessId.Value.ToString():"0"),
               evt.OpcodeDisplayName,
               evt.ToXml()+"\n"
               );
               
        }

        static void SendCallback(IAsyncResult ar)
        {

        }

        /**
         * 
         * Process message and send-it to the server collector
         * 
         */
        private static void W_EventRecordWritten(object sender, EventRecordWrittenEventArgs e)
        {

            try
            {

                if (monitor != "1234")
                {
                    Console.WriteLine(" Event disabled because of exception" + "\n");
                    return;
                }
                ThreadStart threadDelegate = new ThreadStart(() =>
                {
                    SendMessage(e);
                });
                Thread newThread = new Thread(threadDelegate);
                newThread.Start();
            }catch(Exception ex)
            {
                Console.WriteLine("Thread start error " + ex.Message);
            }
        }
    }
}



