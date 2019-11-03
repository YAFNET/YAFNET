<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayPost" EnableViewState="false" Codebehind="DisplayPost.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="DisplayPostFooter" Src="DisplayPostFooter.ascx" %>
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
                                       href='<%# YafBuildLink.GetLink(ForumPages.posts,"m={0}#post{0}", this.DataRow["MessageID"]) %>'>
                                        #<%# this.CurrentPage * this.Get<YafBoardSettings>().PostsPerPage + this.PostCount + 1%>
                                    </a>
                                </li>
                                <li class="list-inline-item">
                                    <YAF:UserLink  ID="UserProfileLink" runat="server" 
                                                   UserID='<%# this.PostData.UserId%>'
                                                   ReplaceName='<%#  this.Get<YafBoardSettings>().EnableDisplayName  ? this.DataRow["DisplayName"] : this.DataRow["UserName"]%>'
                                                   PostfixText='<%# this.DataRow["IP"].ToString() == "NNTP" ? this.GetText("EXTERNALUSER") : string.Empty %>'
                                                   Style='<%# this.DataRow["Style"]%>' 
                                                   EnableHoverCard="False" 
                                                   Suspended='<%# this.DataRow["Suspended"] != DBNull.Value && this.DataRow["Suspended"].ToType<DateTime>() > DateTime.UtcNow %>'
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
                                <asp:PlaceHolder runat="server" Visible='<%# this.Get<YafBoardSettings>().ShowGroups %>'>
                                    <li class="list-inline-item d-none d-md-inline-block"><%# this.GetUserRoles() %></li>
                                </asp:PlaceHolder>
                                
                                <li class="list-inline-item d-none d-md-inline-block"><%# this.GetUserRank() %></li>
                                <asp:PlaceHolder runat="server" ID="UserReputation" Visible='<%#this.Get<YafBoardSettings>().DisplayPoints && !this.DataRow["IsGuest"].ToType<bool>() %>'>
                                    <li class="d-none d-md-block">
                                        <%# YafReputation.GenerateReputationBar(this.DataRow["Points"].ToType<int>(), this.PostData.UserId) %>
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
                                                 IsAltMessage="<%# this.IsAlt %>"
                                                 ShowEditMessage="True">
                            </YAF:MessagePostData>
                        </asp:panel>
                </div>
            </div>
            <div class="card-footer">
                <div class="row">
                    <div class="col-md-2 px-0">
                        <YAF:DisplayPostFooter ID="PostFooter" runat="server"
                            DataRow="<%# this.DataRow %>"></YAF:DisplayPostFooter>
                    </div>
                    <div class="col-md-6 px-2">
                        <div id="<%# "dvThanksInfo{0}".Fmt(this.DataRow["MessageID"]) %>" class="pl-1 small">
                            <asp:Literal runat="server" Visible="false" ID="ThanksDataLiteral"></asp:Literal>
                        </div>
                        <div id="<%# "dvThanks{0}".Fmt(this.DataRow["MessageID"]) %>" class="pl-1 small">
                            <asp:Literal runat="server" Visible="false" ID="thanksDataExtendedLiteral"></asp:Literal>
                        </div>
                    </div>
                    <div class="col-md-4 px-0 d-flex flex-wrap">
                        <span id="<%# "dvThankBox{0}".Fmt(this.DataRow["MessageID"]) %>">
                            <YAF:ThemeButton ID="Thank" runat="server"
                                Type="Link"
                                Icon="thumbs-up"
                                Visible="false"
                                TextLocalizedTag="BUTTON_THANKS"
                                TitleLocalizedTag="BUTTON_THANKS_TT" />
                        </span>
                        <YAF:ThemeButton ID="Manage" runat="server"
                            CssClass="dropdown-toggle"
                            Type="Link"
                            DataToggle="dropdown"
                            TextLocalizedTag="MANAGE_POST"
                            Icon="cogs" />
                        <div class="dropdown-menu" aria-labelledby='<%# this.Manage.ClientID %>'>
                            <YAF:ThemeButton ID="Edit" runat="server"
                                Type="Link"
                                CssClass="dropdown-item"
                                Icon="edit"
                                TextLocalizedTag="BUTTON_EDIT"
                                TitleLocalizedTag="BUTTON_EDIT_TT" />
                            <YAF:ThemeButton ID="MovePost" runat="server"
                                Type="Link"
                                CssClass="dropdown-item"
                                Icon="arrows-alt"
                                TextLocalizedTag="BUTTON_MOVE"
                                TitleLocalizedTag="BUTTON_MOVE_TT" />
                            <YAF:ThemeButton ID="Delete" runat="server"
                                Type="Link"
                                CssClass="dropdown-item"
                                Icon="trash"
                                TextLocalizedTag="BUTTON_DELETE"
                                TitleLocalizedTag="BUTTON_DELETE_TT" />
                            <YAF:ThemeButton ID="UnDelete" runat="server"
                                Type="Link"
                                CssClass="dropdown-item"
                                Icon="trash-alt"
                                TextLocalizedTag="BUTTON_UNDELETE"
                                TitleLocalizedTag="BUTTON_UNDELETE_TT" />
                        </div>
                        <YAF:ThemeButton ID="Quote" runat="server"
                                         Type="Link"
                                         Icon="quote-left"
                                         TextLocalizedTag="BUTTON_QUOTE"
                                         TitleLocalizedTag="BUTTON_QUOTE_TT" />
                        <asp:CheckBox runat="server" ID="MultiQuote" 
                                      CssClass="MultiQuoteButton custom-control custom-checkbox btn btn-link" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    </div>
</asp:Panel>