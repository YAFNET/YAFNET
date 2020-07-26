<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayPost" EnableViewState="false" Codebehind="DisplayPost.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="ServiceStack" %>
<%@ Import Namespace="System.Data" %>

<asp:PlaceHolder runat="server" ID="ShowHideIgnoredUserPost" Visible="False">
    <YAF:Alert runat="server" Type="info" Dismissing="True">
        <YAF:ThemeButton ID="btnTogglePost" runat="server" 
                         Type="Info"
                         TextLocalizedPage="POSTS"
                         TextLocalizedTag="TOGGLEPOST"
                         Icon="eye"
                         DataToggle="collapse"
                         DataTarget="<%# this.MessageRow.ClientID %>"/>
    </YAF:Alert>
</asp:PlaceHolder>


<asp:Panel runat="server" ID="MessageRow">
    <div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header py-1 px-2">
                <div class="d-flex">
                    <div class="mr-2" style="flex:0 0 2.33333%">
                        <asp:Image runat="server" ID="Avatar" 
                                   CssClass="img-avatar-sm mt-2" />
                    </div>
                    <div>
                        <asp:PlaceHolder runat="server" ID="UserInfo">
                            <ul class="list-inline">
                                <li class="list-inline-item">
                                    <YAF:UserLink  ID="UserProfileLink" runat="server" />
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
                                    
                                </li>
                                <asp:PlaceHolder runat="server" ID="IPHolder" Visible="False">
                                    <li class="list-inline-item d-none d-md-inline-block">
                                        <asp:Label id="IPInfo" runat="server" 
                                                   Visible="false" 
                                                   CssClass="badge badge-info">
                                            <%# this.GetText("IP") %>:&nbsp;
                                            <a id="IPLink1" href="#" target="_blank" runat="server" class="text-white"></a>			   
                                        </asp:Label> 
                                    </li>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" ID="UserReputation" Visible='<%#this.Get<BoardSettings>().DisplayPoints && !this.DataRow.Field<bool>("IsGuest") %>'>
                                    <li class="list-inline-item d-none d-md-inline-block" style="width:150px">
                                        <%# this.Get<IReputation>().GenerateReputationBar(this.DataRow.Field<int>("Points"), this.PostData.UserId) %>
                                    </li>
                                </asp:PlaceHolder>
                                <li class="list-inline-item d-block">
                                    <span class="badge badge-secondary"><%# this.DataRow["RankName"]%></span>
                                    <asp:Label ID="TopicStarterBadge" runat="server" 
                                           CssClass="badge badge-dark mb-2"
                                           Visible='<%# this.DataRow.Field<int>("TopicOwnerID").Equals(this.PostData.UserId) %>'
                                           ToolTip='<%# this.GetText("POSTS","TOPIC_STARTER_HELP") %>'>
                                    <YAF:LocalizedLabel ID="TopicStarterText" runat="server" 
                                                        LocalizedTag="TOPIC_STARTER" 
                                                        LocalizedPage="POSTS" />
                                    </asp:Label>
                                    <asp:Label runat="server" CssClass="badge badge-success" ID="MessageIsAnswerBadge" 
                                           Visible="<%# this.PostData.PostIsAnswer %>"
                                           ToolTip='<%# this.GetText("POSTS","MESSAGE_ANSWER_HELP") %>'>
                                    <i class="fas fa-check fa-fw"></i>
                                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="MESSAGE_ANSWER" LocalizedPage="POSTS" />
                                    </asp:Label>
                                </li>
                            </ul>
                        </asp:PlaceHolder>
                    </div>
                    <asp:Panel runat="server" CssClass="ml-auto" id="ToolsHolder">
                        <YAF:ThemeButton ID="Tools1" runat="server"
                                         Type="Link"
                                         DataToggle="dropdown"
                                         TitleLocalizedTag="TOOLS"
                                         Icon="ellipsis-h" />
                        <div class="dropdown-menu dropdown-menu-right dropdown-menu-lg-left">
                            <YAF:ThemeButton ID="MarkAsAnswer" runat="server" 
                                             Visible="false" 
                                             Type="None" 
                                             TextLocalizedPage="POSTS" 
                                             TextLocalizedTag="MARK_ANSWER" 
                                             TitleLocalizedTag="MARK_ANSWER_TITLE"
                                             DataToggle="tooltip"
                                             Icon="check-square"
                                             OnClick="MarkAsAnswerClick"
                                             CssClass="dropdown-item" />
                            <asp:PlaceHolder runat="server" ID="ManageDropPlaceHolder">
                                <YAF:ThemeButton ID="Edit" runat="server"
                                                 Type="None"
                                                 Icon="edit"
                                                 DataToggle="tooltip"
                                                 TextLocalizedTag="BUTTON_EDIT"
                                                 TitleLocalizedTag="BUTTON_EDIT_TT"
                                                 CssClass="dropdown-item" />
                                <YAF:ThemeButton ID="MovePost" runat="server"
                                                 Type="None"
                                                 Icon="arrows-alt"
                                                 DataToggle="tooltip"
                                                 TextLocalizedTag="BUTTON_MOVE"
                                                 TitleLocalizedTag="BUTTON_MOVE_TT"
                                                 CssClass="dropdown-item" />
                                <YAF:ThemeButton ID="Delete" runat="server"
                                                 Type="None"
                                                 Icon="trash"
                                                 DataToggle="tooltip"
                                                 TextLocalizedTag="BUTTON_DELETE"
                                                 TitleLocalizedTag="BUTTON_DELETE_TT"
                                                 CssClass="dropdown-item" />
                                <YAF:ThemeButton ID="UnDelete" runat="server"
                                                 Type="None"
                                                 Icon="trash-restore"
                                                 DataToggle="tooltip"
                                                 TextLocalizedTag="BUTTON_UNDELETE"
                                                 TitleLocalizedTag="BUTTON_UNDELETE_TT"
                                                 CssClass="dropdown-item" />
                            <div class="dropdown-divider"></div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" ID="UserDropHolder"></asp:PlaceHolder>
                            <YAF:ThemeButton ID="ReportPost" runat="server"
                                             Visible="false"
                                             Type="None" 
                                             TextLocalizedPage="POSTS" 
                                             TextLocalizedTag="REPORTPOST"
                                             Icon="exclamation-triangle"
                                             TitleLocalizedTag="REPORTPOST_TITLE"
                                             DataToggle="tooltip"
                                             CssClass="dropdown-item" />
                        </div>
                    </asp:Panel>
                </div>
            </div>
            <div class="card-body pt-0">
                <div class="d-flex justify-content-between border-bottom mb-3">
                    <div>
                        <YAF:Icon runat="server" 
                                      IconName="calendar-day"
                                      IconType="text-secondary"
                                      IconNameBadge="clock" 
                                      IconBadgeType="text-secondary"></YAF:Icon>
                            <YAF:DisplayDateTime id="DisplayDateTime" runat="server" 
                                                 DateTime='<%# this.DataRow["Posted"] %>'>
                            </YAF:DisplayDateTime>
                    </div>
                    <div style="margin-top: 1px">
                        <a id="post<%# this.DataRow["MessageID"] %>" 
                           href='<%# BuildLink.GetLink(ForumPages.Posts,"m={0}#post{0}", this.DataRow["MessageID"]) %>'>
                            #<%# this.CurrentPage * this.Get<BoardSettings>().PostsPerPage + this.PostCount + 1%>
                        </a>
                    </div>
                </div>
                <div class="row message">
                    <asp:panel id="panMessage" runat="server">
                            <YAF:MessagePostData runat="server"
                                                 DataRow="<%# this.DataRow %>"
                                                 ShowEditMessage="True">
                            </YAF:MessagePostData>
                    </asp:panel>
                    <div class="dropdown-menu context-menu" aria-labelledby="context menu" ID="ContextMenu" runat="server">
                        <YAF:ThemeButton ID="Quote2" runat="server"
                                         Type="None"
                                         Icon="quote-left"
                                         TextLocalizedTag="BUTTON_QUOTE"
                                         DataToggle="tooltip"
                                         TitleLocalizedTag="BUTTON_QUOTE_TT"
                                         CssClass="dropdown-item" />
                        <YAF:ThemeButton ID="QuickReplyLink" runat="server"
                                         Type="None"
                                         TextLocalizedTag="QUICKREPLY" TitleLocalizedTag="BUTTON_POSTREPLY_TT"
                                         Icon="reply" 
                                         DataToggle="modal"
                                         DataTarget="QuickReplyDialog"
                                         CssClass="dropdown-item" />
                        <YAF:ThemeButton ID="Reply" runat="server"
                                         Type="None"
                                         TextLocalizedTag="REPLY" TitleLocalizedTag="BUTTON_POSTREPLY_TT"
                                         Icon="reply"
                                         CssClass="dropdown-item"  />
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="Edit2" runat="server"
                                         Type="None"
                                         Icon="edit"
                                         DataToggle="tooltip"
                                         TextLocalizedTag="BUTTON_EDIT"
                                         TitleLocalizedTag="BUTTON_EDIT_TT"
                                         CssClass="dropdown-item"  />
                        <YAF:ThemeButton ID="Move" runat="server"
                                         Type="None"
                                         Icon="arrows-alt"
                                         DataToggle="tooltip"
                                         TextLocalizedTag="BUTTON_MOVE"
                                         TitleLocalizedTag="BUTTON_MOVE_TT"
                                         CssClass="dropdown-item" />
                        <YAF:ThemeButton ID="Delete2" runat="server"
                                         Type="None"
                                         Icon="trash"
                                         DataToggle="tooltip"
                                         TextLocalizedTag="BUTTON_DELETE"
                                         TitleLocalizedTag="BUTTON_DELETE_TT"
                                         CssClass="dropdown-item" />
                        <YAF:ThemeButton ID="UnDelete2" runat="server"
                                         Type="None"
                                         Icon="trash-restore"
                                         DataToggle="tooltip"
                                         TextLocalizedTag="BUTTON_UNDELETE"
                                         TitleLocalizedTag="BUTTON_UNDELETE_TT"
                                         CssClass="dropdown-item" />
                        <div class="dropdown-divider"></div>
                        <asp:PlaceHolder runat="server" ID="UserDropHolder2"></asp:PlaceHolder>
                        <YAF:ThemeButton ID="ReportPost2" runat="server"
                                         Visible="false"
                                         Type="None" 
                                         TextLocalizedPage="POSTS" 
                                         TextLocalizedTag="REPORTPOST"
                                         Icon="exclamation-triangle"
                                         TitleLocalizedTag="REPORTPOST_TITLE"
                                         DataToggle="tooltip"
                                         CssClass="dropdown-item" />
                        
                    </div>
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="Footer">
                <div class="card-footer py-0">
                <div class="row">
                    <div class="col px-0">
                        <div id="<%# "dvThanksInfo{0}".Fmt(this.DataRow["MessageID"]) %>" class="small d-inline-flex">
                            <asp:Literal runat="server" Visible="false" ID="ThanksDataLiteral"></asp:Literal>
                        </div>
                        <span id="<%# "dvThankBox{0}".Fmt(this.DataRow["MessageID"]) %>">
                            <YAF:ThemeButton ID="Thank" runat="server"
                                             Type="Link"
                                             Icon="thumbs-up"
                                             IconColor="text-primary"
                                             Visible="false"
                                             DataToggle="tooltip"
                                             TitleLocalizedTag="BUTTON_THANKS_TT" />
                        </span>
                    </div>
                    <div class="col-auto px-0 d-flex flex-wrap">
                        <div class="btn-toolbar" role="toolbar" aria-label="Toolbar with button groups">
                            <div class="btn-group" role="group">
                                <YAF:ThemeButton ID="ReplyFooter" runat="server"
                                                 Type="Link"
                                                 TitleLocalizedTag="BUTTON_POSTREPLY_TT"
                                                 Icon="reply"
                                                 IconColor="text-primary"
                                                 DataToggle="tooltip"  />
                                <YAF:ThemeButton ID="Quote" runat="server"
                                                 Type="Link"
                                                 Icon="quote-left"
                                                 IconColor="text-primary"
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