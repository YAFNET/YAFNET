<%@ Control Language="C#" AutoEventWireup="true" CodeFile="cp_pm.ascx.cs" Inherits="YAF.Pages.cp_pm" %>
<%@ Register TagPrefix="YAF" TagName="PMList" Src="../controls/PMList.ascx" %>
<!-- This style is dealing with tabs rendering issues in IE - should be removed once YAF is fully XHTML 1.0 compliant -->
<!--style type="text/css">
.ajax__tab_default .ajax__tab_inner {height : 100%;} .ajax__tab_default .ajax__tab_tab {height : 100%;} .ajax__tab_xp .ajax__tab_hover .ajax__tab_tab {height : 100%;} .ajax__tab_xp .ajax__tab_active .ajax__tab_tab {height : 100%;} .ajax__tab_xp .ajax__tab_inner {height : 100%;} .ajax__tab_xp .ajax__tab_tab {height:100%} .ajax__tab_xp .ajax__tab_hover .ajax__tab_inner {height : 100%;} .ajax__tab_xp .ajax__tab_active .ajax__tab_inner {height : 100%;} 
</style-->
<YAF:PageLinks runat="server" ID="PageLinks" />
<div>
    <YAF:ThemeButton ID="NewPM" runat="server" CssClass="yafcssbigbutton" TextLocalizedTag="BUTTON_NEWPM"
        TitleLocalizedTag="BUTTON_NEWPM_TT" />
</div>
<div style="clear: both" />

<ajaxToolkit:TabContainer runat="server" ID="PMTabs">
    <ajaxToolkit:TabPanel runat="server" ID="InboxTab" OnClientClick="InboxTabRefresh">
        <ContentTemplate>
            <asp:UpdatePanel ID="InboxTabUpdatePanel" runat="server">
            <ContentTemplate>
                <YAF:PMList runat="server" View="Inbox" ID="InboxPMList" />
            </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="OutboxTab" OnClientClick="SentTabRefresh">
        <ContentTemplate>
            <asp:UpdatePanel ID="SentTabUpdatePanel" runat="server">
            <ContentTemplate>        
                <YAF:PMList runat="server" View="Outbox" ID="OutboxPMList" />
            </ContentTemplate>
            </asp:UpdatePanel>            
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel runat="server" ID="ArchiveTab" OnClientClick="ArchiveTabRefresh">
        <ContentTemplate>
            <asp:UpdatePanel ID="ArchiveTabUpdatePanel" runat="server">
            <ContentTemplate>        
                <YAF:PMList runat="server" View="Archive" ID="ArchivePMList" />
            </ContentTemplate>
            </asp:UpdatePanel>            
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
<br />
<div>
    <YAF:ThemeButton ID="NewPM2" runat="server" CssClass="yafcssbigbutton" TextLocalizedTag="BUTTON_NEWPM"
        TitleLocalizedTag="BUTTON_NEWPM_TT" />
</div>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
