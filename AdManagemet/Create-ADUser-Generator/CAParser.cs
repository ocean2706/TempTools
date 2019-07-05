using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace Create_ADUser_Generator
{
    public class CAParser
    {

        public class BaseAdUserModel
        {
            public string Name { get; set; }
            public string Surname { get; set; }
        }
        public class AdUserModel:BaseAdUserModel
        {
            public static String ADDomain { get; set; }
            public static String ContextDefaultUserOUContext { get; set; }
            public String AccountName { get;  set; }
            public string Password { get;  set; }
          
            public string DisplayName { get; set; }
            public string HomeDirectory { get; private set; }
            public string GivenName { get; private set; }

            public static Object CreateFromModel(AdUserModel a)
            {
                a = a.AutoCheckAccount();
                PrincipalContext ctx = null;
                try
                {
                     ctx = new PrincipalContext(
                                             ContextType.Domain,
                                             ADDomain,
                                             ContextDefaultUserOUContext,
                                             a.AccountName,
                                             a.Password);
                }
                catch
                {

                }
                System.DirectoryServices.AccountManagement.UserPrincipal p = new UserPrincipal(ctx);

                p.DisplayName = a.DisplayName;
                p.Enabled = true;
                p.Surname = a.Surname;
                p.Name = a.Name;
                if (a.HomeDirectory.Length > 0)
                {
                    p.HomeDirectory = a.HomeDirectory;
                    p.HomeDrive = a.GetHomeDrive();
                }
                p.GivenName = a.GivenName;
                DirectoryEntry de = p.GetUnderlyingObject() as DirectoryEntry;
               // de.InvokeSet("", "");



                var v = "";
                

               
                return v;
                
            }

            private string GetHomeDrive()
            {
                return "";
            }

            private AdUserModel AutoCheckAccount()
            {
                String timestamp = DateTime.Now.ToString("YmdHMs");
                /*
                 * combina Nume Prenume pentru a gasi o denumire de user. Daca userul exista, redenumeste 
                 * accountName cu o valoare care nu exista, si denumire User. pastreaza nume prenume la display name
                 * verifica daca exista user cu denumirea initiala.
                 */

                return this;
            }
        }
       public static List<Object> GenerateAdUserListFromRequest(String requestData)
        {
            NumberFormatInfo nfi = NumberFormatInfo.CurrentInfo;
            List<string> keys = new List<string>()
            {
                "Limited user","Prenume","Nume","Functie",
 "Departament","Manager","Distribution Group","Home Folder","Share Drive access",  "Telefon","Office"
            };

            String pattern = "";
            keys.ForEach(s => {
                pattern += "((" + s + ")\\:(.*)\\r)|";
            });
            pattern = pattern.Substring(0, pattern.Length - 1);
            
            Match m;
            m = Regex.Match(requestData, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            List<Match> mm = new List<Match>();
            while (m.Success)
            {
                mm.Add(m);
               m= m.NextMatch();
            }
            Dictionary<String, String> Pairs = new Dictionary<string, string>() { };
            int i = 0;
            mm.ForEach(s =>
            {
            Pairs.Add(s.Groups[2+i].ToString(),s.Groups[3+i].ToString());
                i += 3;
            });
            return new List<Object>() { };

        }

        public static List<Object> GenerateAdUserListFromRequestEx(String requestData)
        {
            AdUserModel.CreateFromModel(new AdUserModel());
            List<string> keys = new List<string>()
            {
                "Limited user","Prenume","Nume","Functie",
 "Departament","Manager","Distribution Group","Home Folder","Share Drive access",  "Telefon","Office"
            };

            return new List<Object>() { };
        }

            public static List<Object> TestGenerateAdUserListFromRequest()
        {
            String f=File.ReadAllText("testCACreateUser.txt");
            return GenerateAdUserListFromRequestEx(f);
        }

    }
    
}
