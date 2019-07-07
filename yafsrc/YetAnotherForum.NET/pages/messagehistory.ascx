<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.messagehistory"CodeBehind="messagehistory.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="ServiceStack" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-history fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="TITLE" />
            </div>
            <div class="card-body">
                <asp:Repeater ID="RevisionsList" runat="server"  OnItemCommand="RevisionsList_ItemCommand">
        <HeaderTemplate>
            <div class="table-responsive">
            <table class="table">
            <thead>
            <tr>
                <th>
                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="OLD" />
                </th>
                <th>
                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="NEW" />
                </th>
                <th>
                    <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITEREASON" />
                </th>
                <th>
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITEDBY" />
                </th>
                 <th>
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITEDBY_MOD" />
                </th>
                <th>
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedPage="POSTMESSAGE"
                            LocalizedTag="EDITED" />
                </th>
                <th>
                </th>
            </tr>
            </thead>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <asp:RadioButton runat="server" ID="Old" onclick="toggleOldSelection(this);" />
                </td>
                <td>
                    <asp:RadioButton runat="server" ID="New" onclick="toggleNewSelection(this);" />
                </td>
                <td>
                    <asp:HiddenField runat="server" Value='<%#
    this.FormatMessage((System.Data.DataRow)Container.DataItem)%>' ID="MessageField" />
                    <%# Container.DataItemToField<DateTime>("Edited") != Container.DataItemToField<DateTime>("Posted") ? Container.DataItemToField<string>("EditReason").IsNotSet() ? this.GetText("EDIT_REASON_NA") : Container.DataItemToField<string>("EditReason"): this.GetText("ORIGINALMESSAGE") %>
                    <%# Container.ItemIndex.Equals(this.RevisionsCount-1) ? "({0})".Fmt(this.GetText("MESSAGEHISTORY", "CURRENTMESSAGE")) : string.Empty %>
                </td>
                <td>
                    <YAF:UserLink ID="UserLink3" runat="server" UserID='<%# Container.DataItemToField<int>("EditedBy") %>' />
                    <br />
                    <span id="IPSpan1" runat="server" visible='<%#
    this.PageContext.IsAdmin || this.Get<YafBoardSettings>().AllowModeratorsViewIPs && this.PageContext.ForumModeratorAccess%>'>
                        <strong>
                            <%# this.GetText("IP") %>:</strong><a id="IPLink1" href='<%# string.Format(this.Get<YafBoardSettings>().IPInfoPageURL, IPHelper.GetIp4Address(Container.DataItemToField<string>("IP"))) %>'
                                title='<%# this.GetText("COMMON","TT_IPDETAILS") %>'
                                target="_blank" runat="server"><%# IPHelper.GetIp4Address(Container.DataItemToField<string>("IP")) %></a>
                    </span>
                </td>
                <td>
                    <%# Container.DataItemToField<bool>("IsModeratorChanged") ?  this.GetText("YES") : this.GetText("NO") %>
                </td>
                <td><%# this.Get<IDateTime>().FormatDateTimeTopic( Container.DataItemToField<DateTime>("Edited") ) %></td>
                <td>
                    <YAF:ThemeButton ID="ThemeButtonEdit" runat="server"
                        CommandName='restore' CommandArgument='<%# Container.DataItemToField<DateTime>("Edited") %>' 
                        TitleLocalizedTag="RESTORE_MESSAGE" TextLocalizedTag="RESTORE_MESSAGE"
                        Visible='<%# (this.PageContext.IsAdmin || this.PageContext.IsModeratorInAnyForum) && !Container.ItemIndex.Equals(this.RevisionsCount-1) %>'
                        OnLoad="RestoreVersion_Load"
                                     Type="Secondary" Size="Small" Icon="undo">
                    </YAF:ThemeButton>
                </td>
            </tr>
        </ItemTemplate>
                    <FooterTemplate>
                        </table>
                        </div>
                    </FooterTemplate>
    </asp:Repeater>
            </div>
            <div class="card-footer text-center">
                        <a onclick="RenderMessageDiff('<%# this.GetText("MESSAGEHISTORY","MESSAGE_EDITEDAT") %>','<%# this.GetText("MESSAGEHISTORY","NOTHING_SELECTED") %>','<%# this.GetText("MESSAGEHISTORY","SELECT_BOTH") %>','<%# this.GetText("MESSAGEHISTORY","SELECT_DIFFERENT") %>');" 
                           class="btn btn-primary mt-1">
                            <span style="color: white">
                                <%# this.GetText("MESSAGEHISTORY","COMPARE_VERSIONS") %>
                            </span>
                        </a>            
                        <YAF:ThemeButton ID="ReturnBtn" 
                                         CssClass="mt-1"
                                         OnClick="ReturnBtn_OnClick"
                                         TextLocalizedTag="TOMESSAGE" 
                                         Visible="false" 
                                         Type="Secondary"
                                         Icon="external-link-square-alt"
                                         runat="server">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ReturnModBtn"  
                                         CssClass="mt-1"
                                         OnClick="ReturnModBtn_OnClick"
                                         TextLocalizedTag="GOMODERATE" 
                                         Visible="false" 
                                         Type="Secondary"
                                         Icon="external-link-square-alt"
                                         runat="server">
                        </YAF:ThemeButton>
            </div>
        </div>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-history fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                                             LocalizedTag="COMPARE_TITLE" />
            </div>
            <div class="card-body">
                <h6 class="card-subtitle mb-2 text-muted">
                    <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="TEXT_CHANGES" />
                </h6>
                <div id="diffContent">
                    <YAF:Alert runat="server" Type="warning">
                        <%# this.GetText("MESSAGEHISTORY","SELECT_DIFFERENT") %>
                    </YAF:Alert>
                </div>
            </div>
        </div>
    </div>
</div>