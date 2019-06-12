using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using CommonEventLib;

namespace WindowsEventCollectorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console.ReadLine();
            var f = LogEntry.ReadAllFromFile();
                f.ForEach((s)=>
            {
                Console.WriteLine((s as EventRecord).ToXml());
            });
            Console.WriteLine(f.Count);
        }
    }
}
