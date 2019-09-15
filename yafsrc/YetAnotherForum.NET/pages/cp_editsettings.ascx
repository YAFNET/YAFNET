<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.cp_editsettings" Codebehind="cp_editsettings.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="ProfileSettings" Src="../controls/EditUsersSettings.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
<div class="col-sm-auto">
    <YAF:ProfileMenu ID="ProfileMenu1" runat="server" />
</div>
<div class="col">
<asp:UpdatePanel ID="ProfileUpdatePanel" runat="server">
    <ContentTemplate>
        <YAF:ProfileSettings runat="server" ID="ProfileSettings" />
    </ContentTemplate>
</asp:UpdatePanel>
    </div>
</div>