<%@ Page Language="C#" Async="true" AutoEventWireup="true" ValidateRequest="false" Inherits="YAF.Core.BasePages.ForumPageBase" MaintainScrollPositionOnPostback="true" %>

<!doctype html>
<html lang="en">
<head id="YafHead" runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta id="YafMetaScriptingLanguage" http-equiv="Content-Script-Type" runat="server"
        name="scriptlanguage" content="text/javascript" />
    <meta id="YafMetaStyles" http-equiv="Content-Style-Type" runat="server" name="styles"
        content="text/css" />
    <meta id="YafMetaDescription" runat="server" name="description" content="YetAnotherForum.NET -- A bulletin board system written in ASP.NET" />
    <meta id="YafMetaKeywords" runat="server" name="keywords" content="Yet Another Forum .Net, Forum, ASP.NET, BB, Bulletin Board, opensource" />
    <title></title>
</head>
<body id="YafBody" runat="server">
<asp:HyperLink runat="server" id="BannerLink">
    <asp:Image runat="server" ID="ForumLogo" Width="276" Height="128">
    </asp:Image>
</asp:HyperLink>
<YAF:Form id="form1" runat="server" enctype="multipart/form-data">
    <YAF:Forum runat="server" ID="forum" BoardID="1">
    </YAF:Forum>
</YAF:Form>
</body>
</html>
