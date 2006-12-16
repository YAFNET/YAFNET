<%@ Page Language="C#" %>
<%@ Register TagPrefix="YAF" Namespace="YAF" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Data" Assembly="YAF.Classes.Data" %>
<script runat="server">
void Page_Load(object sender,System.EventArgs e)
{
	yafForum.Header = yafHeader;
	yafForum.Footer = yafFooter;
}
public void Page_Error(object sender,System.EventArgs e)
{
	Exception x = Server.GetLastError();
	YAF.DB.eventlog_create(yafForum.PageUserID,this,x);
	YAF.Utils.LogToMail(x);
}
</script>

<html>
<head>
<meta name="Description" content="A bulletin board system written in ASP.NET">
<meta name="Keywords" content="Yet Another Forum.net, Forum, ASP.NET, BB, Bulletin Board, opensource">
<title runat="server" id="ForumTitle">runat="server" necessary if the forum should set the title</title>
</head>
<body>

<img src="/images/YAFLogo.png" runat="server" id="imgBanner" />
<br />

<form runat="server" enctype="multipart/form-data">

<table border=0 width=100%>
<tr>
	<td colspan="2"><YAF:Header runat="server" id="yafHeader"/></td>
</tr>
<tr>
	<td width="160px" valign="top"><yaf:test runat="server"/></td>
	<td valign="top"><YAF:Forum runat="server" id="yafForum"/></td>
</tr>
<tr>
	<td colspan="2"><hr/><YAF:Footer runat="server" id="yafFooter"/></td>
</tr>
</table>

</form>

</body>
</html>
