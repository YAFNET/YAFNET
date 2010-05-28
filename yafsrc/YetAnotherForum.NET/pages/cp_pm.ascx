<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.cp_pm" Codebehind="cp_pm.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="PMList" Src="../controls/PMList.ascx" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<div>
	<YAF:ThemeButton ID="NewPM" runat="server" CssClass="yafcssbigbutton rightItem" TextLocalizedTag="BUTTON_NEWPM" TitleLocalizedTag="BUTTON_NEWPM_TT" />
</div>
<br style="clear: both" />
<DotNetAge:Tabs ID="PmTabs" runat="server" ActiveTabEvent="Click" AsyncLoad="false" AutoPostBack="false" Collapsible="false" ContentCssClass="" ContentStyle="" Deselectable="false" EnabledContentCache="false" HeaderCssClass="" HeaderStyle="" OnClientTabAdd="" OnClientTabDisabled="" OnClientTabEnabled="" OnClientTabLoad="" OnClientTabRemove="" OnClientTabSelected="" OnClientTabShow="" SelectedIndex="0" Sortable="false" Spinner="">
	
	<Animations>
	</Animations>
	<Views>
	<DotNetAge:View runat="server"
            ID="View1"
            NavigateUrl=""
            HeaderCssClass=""
            HeaderStyle=""            
            Target="_blank">
	<asp:UpdatePanel ID="InboxTabUpdatePanel" runat="server">
            <ContentTemplate>            
							<YAF:PMList runat="server" View="Inbox" ID="InboxPMList" />
            </ContentTemplate>
            </asp:UpdatePanel>							
	</DotNetAge:View>
	<DotNetAge:View runat="server"
            ID="View2"
            NavigateUrl=""
            HeaderCssClass=""
            HeaderStyle=""
            Target="_blank">
	<asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>               
							<YAF:PMList runat="server" View="Outbox" ID="OutboxPMList" />
            </ContentTemplate>
            </asp:UpdatePanel>							
</DotNetAge:View>
	
	<DotNetAge:View runat="server"
            ID="View3"
            NavigateUrl=""
            HeaderCssClass=""
            HeaderStyle=""
            Target="_blank">
	<asp:UpdatePanel ID="ArchiveTabUpdatePanel" runat="server">
            <ContentTemplate>            
							<YAF:PMList runat="server" View="Archive" ID="ArchivePMList" />
            </ContentTemplate>
            </asp:UpdatePanel>							
	</DotNetAge:View>
	
	</Views>
</DotNetAge:Tabs><br />
<div>
	<YAF:ThemeButton ID="NewPM2" runat="server" CssClass="yafcssbigbutton rightItem" TextLocalizedTag="BUTTON_NEWPM" TitleLocalizedTag="BUTTON_NEWPM_TT" />
</div>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
