<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.cp_profile" Codebehind="cp_profile.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="ProfileYourAccount" Src="../controls/ProfileYourAccount.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table width="100%" cellspacing="1" cellpadding="0" class="content">
    <tr>
        <td colspan="2" class="header1">
            <YAF:LocalizedLabel ID="ControlPanel" runat="server" LocalizedTag="CONTROL_PANEL" />
        </td>
    </tr>
    <tr>
        <td class="post">
            <div id="yafprofilecontainer">
                <YAF:ProfileMenu ID="ProfileMenu1" runat="server" />
                <YAF:ProfileYourAccount ID="YourAccount" runat="server" />
            </div>
        </td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
