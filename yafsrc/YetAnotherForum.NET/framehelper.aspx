<%@ Page Language="C#" %>

<script runat="server">
public void Page_Error(object sender,System.EventArgs e)
{
	YAF.Classes.Core.CreateMail.CreateLogEmail(Server.GetLastError());
}
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> 
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="YafHead" runat="server">	
	<title>This title is overwritten</title>
	<meta http-equiv="refresh" content="600" />
</head>
<body>
<form runat="server" enctype="multipart/form-data" id="Form1">
	<YAF:forum runat="server" ID="Forum1"/>
</form>
</body>
</html>
