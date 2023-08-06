<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.MessageHistory" CodeBehind="MessageHistory.ascx.cs" %>

<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>
<%@ Import Namespace="ServiceStack.Text" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                LocalizedTag="TITLE" />
        </h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="history"/>
            </div>
            <div class="card-body">
                <asp:Repeater ID="RevisionsList" runat="server"
                              OnItemCommand="RevisionsList_ItemCommand">
                    <HeaderTemplate>
                        <ul class="list-group">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item list-group-item-action">
                            <div class="d-flex w-100 justify-content-between">
                                     <h5 class="mb-1">
                                         <div class="form-check d-inline-block">
                                             <asp:Checkbox runat="server" ID="Compare" 
                                                           onclick="toggleSelection(this);" 
                                                           Text="&nbsp;" />
                                         </div>
                                         <asp:HiddenField runat="server" 
                                                          Value="<%# this.HtmlEncode((Container.DataItem as MessageHistoryTopic).Message)%>" ID="MessageField" />
                                         <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" 
                                                             LocalizedPage="POSTMESSAGE"
                                                             LocalizedTag="EDITEREASON" />: <%# (Container.DataItem as MessageHistoryTopic).Edited != (Container.DataItem as MessageHistoryTopic).Posted  ? (Container.DataItem as MessageHistoryTopic).EditReason.IsNotSet() ? this.GetText("EDIT_REASON_NA") : (Container.DataItem as MessageHistoryTopic).EditReason: this.GetText("ORIGINALMESSAGE") %>
                                         <%# Container.ItemIndex.Equals(this.RevisionsCount-1) ? "({0})".Fmt(this.GetText("MESSAGEHISTORY", "CURRENTMESSAGE")) : string.Empty %>
                                     </h5>
                                     <small class="d-none d-md-block">
                                         <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                             LocalizedPage="POSTMESSAGE" 
                                                             LocalizedTag="EDITED" />: <%# this.Get<IDateTimeService>().FormatDateTimeTopic((Container.DataItem as MessageHistoryTopic).Edited) %>
                                     </small>
                                 </div>
                            <p class="mb-1">
                                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                        LocalizedPage="POSTMESSAGE"
                                                        LocalizedTag="EDITEDBY" />: <YAF:UserLink ID="UserLink3" runat="server"
                                                                                                  ReplaceName="<%# this.PageBoardContext.BoardSettings.EnableDisplayName ? (Container.DataItem as MessageHistoryTopic).DisplayName : (Container.DataItem as MessageHistoryTopic).Name %>"
                                                                                                  Suspended="<%# (Container.DataItem as MessageHistoryTopic).Suspended %>"
                                                                                                  Style="<%# (Container.DataItem as MessageHistoryTopic).UserStyle %>"
                                                                                                  UserID="<%# (Container.DataItem as MessageHistoryTopic).EditedBy %>" />
                                    <asp:PlaceHolder runat="server" Visible="<%# this.PageBoardContext.IsAdmin || this.PageBoardContext.BoardSettings.AllowModeratorsViewIPs && this.PageBoardContext.ForumModeratorAccess%>">
                                        <span class="fw-bold me-2">
                                            <%# this.GetText("IP") %>:
                                        </span><a id="IPLink1" 
                                                  href="<%# string.Format(this.PageBoardContext.BoardSettings.IPInfoPageURL, this.GetIpAddress(Container.DataItem as MessageHistoryTopic)) %>"
                                                  title='<%# this.GetText("COMMON","TT_IPDETAILS") %>'
                                                  target="_blank" runat="server"><%# this.GetIpAddress(Container.DataItem as MessageHistoryTopic) %></a>
                                    </asp:PlaceHolder>
                                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                        LocalizedPage="POSTMESSAGE"
                                                        LocalizedTag="EDITEDBY_MOD" />: <span class="badge text-bg-secondary"><%# (Container.DataItem as MessageHistoryTopic).IsModeratorChanged.Value ?  this.GetText("YES") : this.GetText("NO") %></span>
                                </p>
                            <small>
                                <YAF:ThemeButton ID="ThemeButtonEdit" runat="server"
                                                     CommandName="restore" 
                                                     CommandArgument='<%# (Container.DataItem as MessageHistoryTopic).Edited.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) %>'
                                                     TitleLocalizedTag="RESTORE_MESSAGE" 
                                                     TextLocalizedTag="RESTORE_MESSAGE"
                                                     Visible="<%# (this.PageBoardContext.IsAdmin || this.PageBoardContext.IsModeratorInAnyForum) && !Container.ItemIndex.Equals(this.RevisionsCount-1) %>"
                                                     ReturnConfirmTag="CONFIRM_RESTORE"
                                                     Type="Secondary" 
                                                     Size="Small" 
                                                     Icon="undo">
                                    </YAF:ThemeButton>
                                </small>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton runat="server" ID="ShowDif" 
                                 CssClass="mb-1"
                                 TextLocalizedTag="COMPARE_VERSIONS"
                                 Icon="equals"
                                 OnClick="ShowDiffClick"></YAF:ThemeButton>
                <YAF:ThemeButton ID="ReturnBtn" 
                                 CssClass="mb-1"
                                 OnClick="ReturnBtn_OnClick"
                                 TextLocalizedTag="TOMESSAGE" 
                                 Visible="false" 
                                 Type="Secondary"
                                 Icon="external-link-square-alt"
                                 runat="server">
                </YAF:ThemeButton>
                <YAF:ThemeButton ID="ReturnModBtn"  
                                 CssClass="mb-1"
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
                <YAF:IconHeader runat="server"
                                IconName="history"
                                LocalizedTag="COMPARE_TITLE"/>
            </div>
            <div class="card-body">
                <h6 class="card-subtitle mb-2 text-body-secondary">
                    <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" 
                                        LocalizedTag="TEXT_CHANGES" />
                </h6>
                <div id="diffContent">
                    <YAF:Alert runat="server" Type="info" ID="InfoSelect">
                        <YAF:Icon runat="server" IconName="info-circle" />
                        <%# this.GetText("MESSAGEHISTORY","SELECT_DIFFERENT") %>
                    </YAF:Alert>
                    <asp:Literal runat="server" ID="DiffView"></asp:Literal>
                </div>
            </div>
        </div>
    </div>
</div>