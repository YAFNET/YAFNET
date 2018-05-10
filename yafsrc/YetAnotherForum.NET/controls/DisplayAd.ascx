<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayAd"
    EnableViewState="false" Codebehind="DisplayAd.ascx.cs" %>
<table class="content postContainer" width="100%">
    <tr class="postheader">
     <td width="140" id="NameCell" class="postUser" runat="server">
        <strong>
            <YAF:LocalizedLabel ID="SponserName" runat="server" LocalizedTag="AD_USERNAME" />
        </strong>
    </td>
    <td width="80%" class="postPosted">
    </td>
</tr>
<tr class="<%#GetPostClass()%>">
    <td valign="top" class="UserBox">
    </td>
    <td valign="top" class="message">
        <div class="postdiv AdMessage">
            <YAF:MessagePost ID="AdMessage" runat="server"></YAF:MessagePost>
        </div>
    </td>
</tr>
<tr class="postfooter">
    <td class="small postTop">
        <a onclick="ScrollToTop();" class="postTopLink" href="javascript: void(0)">            
            <YAF:ThemeImage ID="ThemeImage1" LocalizedTitlePage="POSTS" LocalizedTitleTag="TOP"  runat="server" ThemeTag="TOTOPPOST" />
        </a>
    </td>
    <td class="postfooter postInfoBottom">
        &nbsp;
    </td>
</tr>
<tr class="postsep">
    <td colspan="2">

    </td>
</tr>
</table>