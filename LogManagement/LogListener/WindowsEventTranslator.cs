using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogListener
{
    public class WindowsEventTranslator
    {
        public class Event : WindowsEventData
        {
            

        }


    }

    public class WindowsEventData
    {
        public String Level = "";
        public String Task = "";
    }
}
