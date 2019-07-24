using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace CallPowershell_AD
{
    class Program
    {
        static void Main(string[] args)
        {
            String cmd = "Get-ADUsers -Filter * -Properties *";

            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddCommand(cmd);
                var result = PowerShellInstance.Invoke();
            }

        }
    }
}
