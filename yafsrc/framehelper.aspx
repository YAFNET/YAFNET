<%@ Page Language="C#" %>

<script runat="server">
public void Page_Error(object sender,System.EventArgs e)
{
	General.LogToMail(Server.GetLastError());
}
</script>

<html>
<head runat="server">
<meta http-equiv="refresh" content="600">
</head>
<body>
<form runat="server" enctype="multipart/form-data" ID="Form1">
	<YAF:forum runat="server" ID="Forum1"/>
</form>
</body>
</html>
