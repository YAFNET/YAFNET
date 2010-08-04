<%@ Control Language="c#" CodeFile="cp_editprofile.ascx.cs" AutoEventWireup="True"
    Inherits="YAF.Pages.cp_editprofile" %>
<%@ Register TagPrefix="YAF" TagName="ProfileEdit" Src="EditUsersProfile.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator"></div>
<asp:UpdatePanel ID="ProfileUpdatePanel" runat="server">
    <ContentTemplate>
        <YAF:ProfileEdit runat="server" ID="ProfileEditor" />
    </ContentTemplate>
</asp:UpdatePanel>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
