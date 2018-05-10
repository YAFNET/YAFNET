<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayAd"
    EnableViewState="false" Codebehind="../../../controls/DisplayAd.ascx.cs" %>
<tr class="postheader">
    <td width="140" id="NameCell" runat="server">
        <strong>
            <YAF:LocalizedLabel ID="SponserName" runat="server" LocalizedTag="AD_USERNAME" />
        </strong>
    </td>
</tr>
<tr class="<%#GetPostClass()%>">
    <td valign="top" class="message" >
        <div class="postdiv AdMessage">
            <YAF:MessagePost ID="AdMessage" runat="server"></YAF:MessagePost>
        </div>
    </td>
</tr>