<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.MyMessages" Codebehind="MyMessages.ascx.cs" %>
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
                                 DataToggle="tooltip"
                                 Type="Secondary"
                                 Icon="envelope-open-text"/>
            </div>
        </div>
        <div class="row mt-3">
            <div class="col">
        <asp:Panel id="PmTabs" runat="server">
            <ul class="nav nav-tabs" role="tablist">
                 <li class="nav-item">
                     <a href="#View0" class="nav-link" data-bs-toggle="tab" role="tab">
                         <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="INBOX" />
                     </a>
                 </li>
                 <li class="nav-item">
                     <a href="#View1" class="nav-link" data-bs-toggle="tab" role="tab">
                         <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="SENTITEMS" />
                     </a>
                 </li>
            </ul>
            <div class="tab-content">
                <div id="View0" class="tab-pane" role="tabpanel">
                    <div class="card mb-3">
                        <YAF:PMList runat="server" ID="InboxPMList"
                                    View="Inbox" />
                        <div class="card-footer">
                            <small class="text-body-secondary">
                                <asp:Label ID="InfoInbox" runat="server"/>
                            </small>
                        </div>
                    </div>
                </div>
                <div id="View1" class="tab-pane" role="tabpanel">
                    <div class="card mb-3">
                        <YAF:PMList runat="server" ID="OutboxPMList"
                                    View="Outbox" />
                        <div class="card-footer">
                            <small class="text-body-secondary">
                                <asp:Label ID="InfoOutbox" runat="server"/>
                            </small>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:HiddenField runat="server" ID="hidLastTab"
                         Value="View0" />
    </div>
        </div>
        <div class="row">
            <div class="col">
                <YAF:ThemeButton ID="NewPM2" runat="server"
                                 TextLocalizedTag="BUTTON_NEWPM" TitleLocalizedTag="BUTTON_NEWPM_TT"
                                 DataToggle="tooltip"
                                 Type="Secondary"
                                 Icon="envelope-open-text"/>
            </div>
        </div>
    </div>
</div>

