<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.PM" Codebehind="PM.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="PMList" Src="../controls/PMList.ascx" %>

<YAF:PageLinks ID="PageLinks" runat="server" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>

    <div class="col">
    
    <div class="row">
        <div class="col">
            <YAF:ThemeButton ID="NewPM" runat="server" 
                             TextLocalizedTag="BUTTON_NEWPM" TitleLocalizedTag="BUTTON_NEWPM_TT"
                             Type="Secondary"
                             Icon="envelope-open-text"/>
        </div>
    </div>

<div class="row mt-3">
    <div class="col">
        <asp:Panel id="PmTabs" runat="server">
            <ul class="nav nav-tabs" role="tablist">
                 <li class="nav-item">
                     <a href="#View0" class="nav-link" data-toggle="tab" role="tab">
                         <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="INBOX" />
                     </a>
                 </li>
		         <li class="nav-item">
		             <a href="#View1" class="nav-link" data-toggle="tab" role="tab">
		                 <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="SENTITEMS" />
		             </a>
		         </li>
		         <li class="nav-item">
		             <a href="#View2" class="nav-link" data-toggle="tab" role="tab">
		                 <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ARCHIVE" />
		             </a>
		         </li>
            </ul>
            <div class="tab-content">
                <div id="View0" class="tab-pane" role="tabpanel">
                  <asp:UpdatePanel ID="InboxTabUpdatePanel" runat="server">
            <ContentTemplate>
                <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-inbox fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="INBOX" />
                </div>
                <div class="card-body">
							<YAF:PMList runat="server" View="Inbox" ID="InboxPMList" />
                </div>
            </ContentTemplate>
            </asp:UpdatePanel>
                   
                </div>
                <div id="View1" class="tab-pane" role="tabpanel">
                
                  <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-inbox fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="SENTITEMS" />
                </div>
                <div class="card-body">
							<YAF:PMList runat="server" View="Outbox" ID="OutboxPMList" />
                </div>
            </ContentTemplate>
            </asp:UpdatePanel>

                   
                </div>
                <div id="View2" class="tab-pane" role="tabpanel">
                    
                  <asp:UpdatePanel ID="ArchiveTabUpdatePanel" runat="server">
            <ContentTemplate>
                <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-inbox fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="ARCHIVE" />
                </div>
                <div class="card-body">
							<YAF:PMList runat="server" View="Archive" ID="ArchivePMList" />
                </div>
            </ContentTemplate>
            </asp:UpdatePanel>
                </div>

            </div>
        </asp:Panel>
        <asp:HiddenField runat="server" ID="hidLastTab" Value="View0" />
    </div>
</div>
<div class="row">
    <div class="col">
        <YAF:ThemeButton ID="NewPM2" runat="server" 
                         TextLocalizedTag="BUTTON_NEWPM" TitleLocalizedTag="BUTTON_NEWPM_TT"
                         Type="Secondary"
                         Icon="envelope-open-text"/>
    </div>
</div>
    
    </div>
    </div>