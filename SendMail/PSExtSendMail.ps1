function SendMailEx ( $SmtpServer, $SmtpServerPort, $Credentials, $Subject, $From, $To, $Body, $Attachment, $EnableSSL ){

[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $True }
#$emailSmtpServer = "mail"
#$emailSmtpServerPort = "587"
#$emailSmtpUser = "user"
#$emailSmtpPass = "P@ssw0rd"
 
#$emailFrom = "from"
#$emailTo = "to"
#$emailcc="CC"
 
$emailMessage = New-Object System.Net.Mail.MailMessage( $From , $To )
#$emailMessage.cc.add($emailcc)
$emailMessage.Subject = $Subject; 
$emailMessage.IsBodyHtml = $True;  
$emailMessage.Body = $Body;
 
$SMTPClient = New-Object System.Net.Mail.SmtpClient( $SmtpServer , $SmtpServerPort );
$SMTPClient.Timeout=60000;

if( $EnableSSL){
$SMTPClient.EnableSsl = $True;

} else {
$SMTPClient.EnableSsl = $False;
}

if($credentials){
    $SMTPClient.UseDefaultCredentials=$False;
    $SMTPClient.Credentials = $Credentials;
}
try{
    $SMTPClient.Send( $emailMessage );
    Write-Host "Mail Sent";
}catch{
    Write-Host $PSItem;
}
}

$cred=New-Object System.Net.NetworkCredential("george.bungarzescu@gmail.com", "thisisnotmypassword");
SendMailEx -SmtpServer smtp.gmail.com -SmtpServerPort 587 -Credentials $cred -From george.bungarzescu@gmail.com -To george.bungarzescu@gmail.com -Subject "verificare powershell" -Body "Corp mesaj" -EnableSSL $true


