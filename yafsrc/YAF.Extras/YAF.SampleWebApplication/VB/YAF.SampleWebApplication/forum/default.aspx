<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" Inherits="YAF.ForumPageBase" %>
<%@ Register TagPrefix="YAF" Assembly="YAF" Namespace="YAF" %>
<script runat="server">
	
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="YafHead" runat="server">
    <meta id="YafMetaScriptingLanguage" http-equiv="Content-Script-Type" runat="server"
        name="scriptlanguage" content="text/javascript" />
    <meta id="YafMetaStyles" http-equiv="Content-Style-Type" runat="server" name="styles"
        content="text/css" />
    <meta id="YafMetaDescription" runat="server" name="description" content="Yet Another Forum.NET -- A bulletin board system written in ASP.NET" />
    <meta id="YafMetaKeywords" runat="server" name="keywords" content="Yet Another Forum.net, Forum, ASP.NET, BB, Bulletin Board, opensource" />
    <meta name="HandheldFriendly" content="true"/>
    <meta name="viewport" content="width=device-width,user-scalable=yes"/>
    <title></title>
</head>
<body style="margin: 0; padding: 5px">
    <asp:HyperLink runat="server" id="BannerLink" >
        <img src="~/forumlogo.jpg" runat="server" alt="logo" style="border: 0;" id="imgBanner" />
    </asp:HyperLink>
    <br />
    <form id="form1" runat="server" enctype="multipart/form-data">
    <YAF:Forum runat="server" ID="forum" BoardID="1">
    </YAF:Forum>
    </form>
</body>
</html>
