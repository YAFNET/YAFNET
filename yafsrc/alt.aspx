<%@ Page Language="C#" %>
<%@ Register TagPrefix="yaf" Namespace="yaf" Assembly="yaf" %>
<script runat="server">
void Page_Load(object sender,System.EventArgs e)
{
	yafForum.Header = yafHeader;
	yafForum.Footer = yafFooter;
}
public void Page_Error(object sender,System.EventArgs e)
{
	yaf.Utils.LogToMail(Server.GetLastError());
}
</script>

<html>
<head>
<meta name="Description" content="A bulletin board system written in ASP.NET">
<meta name="Keywords" content="Yet Another Forum.net, Forum, ASP.NET, BB, Bulletin Board, opensource">
<title runat="server" id="ForumTitle">runat="server" necessary if the forum should set the title</title>
</head>
<body>

<img src="/yetanotherforum.net/images/yaf.png" />
<br />

<form runat="server" enctype="multipart/form-data">

<table border=0 width=100%>
<tr>
	<td colspan=2><yaf:header runat="server" id="yafHeader"/></td>
</tr>
<tr>
	<td width="160px" valign="top"><yaf:test runat="server"/></td>
	<td valign="top"><yaf:forum runat="server" id="yafForum"/></td>
</tr>
<tr>
	<td colspan=2><hr/><yaf:footer runat="server" id="yafFooter"/></td>
</tr>
</table>

</form>

</body>
</html>
