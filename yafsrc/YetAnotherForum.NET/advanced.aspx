<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" Inherits="YAF.ForumPageBase" %>
<%@ Register TagPrefix="YAF" Assembly="YAF" Namespace="YAF" %>
<%@ Register TagPrefix="YAF" Assembly="YAF.Controls" Namespace="YAF" %>
<%@ Register TagPrefix="url" Namespace="Intelligencia.UrlRewriter" Assembly="Intelligencia.UrlRewriter" %>

<!doctype html>
<html lang="en">
<head id="Head1" runat="server">
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
<body>
    <div class="yafnet">
        <img src="~/forumlogo.jpg" runat="server" alt="logo" style="border: 0;" id="imgBanner" />
        <br />
        <url:Form id="Form1" runat="server" enctype="multipart/form-data">
        <br />
        <table border="0" width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td valign="top">
                    <YAF:Forum runat="server" ID="yafForum" />
                </td>
                <td width="10">
                    &nbsp;
                </td>
                <td width="200" valign="top">
                    <YAF:MostActiveUsers ID="MostActiveList" runat="server" DisplayNumber="10" />
                </td>
            </tr>
        </table>
        </url:Form>
    </div>
</body>
</html>
