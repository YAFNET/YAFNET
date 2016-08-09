<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.cp_pm" Codebehind="cp_pm.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="PMList" Src="../controls/PMList.ascx" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<div>
	<YAF:ThemeButton ID="NewPM" runat="server" CssClass="yafcssbigbutton rightItem button-newpm" TextLocalizedTag="BUTTON_NEWPM" TitleLocalizedTag="BUTTON_NEWPM_TT" />
</div>
<br style="clear: both" />
<asp:Panel id="PmTabs" runat="server">
               <ul class="nav nav-tabs" role="tablist">
                 <li class="nav-item">
                     <a href="#View1" class="nav-link" data-toggle="tab" role="tab">
                         <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="INBOX" />
                     </a>
                 </li>
		         <li class="nav-item">
		             <a href="#View2" class="nav-link" data-toggle="tab" role="tab">
		                 <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="SENTITEMS" />
		             </a>
		         </li>
		         <li class="nav-item">
		             <a href="#View3" class="nav-link" data-toggle="tab" role="tab">
		                 <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ARCHIVE" />
		             </a>
		         </li>
               </ul>
              <div class="tab-content">
                <div id="View1" class="tab-pane" role="tabpanel">
                  <asp:UpdatePanel ID="InboxTabUpdatePanel" runat="server">
            <ContentTemplate>
							<YAF:PMList runat="server" View="Inbox" ID="InboxPMList" />
            </ContentTemplate>
            </asp:UpdatePanel>
                </div>
                <div id="View2" class="tab-pane" role="tabpanel">
                  <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
							<YAF:PMList runat="server" View="Outbox" ID="OutboxPMList" />
            </ContentTemplate>
            </asp:UpdatePanel>
                </div>
                <div id="View3" class="tab-pane" role="tabpanel">
                  <asp:UpdatePanel ID="ArchiveTabUpdatePanel" runat="server">
            <ContentTemplate>
							<YAF:PMList runat="server" View="Archive" ID="ArchivePMList" />
            </ContentTemplate>
            </asp:UpdatePanel>
                </div>
             </div>
             </asp:Panel>
<asp:HiddenField runat="server" ID="hidLastTab" Value="View1" />
<br />
<div>
	<YAF:ThemeButton ID="NewPM2" runat="server" CssClass="yafcssbigbutton rightItem button-newpm" TextLocalizedTag="BUTTON_NEWPM" TitleLocalizedTag="BUTTON_NEWPM_TT" />
</div>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
