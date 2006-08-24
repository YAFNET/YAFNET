<%@ Page Language="C#" Theme="Default" %>
<%@ Register TagPrefix="yaf" Namespace="yaf" Assembly="yaf" %>
<%@ Register TagPrefix="yc" Namespace="yaf.controls" Assembly="yaf" %>

<script runat="server">
public void Page_Error(object sender,System.EventArgs e)
{
	Exception x = Server.GetLastError();
	yaf.DB.eventlog_create(forum.PageUserID,this,x);
	yaf.Utils.LogToMail(x);
}
</script>

<html>
<head runat="server">
<meta name="Description" content="A bulletin board system written in ASP.NET">
<meta name="Keywords" content="Yet Another Forum.net, Forum, ASP.NET, BB, Bulletin Board, opensource">
<!-- If you don't want the forum to set the page title, you can remove runat and id -->
<title runat="server" id="ForumTitle">This title is overwritten</title>
</head>
<body>

<img src="images/yaflogo.jpg" runat="server" id="imgBanner" />
<br />

<form runat="server" enctype="multipart/form-data">
	<yaf:forum runat="server" id="forum" />
</form>

</body>
</html>
