using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Create_ADUser_Generator
{
    public class CAParser
    {

        /**
         * This is used to store extracted data
         * 
         */
        public class BaseAdUserModelParser
        {
             
            public String RawLimitedUser { get; set; }
            public string RawName { get; set; }
            public string RawSurname { get; set; }
            public String RawTitle { get; set; }
            public String RawDepartment { get; set; }
            public String RawManager { get; set; }

            public String RawDistributionGroup { get; set; }
            public String RawHomeFolder { get; set; }
            public String RawShareDriveAccess { get; set; }
            public String RawPhone { get; set; }
            public String RawOffice { get; set; }
            public String RawPrinters { get; set; }
            public String RawMobile { get; set; }
            

            public BaseAdUserModelParser()
            {
                Type t = typeof(BaseAdUserModelParser);
                t.GetProperties().ToList().ForEach((p) =>
                {
                    p.SetValue(this, "");
                });
            }

            internal void PrepareFix()
            {
                Type t = typeof(BaseAdUserModelParser);
                t.GetProperties().ToList().ForEach((p) =>
                {
                    String o=(String)p.GetValue(this);
                    o = o.Trim();
                    p.SetValue(this, o);
                });
            }
        }
        public class AdUserModel:BaseAdUserModelParser
        {
            public AdUserModel()
            {
                Timestamp = DateTime.Now.ToString("YmdHMs");
                if (CurrentContext == null)
                {
                    CurrentContext = ADContext.Default;
                }
            }
            public static ADContext CurrentContext { get; set; }
           
           
            public String Manager { get; set; }
            public String AccountName { get;  set; }
            public string Password { get;  set; }
          
            public string DisplayName { get; set; }
            public string SamAccountName { get; private set; }
            public string HomeDirectory { get; private set; }
            public string GivenName { get; private set; }
            public string Name { get; private set; }
            public string Surname { get; private set; }
            public string Title { get; private set; }
            public string Department { get; private set; }
            public String Timestamp { get; set; }
            public string Mobile { get; private set; }
            public string Phone { get; private set; }
            public string Office { get; private set; }
            public string Company { get; private set; }
            public List<string> DistrbutionGroups { get; private set; }

            public static Object CreateFromModel(AdUserModel b)
            {
                //CurrentContext.ToPrincipalContext();   
                var a = b.AutoCheckAccount();
              


                var v = "";
                

               
                return v;
                
            }

            private string GetHomeDrive()
            {
                return "";
            }

            private AdUserModel AutoCheckAccount()
            {
               
                /*
                 * combina Nume Prenume pentru a gasi o denumire de user. Daca userul exista, redenumeste 
                 * accountName cu o valoare care nu exista, si denumire User. pastreaza nume prenume la display name
                 * verifica daca exista user cu denumirea initiala.
                 */
                 
                return new AdUserModel()
                {
                    DisplayName="",
                    AccountName="",
                    GivenName="",
                    HomeDirectory="",
                    Name="",
                    Password="",
                    Surname=""
                };
            }

            /**
             *  Split Raw values to usable values
             *  @todo fix initial
             */ 
            public void GetValues()
            {
                DisplayName = RawName + " " + RawSurname;
                SamAccountName = DisplayName.Replace(" ",".");
                Name = RawName;
                Surname = RawSurname;
                Title = RawTitle;
                Department = RawDepartment;
                Manager = RawManager;
                HomeDirectory = RawHomeFolder.Contains("1G")?"\\\\srvdlpes\\Users$\\$samaccountname":"";
                Office = RawOffice;
                
                if(HomeDirectory=="" && RawHomeFolder.Contains("G"))
                {
                    HomeDirectory = "\\\\srvdlpes\\Managers$\\$samaccountname";
                }

                DistrbutionGroups = ExtractDistributionList(RawDistributionGroup);
                


            }

            public static String EscapePowershell(String instring)
            {
                String[] list = new String[] { "\"", "$" };
                foreach (string c in list)
                {
                    instring = instring.Replace(c, "`"+c);
                }
                return instring;
            }

            private List<string> ExtractDistributionList(string rawDistributionGroup)
            {

                /**
                 * if it has comma, then split on comma , if not, is only a 
                 */

                List < String > rawList= new List<String>() { rawDistributionGroup.Trim() };
                if (rawDistributionGroup.Contains(','))
                {
                     rawList = rawDistributionGroup.Split(',').ToList();
                    rawList.ForEach((l) =>
                    {
                        l = l.Trim();
                    });
                }
                List<String> finalStr = new List<String>();
                rawList.ForEach((l) => {
                    if (!String.IsNullOrEmpty(l))
                    {
                        finalStr.Add(l);
                    }
                });

                return finalStr;
            }

            public string GetPowershellCreateUserScript()
            {

                this.Company = "DT Business Services";
                StringBuilder bldr = new StringBuilder();
                
                bldr.AppendLine("$samaccountname=\""+this.SamAccountName+"\"");
                bldr.AppendLine("$timestamp=\"" + this.Timestamp + "\"");
               // bldr.AppendLine("$pass= ConvertTo-SecureString \"HRSSC1234!@\" -AsPlainText -Force");
                bldr.AppendLine(@"$manager=Get-AdUser -Filter { ( DisplayName -like '" + this.Manager.Replace(" ", "*") + @"')  }
" + "$useManager=$False");
                bldr.AppendLine(@"if ( $manager -and  !$manager.Count ) {
" +
            @"# found only one manager
" +
    @"$useManager=$True
}
"+@"$existingUser=Get-AdUser $samaccountname
if($existingUser){
    #user already exists with this account name
    $samaccountname="""+this.SamAccountName+this.Timestamp+@"""
}
$homepath="""+this.HomeDirectory+@"""
");
                bldr.AppendLine("$displayname=\""+this.DisplayName+"\""); //append each variable in order to autocomplete path
                bldr.AppendLine("$department=\"" + this.Department + "\""); //append each variable in order to autocomplete path
                bldr.AppendLine("$accountName=\"" + this.AccountName + "\""); //append each variable in order to autocomplete path
                bldr.AppendLine("$mobile=\"" + this.Mobile + "\""); //append each variable in order to autocomplete path
                bldr.AppendLine("$office=\"" + this.Office + "\""); //append each variable in order to autocomplete path
                bldr.AppendLine("$phone=\"" + this.Phone + "\""); //append each variable in order to autocomplete path
                bldr.AppendLine("$title=\"" + this.Title + "\""); //append each variable in order to autocomplete path
                bldr.AppendLine("$company=\""+this.Company+"\"");
                /*   bldr.AppendLine("$department=\"" + this.Department + "\""); //append each variable in order to autocomplete path*/

                string cmd = "$bldr=\"`$pass= ConvertTo-SecureString 'HRSSC1234!@' -AsPlainText -Force `r`n \"";

                    cmd +="$bldr+=\"New-ADUser  -SamAccountName $samaccountname -AccountPassword `$pass -ChangePasswordAtLogon `$True -Company '$company' " +
                            " -Department '$department' -DisplayName '$displayname' -Enabled `$True " +
                            (String.IsNullOrEmpty(this.GivenName) ? "" : " -GivenName '" + this.GivenName + "'") +
                            (String.IsNullOrEmpty(this.HomeDirectory) ? "" : " -HomeDirectory '$homepath'  -HomeDrive I: ") +
                            (String.IsNullOrEmpty(this.Mobile) ? "" : "-MobilePhone '" + this.Mobile+"' ") +
                            " -Name '" + this.Name + "' -Office '$office'" +
                            (String.IsNullOrEmpty(this.Phone) ? "" : " -OfficePhone '" + this.Phone+"'") + 
                            " -Organization '$company' -Surname '" + this.Surname + "' -Title '$title' \"";

                cmd += @"
if ($useManager){
    $m=$manager.SamAccountName
    $bldr+="" -Manager $m ""
}";
                bldr.AppendLine(cmd);
                bldr.AppendLine(@"
$bldr+=""`r`n""

");
                cmd = ""; //cleanup
                          /**
                           * get newly created user
                           */
                cmd = "`$usr=Get-ADUser $samaccountName";
                bldr.AppendLine("$bldr+=\"" + cmd + "\"");
                List<String> ldl = new List<String>() { "All Users", "DTBS_ALL_USERS" };
                ldl.AddRange(DistrbutionGroups);
                ldl.ForEach((l) =>
                {
                    cmd += "Add-ADGroupMember -Identity '"+l+"' -Members '$samaccountname'";
                    bldr.AppendLine("$bldr+=\""+cmd+"\"");
                    cmd = ""; //cleanup
                });
                


                bldr.AppendLine("$bldr");
                
                return bldr.ToString();
            }
        }
       public static BaseAdUserModelParser GenerateAdUserListFromRequest(String requestData)
        {
            NumberFormatInfo nfi = NumberFormatInfo.CurrentInfo;
            List<string> keys = new List<string>()
            {
                "Limited user","Prenume","Nume","Functie",
 "Departament","Manager","Distribution Group","Home Folder","Share Drive access",  "Telefon","Office","Setare Printare","Mobil",
            };
            
            Dictionary<String, String> d = new Dictionary<string, string> {
                { "L(.*)us(.*)", "LimitedUser" },{"Nume","Name" },{"Pre","Surname"},
                {"Functie|Title", "Title" },{"Dep(.*)ment","Department" },{"Ma(.*)ger","Manager" },
                {"Distr(.*)rup","DistributionGroup" },{"Ho(.*)er","HomeFolder"},{"Sh(.*)ive","ShareDriveAccess"},{"Te(.*)","Phone" },{"Mobil","Mobile"},
                {"Loca(.*)|Off(.*)","Office" },{"Print","Printers"}
            };

            String pattern = "";
            keys.ForEach(s => {
                pattern += "((" + s + ")\\:(.*)\\r)|";
            });
            pattern = pattern.Substring(0, pattern.Length - 1);
            
            Match m;
            m = Regex.Match(requestData, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            List<Match> mm = new List<Match>();
            List<PropertyInfo> f = typeof(BaseAdUserModelParser).GetProperties().ToList();
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
            var ret = new AdUserModel();
            Pairs.Keys.ToList().ForEach(k =>
            {

                d.Keys.ToList().ForEach(j =>
                {
                    if (Regex.IsMatch(k, j, RegexOptions.IgnoreCase))
              
                    {
                        f.Find(a =>
                        {
                            return a.Name == "Raw" + d[j];
                        }).SetValue(ret, Pairs[k]);
                        //ret.RawLimitedUser = Pairs[k];
                    };



                });
                
            });
            (ret as BaseAdUserModelParser).PrepareFix();
            ret.GetValues();
            return ret;

        }

       

            public static Object TestGenerateAdUserListFromRequest()
        {
            String f=File.ReadAllText("testCACreateUser.txt");
            return (GenerateAdUserListFromRequest(f) as AdUserModel).GetPowershellCreateUserScript();
        }

    }
    public class ADContext
    {
        public static ADContext Default { get; internal set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String DomainName { get; set; }
        public String RootOU { get; set; }
        /**
         * FQDN, local name, or ip
         */
        public String DomainController { get; set; }
        public PrincipalContext ToPrincipalContext()
        {
            return new PrincipalContext(ContextType.Domain, DomainName, RootOU, ContextOptions.Negotiate, Username, Password);
        
        }
    }
    
}
