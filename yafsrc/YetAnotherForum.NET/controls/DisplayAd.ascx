<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayAd"
    EnableViewState="false" Codebehind="DisplayAd.ascx.cs" %>
<tr class="postheader">
    <td width="140" id="NameCell" runat="server">
        <b>
            <YAF:LocalizedLabel ID="SponserName" runat="server" LocalizedTag="AD_USERNAME" />
        </b>
    </td>
    <td width="80%" colspan="2">
        &nbsp;
    </td>
</tr>
<tr class="<%#GetPostClass()%>">
    <td valign="top" height="100" class="UserBox" colspan='2'>
        &nbsp;
    </td>
    <td valign="top" class="message">
        <div class="postdiv">
            <YAF:MessagePost ID="AdMessage" runat="server"></YAF:MessagePost>
        </div>
    </td>
</tr>
<tr class="postfooter">
    <td class="small" colspan='2'>
        <a href="javascript:scroll(0,0)">
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TOP" />
        </a>
    </td>
    <td class="postfooter">
        &nbsp;
    </td>
</tr>
<tr class="postsep">
    <td colspan="3">

    </td>
</tr>
