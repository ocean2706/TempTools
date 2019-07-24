<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InputOperation.aspx.cs" Inherits="UserManagementWebForm.admanage.InputOperation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #inText {
            height: 238px;
            width: 824px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <textarea id="inText" runat="server" wrap="hard" ></textarea></div>
            <div><input type="submit" name="submit" value="Submit" runat="server" /></div>
        <div id="outArea" runat="server" />
 
       
    </form>
</body>
</html>
