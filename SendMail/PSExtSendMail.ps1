function SendMailEx ( $SmtpServer, $SmtpServerPort, $Credentials, $Subject, $From, $To, $Body, $Attachment, $EnableSSL ){

[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $True }

 
$emailMessage = New-Object System.Net.Mail.MailMessage( $From , $To )

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

if($Credentials){
    $SMTPClient.UseDefaultCredentials=$False;
    $SMTPClient.Credentials = $Credentials;
	Write-Host "using credentials ";
}
try{
    $SMTPClient.Send( $emailMessage );
    Write-Host "Mail Sent";
}catch{
    Write-Host $PSItem;
}
}

$cred=New-Object System.Net.NetworkCredential("zzzzzz", "@@@@@@@");
SendMailEx -SmtpServer xch1 -SmtpServerPort 2525 -Credentials $cred -From aaaa@bbb.com -To xxx@yyyy.com  -Subject "verificare powershell" -Body "Corp mesaj" -EnableSSL $True


