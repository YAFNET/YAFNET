<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="yaf.pages._default" %>
<%@ Register TagPrefix="yaf" Namespace="yaf" Assembly="yaf" %>

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
	<yaf:forum runat="server" root="/yetanotherforum.net"/>
</form>

</body>
</html>
