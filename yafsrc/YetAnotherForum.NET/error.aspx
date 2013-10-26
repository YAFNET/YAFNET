<%@ Page Language="c#" AutoEventWireup="True" Inherits="YAF.error" CodeBehind="error.aspx.cs" %>

<%@ Import Namespace="System.Security.Cryptography" %>
<%@ Import Namespace="System.Threading" %>
<script runat="server">
    void Page_Load()
    {
        using (RandomNumberGenerator prng = new RNGCryptoServiceProvider())
        {
            var delay = new byte[1];
            prng.GetBytes(delay);
            Thread.Sleep((int)delay[0]);
        }
    }
</script>
<html>
<head>
    <title>Forum Error</title>
    <link type="text/css" rel="stylesheet" href="resources/css/forum.css" />
    <link type="text/css" rel="stylesheet" href="themes/cleanslate/theme.css" />
</head>
<body>
    <div class="yafnet">
        <table class="content" width="100%" cellspacing="1" cellpadding="0">
            <tr>
                <td class="header1">
                    Forum Error
                </td>
            </tr>
            <tr>
                <td class="post" align="center">
                    <p style="color: #990000;">
                        <asp:Label ID="ErrorMsg" Enabled="true" runat="server" />
                    </p>
                    <hr />
                    <p style="font-size: 9pt">
                        Note: If you are the administrator, and need help with this problem, please
                        Turn off CustomErrors in your web.config.
                    </p>
                </td>
            </tr>
            <tr>
                <td class="footer1" align="center">
                    <a href="Default.aspx">Try Again</a>
                </td>
            </tr>
        </table>
    </div>
</body>
</html>
