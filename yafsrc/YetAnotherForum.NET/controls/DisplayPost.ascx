<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayPost" EnableViewState="false" Codebehind="DisplayPost.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="DisplayPostFooter" Src="DisplayPostFooter.ascx" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>


<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <span class="float-left">
                    <a onclick="ScrollToTop();" class="postTopLink" href="javascript: void(0)">            
                        <i class="fa fa-angle-double-up fa-fw"></i>
                    </a>
                    <a id="post<%# this.DataRow["MessageID"] %>" 
                       href='<%# YafBuildLink.GetLink(ForumPages.posts,"m={0}#post{0}", this.DataRow["MessageID"]) %>'>
                        #<%# this.CurrentPage * this.Get<YafBoardSettings>().PostsPerPage + this.PostCount + 1%>
                    </a>
                    <asp:Label runat="server" CssClass="badge badge-success" ID="MessageIsAnswerBadge" 
                               Visible='<%# this.PostData.PostIsAnswer %>'
                               ToolTip='<%# this.GetText("POSTS","MESSAGE_ANSWER_HELP") %>'>
                        <i class="fa fa-check fa-fw"></i>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="MESSAGE_ANSWER" LocalizedPage="POSTS" />
                    </asp:Label>
                </span>
                <span id="IPSpan1" runat="server" visible="false" class="float-right d-none d-md-block"> 
                    &nbsp;&nbsp;
                    <strong><i class="fa fa-laptop fa-fw"></i>&nbsp;<%# this.GetText("IP") %>:</strong>&nbsp;<a id="IPLink1" target="_blank" runat="server"></a>			   
                </span> 
                <time class="float-right">
                    <i class="fa fa-calendar-alt fa-fw"></i>
                    <YAF:DisplayDateTime id="DisplayDateTime" runat="server" 
                                         DateTime='<%# this.DataRow["Posted"] %>'>
                    </YAF:DisplayDateTime>
                </time>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-9">
                        <asp:panel id="panMessage" runat="server">
                            <YAF:MessagePostData runat="server"
                                                 DataRow="<%# this.DataRow %>" 
                                                 IsAltMessage="<%# this.IsAlt %>"
                                                 ShowEditMessage="True">
                            </YAF:MessagePostData>
                        </asp:panel> 
                    </div>
                    <div class="col-md-3">
                            <div class="card card-body mb-3 text-center">
                                <YAF:UserLink  ID="UserProfileLink" runat="server" 
                                               UserID='<%# this.DataRow["UserID"].ToType<int>()%>'
                                               ReplaceName='<%# this.Get<YafBoardSettings>().EnableDisplayName && (!this.DataRow["IsGuest"].ToType<bool>() || 
                                                                                                                   this.DataRow["IsGuest"].ToType<bool>() && this.DataRow["DisplayName"].ToString() == this.DataRow["UserName"].ToString()) ? this.DataRow["DisplayName"] : this.DataRow["UserName"]%>'
                                               PostfixText='<%# this.DataRow["IP"].ToString() == "NNTP" ? this.GetText("EXTERNALUSER") : String.Empty %>'
                                               Style='<%# this.DataRow["Style"]%>' 
                                               EnableHoverCard="False" 
                                               Suspended='<%# this.DataRow["Suspended"] != DBNull.Value && this.DataRow["Suspended"].ToType<DateTime>() > DateTime.UtcNow %>'
                                               CssClass="dropdown-toggle" />
                                <YAF:PopMenu runat="server" ID="PopMenu1" Control="UserName" />
                                &nbsp;<YAF:ThemeButton ID="AddReputation" runat="server" 
                                                       CssClass='<%# "AddReputation_" + this.DataRow["UserID"]%>'
                                                       ImageThemeTag="VOTE_UP" 
                                                       Visible="false" 
                                                       TitleLocalizedTag="VOTE_UP_TITLE"
                                                       OnClick="AddUserReputation">
                                </YAF:ThemeButton>
                                <YAF:ThemeButton ID="RemoveReputation" runat="server" 
                                                 CssClass='<%# "RemoveReputation_" + this.DataRow["UserID"]%>'
                                                 ImageThemeTag="VOTE_DOWN" 
                                                 Visible="false" 
                                                 TitleLocalizedTag="VOTE_DOWN_TITLE"
                                                 OnClick="RemoveUserReputation">
                                </YAF:ThemeButton>
                                <asp:Label ID="TopicStarterBadge" runat="server" 
                                           CssClass="topicStarter badge badge-dark mb-2"
                                           Visible='<%# this.DataRow["TopicOwnerID"].ToType<int>().Equals(this.DataRow["UserID"].ToType<int>()) %>'
                                           ToolTip='<%# this.GetText("POSTS","TOPIC_STARTER_HELP") %>'>
                                    <YAF:LocalizedLabel ID="TopicStarterText" runat="server" 
                                                        LocalizedTag="TOPIC_STARTER" 
                                                        LocalizedPage="POSTS" />
                                </asp:Label>
                                <ul class="list-group list-group-flush">
                                    <YAF:UserBox id="UserBox1" runat="server"
                                                 Visible="<%# !this.PostData.IsSponserMessage %>" 
                                                 PageCache="<%# this.PageContext.CurrentForumPage.PageCache %>" 
                                                 DataRow='<%# this.DataRow %>'></YAF:UserBox>
                                </ul>
                            </div>
                    </div>
                </div>
            </div>
            <div class="card-footer">
                <div class="row">
                    <div class="col-md-2">
                        <YAF:DisplayPostFooter id="PostFooter" runat="server" 
                                               DataRow="<%# this.DataRow %>">
                        </YAF:DisplayPostFooter>
                    </div>
                    <div class="col-md-6">
                        <div id="<%# "dvThanksInfo" + this.DataRow["MessageID"] %>" class="ThanksInfo">
                            <asp:Literal runat="server"  Visible="false" ID="ThanksDataLiteral"></asp:Literal></div><div id="<%# "dvThanks" + this.DataRow["MessageID"] %>" class="ThanksList">
                            <asp:Literal runat="server" Visible="false" ID="thanksDataExtendedLiteral"></asp:Literal>
                        </div>
                        
                    </div>
                <div class="col-md-4">
                    <YAF:ThemeButton ID="Retweet" runat="server" 
                                     Type="Link"
                                     Icon="retweet"
                                     TextLocalizedTag="BUTTON_RETWEET"
                                     TitleLocalizedTag="BUTTON_RETWEET_TT" 
                                     OnClick="Retweet_Click" />
                    <span id="<%# "dvThankBox" + this.DataRow["MessageID"] %>">
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
                                     Icon="comment-alt"
                                     TextLocalizedTag="BUTTON_QUOTE"
                                     TitleLocalizedTag="BUTTON_QUOTE_TT" />

                    <asp:CheckBox runat="server" ID="MultiQuote" CssClass="MultiQuoteButton btn btn-link"  />
                </div>
                </div>
            </div>
        </div>
    </div>
</div>