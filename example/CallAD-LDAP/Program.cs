using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CallAD_LDAP
{
    class Program
    {
        static void Main(string[] args)
        {
            SetCultureAndIdentity();

        }

        static WindowsPrincipal principal = null;
        static WindowsIdentity identity = null;
        public static void SetCultureAndIdentity()
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
             principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            identity = (WindowsIdentity)principal.Identity;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }
    }
}
