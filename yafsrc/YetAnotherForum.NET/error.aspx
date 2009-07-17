<%@ Page Language="c#" CodeFile="error.aspx.cs" AutoEventWireup="True" Inherits="YAF.error" %>
<html>
<head>
<title>Forum Error</title>
<link type="text/css" rel="stylesheet" href="resources/forum.css" />
<link type="text/css" rel="stylesheet" href="themes/cleanslate/theme.css" />
</head>
<body>
<div class="yafnet">
<table class="content" width="100%" cellspacing="1" cellpadding="0">
  <tr>
    <td class="header1">
      Forum Error</td>
  </tr>
  <tr>
    <td class="post" align="center">
      <p style="color:#990000;">
      <asp:Label ID="ErrorMsg" Enabled="true" runat="server" />
      </p>
      <hr/>
      <p style="font-size:9pt">
      Note: If you are the administrator, and need help with this problem, please visit this url:
      <a href="http://wiki.yetanotherforum.net/TroubleShooting%20CustomErrors.ashx">Turn off CustomErrors in your web.config</a>.
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