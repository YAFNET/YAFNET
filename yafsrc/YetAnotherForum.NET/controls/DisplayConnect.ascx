<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayConnect"
    EnableViewState="false" Codebehind="DisplayConnect.ascx.cs" %>
<table class="content postContainer" width="100%">
    <tr class="postheader">
     <td width="140" id="NameCell" class="postUser" runat="server">
       
    </td>
    <td width="80%" class="postPosted">
    </td>
</tr>
<tr class="<%#GetPostClass()%>">
    <td valign="top" class="UserBox">
    </td>
    <td valign="top" class="message">
        <div class="postdiv">
            <div class="ui-widget">
                <div class="ui-state-highlight ui-corner-all" style="padding: 0 .7em;">
                    <p><span class="ui-icon ui-icon-info"></span><asp:PlaceHolder runat="server" ID="ConnectHolder"></asp:PlaceHolder></p>
                </div>
            </div>
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