using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TestLogListener.Properties;

namespace TestLogListener
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
            Trace.Indent();

            LogListener.LogListener.SetupServer(Settings.Default.SyslogHost,Settings.Default.SyslogPort);
            LogListener.LogListener.SetupAll();
            
            Console.WriteLine("Waiting for something to happen.... press any key to exit");
            Console.ReadKey();
            System.Diagnostics.Trace.WriteLine("test");
            Console.ReadKey();
        }
    }
}
