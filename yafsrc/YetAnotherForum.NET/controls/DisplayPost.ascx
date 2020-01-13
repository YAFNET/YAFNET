<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayPost" EnableViewState="false" Codebehind="DisplayPost.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="ServiceStack" %>
<%@ Import Namespace="YAF.Core.Services" %>

<asp:PlaceHolder runat="server" ID="ShowHideIgnoredUserPost" Visible="False">
    <YAF:Alert runat="server" Type="info" Dismissing="True">
        <YAF:ThemeButton ID="btnTogglePost" runat="server" 
                         Type="Info"
                         TextLocalizedPage="POSTS"
                         TextLocalizedTag="TOGGLEPOST"
                         Icon="eye"
                         DataToggle="collapse"
                         DataTarget='<%# this.MessageRow.ClientID %>'/>
    </YAF:Alert>
</asp:PlaceHolder>


<asp:Panel runat="server" ID="MessageRow">
    <div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <div class="row">
                    <div class="col-auto d-none d-md-block">
                        <asp:Image runat="server" ID="Avatar" 
                                   CssClass="rounded img-thumbnail img-avatar-sm" />
                    </div>
                    <div class="col-auto mr-auto">
                        <asp:PlaceHolder runat="server" ID="UserInfo">
                            <ul class="list-inline">
                                <li class="list-inline-item">
                                    <a id="post<%# this.DataRow["MessageID"] %>" 
                                       href='<%# BuildLink.GetLink(ForumPages.posts,"m={0}#post{0}", this.DataRow["MessageID"]) %>'>
                                        #<%# this.CurrentPage * this.Get<BoardSettings>().PostsPerPage + this.PostCount + 1%>
                                    </a>
                                </li>
                                <li class="list-inline-item">
                                    <YAF:UserLink  ID="UserProfileLink" runat="server" 
                                                   UserID='<%# this.PostData.UserId%>'
                                                   ReplaceName='<%#  this.Get<BoardSettings>().EnableDisplayName  ? this.DataRow["DisplayName"] : this.DataRow["UserName"]%>'
                                                   PostfixText='<%# this.DataRow["IP"].ToString() == "NNTP" ? this.GetText("EXTERNALUSER") : string.Empty %>'
                                                   Style='<%# this.DataRow["Style"]%>' 
                                                   EnableHoverCard="False"
                                                   CssClass="dropdown-toggle" />
                                    <YAF:PopMenu runat="server" ID="PopMenu1" Control="UserName" />
                                    <YAF:ThemeButton ID="AddReputation" runat="server" 
                                                     CssClass='<%# "AddReputation_{0} mr-1".Fmt(this.DataRow["UserID"])%>'
                                                     Size="Small"
                                                     Icon="thumbs-up"
                                                     IconColor="text-success"
                                                     Type="None"
                                                     Visible="false" 
                                                     TitleLocalizedTag="VOTE_UP_TITLE"
                                                     OnClick="AddUserReputation">
                                    </YAF:ThemeButton>
                                    <YAF:ThemeButton ID="RemoveReputation" runat="server" 
                                                     CssClass='<%# "RemoveReputation_{0}".Fmt(this.DataRow["UserID"])%>'
                                                     Type="None"
                                                     Size="Small"
                                                     IconColor="text-danger"
                                                     Icon="thumbs-down"
                                                     Visible="false" 
                                                     TitleLocalizedTag="VOTE_DOWN_TITLE"
                                                     OnClick="RemoveUserReputation">
                                    </YAF:ThemeButton>
                                    <asp:Label ID="TopicStarterBadge" runat="server" 
                                               CssClass="badge badge-dark mb-2"
                                               Visible='<%# this.DataRow["TopicOwnerID"].ToType<int>().Equals(this.PostData.UserId) %>'
                                               ToolTip='<%# this.GetText("POSTS","TOPIC_STARTER_HELP") %>'>
                                        <YAF:LocalizedLabel ID="TopicStarterText" runat="server" 
                                                            LocalizedTag="TOPIC_STARTER" 
                                                            LocalizedPage="POSTS" />
                                    </asp:Label>
                                    <asp:Label runat="server" CssClass="badge badge-success" ID="MessageIsAnswerBadge" 
                                               Visible='<%# this.PostData.PostIsAnswer %>'
                                               ToolTip='<%# this.GetText("POSTS","MESSAGE_ANSWER_HELP") %>'>
                                        <i class="fas fa-check fa-fw"></i>
                                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="MESSAGE_ANSWER" LocalizedPage="POSTS" />
                                    </asp:Label>
                                </li>
                                <asp:PlaceHolder runat="server" Visible='<%# this.Get<BoardSettings>().ShowGroups %>'>
                                    <li class="list-inline-item d-none d-md-inline-block"><%# this.GetUserRoles() %></li>
                                </asp:PlaceHolder>
                                
                                <li class="list-inline-item d-none d-md-inline-block"><%# this.GetUserRank() %></li>
                                <asp:PlaceHolder runat="server" ID="UserReputation" Visible='<%#this.Get<BoardSettings>().DisplayPoints && !this.DataRow["IsGuest"].ToType<bool>() %>'>
                                    <li class="d-none d-md-block">
                                        <%# this.Get<IReputation>().GenerateReputationBar(this.DataRow["Points"].ToType<int>(), this.PostData.UserId) %>
                                    </li>
                                </asp:PlaceHolder>
                            </ul>
                        </asp:PlaceHolder>
                    </div>
                    <div class="col-auto">
                        <ul class="list-inline">
                            <li class="list-inline-item d-none d-md-inline-block">
                                <strong><YAF:LocalizedLabel runat="server" LocalizedTag="POSTS"></YAF:LocalizedLabel>:</strong>
                                 <%# this.DataRow["Posts"] %>
                            </li>
                            <li class="list-inline-item d-none d-md-inline-block">
                                <strong><YAF:LocalizedLabel runat="server" LocalizedTag="JOINED"></YAF:LocalizedLabel>:</strong>
                                 <%# this.Get<IDateTime>().FormatDateShort((DateTime)this.DataRow["Joined"]) %>
                            </li>
                            <asp:PlaceHolder runat="server" ID="IPHolder" Visible="False">
                                <li class="list-inline-item">
                                    <asp:Label id="IPInfo" runat="server" 
                                               Visible="false" 
                                               CssClass="d-none d-md-inline-block"> 
                                        &nbsp;&nbsp;
                                        <strong>
                                            <i class="fas fa-laptop fa-fw text-secondary"></i>&nbsp;<%# this.GetText("IP") %>:
                                        </strong>&nbsp;
                                        <a id="IPLink1" href="#" target="_blank" runat="server"></a>			   
                                    </asp:Label> 
                                </li>
                            </asp:PlaceHolder>
                            <li class="list-inline-item">
                                <time>
                                    <span class="fa-stack">
                                        <i class="fa fa-calendar-day fa-stack-1x text-secondary"></i>
                                        <i class="fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse"></i>
                                        <i class="fa fa-clock fa-badge text-secondary"></i>
                                    </span>
                                    <YAF:DisplayDateTime id="DisplayDateTime" runat="server" 
                                                         DateTime='<%# this.DataRow["Posted"] %>'>
                                    </YAF:DisplayDateTime>
                                </time>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <div class="row">
                    <asp:panel id="panMessage" runat="server">
                            <YAF:MessagePostData runat="server"
                                                 DataRow="<%# this.DataRow %>"
                                                 ShowEditMessage="True">
                            </YAF:MessagePostData>
                        </asp:panel>
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="Footer">
                <div class="card-footer">
                <div class="row">
                    <div class="col px-0">
                        <YAF:ThemeButton ID="ReportPost" runat="server"
                                         Visible="false"
                                         Type="Link" 
                                         TextLocalizedPage="POSTS" 
                                         TextLocalizedTag="REPORTPOST"
                                         Icon="exclamation-triangle"
                                         IconColor="text-danger"
                                         TitleLocalizedTag="REPORTPOST_TITLE"
                                         DataToggle="tooltip"
                                         CssClass="text-left" />
                        <YAF:ThemeButton ID="MarkAsAnswer" runat="server" 
                                         Visible="false" 
                                         Type="Link" 
                                         TextLocalizedPage="POSTS" 
                                         TextLocalizedTag="MARK_ANSWER" 
                                         TitleLocalizedTag="MARK_ANSWER_TITLE"
                                         DataToggle="tooltip"
                                         Icon="check-square"
                                         IconColor="text-success"
                                         OnClick="MarkAsAnswerClick" />	
                    </div>
                    <div class="col-auto px-0 d-flex flex-wrap">
                        <div id="<%# "dvThanksInfo{0}".Fmt(this.DataRow["MessageID"]) %>" class="small mt-1">
                            <asp:Literal runat="server" Visible="false" ID="ThanksDataLiteral"></asp:Literal>
                        </div>
                        <div class="btn-toolbar" role="toolbar" aria-label="Toolbar with button groups">
                            <div class="btn-group mr-2" role="group">
                                <span id="<%# "dvThankBox{0}".Fmt(this.DataRow["MessageID"]) %>">
                                    <YAF:ThemeButton ID="Thank" runat="server"
                                                     Type="Link"
                                                     Icon="thumbs-up"
                                                     IconColor="text-primary"
                                                     Visible="false"
                                                     DataToggle="tooltip"
                                                     TextLocalizedTag="BUTTON_THANKS"
                                                     TitleLocalizedTag="BUTTON_THANKS_TT" />
                                </span>
                            </div>
                            <asp:PlaceHolder runat="server" ID="ManageDropPlaceHolder">
                                <div class="btn-group mr-2" role="group">
                                    <YAF:ThemeButton ID="Edit" runat="server"
                                                     Type="Link"
                                                     Icon="edit"
                                                     IconColor="text-secondary"
                                                     DataToggle="tooltip"
                                                     TitleLocalizedTag="BUTTON_EDIT_TT" />
                                    <YAF:ThemeButton ID="MovePost" runat="server"
                                                     Type="Link"
                                                     Icon="arrows-alt"
                                                     IconColor="text-secondary"
                                                     DataToggle="tooltip"
                                                     TitleLocalizedTag="BUTTON_MOVE_TT" />
                                    <YAF:ThemeButton ID="Delete" runat="server"
                                                     Type="Link"
                                                     Icon="trash"
                                                     IconColor="text-secondary"
                                                     DataToggle="tooltip"
                                                     TitleLocalizedTag="BUTTON_DELETE_TT" />
                                    <YAF:ThemeButton ID="UnDelete" runat="server"
                                                     Type="Link"
                                                     IconColor="text-warning"
                                                     Icon="trash-restore"
                                                     DataToggle="tooltip"
                                                     TitleLocalizedTag="BUTTON_UNDELETE_TT" />
                                </div>
                            </asp:PlaceHolder>
                            <div class="btn-group" role="group">
                                <YAF:ThemeButton ID="Quote" runat="server"
                                                 Type="Link"
                                                 Icon="quote-left"
                                                 IconColor="text-primary"
                                                 TextLocalizedTag="BUTTON_QUOTE"
                                                 DataToggle="tooltip"
                                                 TitleLocalizedTag="BUTTON_QUOTE_TT" />
                                <asp:CheckBox runat="server" ID="MultiQuote" 
                                              CssClass="MultiQuoteButton custom-control custom-checkbox btn btn-link" />
                            </div>
                        </div>
                        </div>
                </div>
            </div>
            </asp:PlaceHolder>
        </div>
    </div>
    </div>
</asp:Panel>