using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Create_ADUser_Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            //CAParser.AdUserModel.ContextDomainController = Properties.Settings.Default.AdDomain;
            //CAParser.AdUserModel.ContextDefaultUserOUContext = "";
            //CAParser.AdUserModel.ContextUser = Properties.Settings.Default.AdAdmin;
            //CAParser.AdUserModel.ContextUserPass = Properties.Settings.Default.AdAdminPass;
            ADContext.Default = new ADContext();

           var s= CAParser.TestGenerateAdUserListFromRequest();
            
        }
    }
}
