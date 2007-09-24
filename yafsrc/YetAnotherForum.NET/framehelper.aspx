<%@ Page Language="C#" %>

<script runat="server">
public void Page_Error(object sender,System.EventArgs e)
{
	General.LogToMail(Server.GetLastError());
}
</script>

<html>
<head id="YafHead" runat="server">
	<meta http-equiv="refresh" content="600" />
	<title>This title is overwritten</title>
</head>
<body>
<form runat="server" enctype="multipart/form-data" id="Form1">
	<YAF:forum runat="server" ID="Forum1"/>
</form>
</body>
</html>
