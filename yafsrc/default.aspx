<%@ Page Language="C#" %>
<%@ Register TagPrefix="yaf" Namespace="yaf" Assembly="yaf" %>
<html>
<head>
<meta name="Description" content="A bulletin board system written in ASP.NET">
<meta name="Keywords" content="Yet Another Forum.net, Forum, ASP.NET, BB, Bulletin Board, opensource">
<!-- If you don't want the forum to set the page title, you can remove runat and id -->
<title runat="server" id="ForumTitle">This title is overwritten</title>
</head>
<body>

<img src="/yetanotherforum.net/images/yaf.png" />
<br />

<form runat="server" enctype="multipart/form-data">
	<yaf:forum runat="server"/>
</form>

</body>
</html>
