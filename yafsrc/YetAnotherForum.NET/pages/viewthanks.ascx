<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.ViewThanks" Codebehind="ViewThanks.ascx.cs" %>
<%@ Import Namespace="YAF.Classes.Core" %>
<%@ Register TagPrefix="YAF" TagName="ViewThanksList" Src="../controls/ViewThanksList.ascx" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<div class="DivTopSeparator">
</div>
<asp:UpdatePanel ID="ThanksUpdatePanel" runat="server">
    <ContentTemplate>
        <br style="clear: both" />
        <DotNetAge:Tabs ID="ThanksTabs" runat="server" ActiveTabEvent="Click" AsyncLoad="false"
            AutoPostBack="false" Collapsible="false" ContentCssClass="" ContentStyle="" Deselectable="false"
            EnabledContentCache="false" HeaderCssClass="" HeaderStyle="" OnClientTabAdd=""
            OnClientTabDisabled="" OnClientTabEnabled="" OnClientTabLoad="" OnClientTabRemove=""
            OnClientTabSelected="" OnClientTabShow="" SelectedIndex="0" Sortable="false"
            Spinner="">
            <Views>
                <DotNetAge:View runat="server" ID="ThanksFromTab" NavigateUrl="" HeaderCssClass=""
                    HeaderStyle="" Target="_blank">
                    <YAF:ViewThanksList runat="server" ID="ThanksFromList" CurrentMode="FromUser" />
                </DotNetAge:View>
                <DotNetAge:View runat="server" ID="ThanksToTab" NavigateUrl="" HeaderCssClass=""
                    HeaderStyle="" Target="_blank">
                    <YAF:ViewThanksList runat="server" ID="ThanksToList" CurrentMode="ToUser" />
                </DotNetAge:View>
            </Views>
        </DotNetAge:Tabs>
    </ContentTemplate>
</asp:UpdatePanel>
<div id="Div1">
    <YAF:SmartScroller ID="SmartScroller2" runat="server" />
</div>
