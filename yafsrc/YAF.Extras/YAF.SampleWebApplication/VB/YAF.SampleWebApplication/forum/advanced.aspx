<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" Inherits="YAF.ForumPageBase" %>
<%@ Register TagPrefix="YAF" Assembly="YAF" Namespace="YAF" %>
<%@ Register TagPrefix="YAF" Assembly="YAF.Controls" Namespace="YAF" %>

<script runat="server">
    void Page_Load(object sender, System.EventArgs e)
    {
        yafForum.Header = yafHeader;
        yafForum.Footer = yafFooter;
    }

</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="Description" content="Yet Another Forum.NET -- A bulletin board system written in ASP.NET" />
    <meta name="Keywords" content="Yet Another Forum.net, Forum, ASP.NET, BB, Bulletin Board, opensource" />
    <title></title>
</head>
<body>
    <div class="yafnet">
        <img src="~/images/YAFLogo.jpg" runat="server" alt="YetAnotherForum" id="imgBanner" />
        <br />
        <form id="Form1" runat="server" enctype="multipart/form-data">
        <table border="0" width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td colspan="3">
                    <YAF:Header runat="server" ID="yafHeader" />
                </td>
            </tr>
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
            <tr>
                <td colspan="3">
                    <YAF:Footer runat="server" ID="yafFooter" />
                </td>
            </tr>
        </table>
        </form>
    </div>
</body>
</html>
