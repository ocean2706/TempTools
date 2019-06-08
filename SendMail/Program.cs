using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace SendMail
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * $emailSmtpServer = "mail"
$emailSmtpServerPort = "587"
$emailSmtpUser = "user"
$emailSmtpPass = "P@ssw0rd"
 
$emailFrom = "from"
$emailTo = "to"
$emailcc="CC"
 
$emailMessage = New-Object System.Net.Mail.MailMessage( $emailFrom , $emailTo )
$emailMessage.cc.add($emailcc)
$emailMessage.Subject = "subject" 
#$emailMessage.IsBodyHtml = $true #true or false depends
$emailMessage.Body = "body"
 
$SMTPClient = New-Object System.Net.Mail.SmtpClient( $emailSmtpServer , $emailSmtpServerPort )
$SMTPClient.EnableSsl = $False
$SMTPClient.Credentials = New-Object System.Net.NetworkCredential( $emailSmtpUser , $emailSmtpPass );
$SMTPClient.Send( $emailMessage )
*/
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
            if (args.Length != 4)
            {
                Console.WriteLine("Usage:  destination name messagedays subject");
                return ;
            }
            var Config = SendMail.Properties.Settings.Default;
            SmtpClient cl = new SmtpClient();
            cl.Host = Config.smtpServer; //add cheks ?
            cl.Port = Convert.ToInt32(Config.smtpPort);// add checks
            cl.EnableSsl = false;
            cl.UseDefaultCredentials = false;
            cl.Credentials = new System.Net.NetworkCredential(Config.smtpUsername, Config.smtpPassword);
            cl.DeliveryMethod = SmtpDeliveryMethod.Network;
            
            cl.Timeout = 15000;
           
            
            
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("noreply@telekombs.com");
            mail.To.Add(args[0]);
            mail.Subject = args[3];
            mail.Body = System.IO.File.ReadAllText("./MailTemplate.tpl").Replace("$messageDays", args[2]).Replace("$name", args[1]); //@todo read templates
            mail.IsBodyHtml = true;
            try
            {
                cl.Send(mail);
                Console.Write(mail.Body);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }

    }
}
