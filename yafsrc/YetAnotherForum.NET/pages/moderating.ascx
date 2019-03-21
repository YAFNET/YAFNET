<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.moderating" CodeBehind="moderating.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<%@ Register TagPrefix="YAF" TagName="TopicLine" Src="../controls/TopicLine.ascx" %>

<asp:PlaceHolder runat="server" ID="ModerateUsersHolder">
<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-user-secret fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                                                                 LocalizedTag="MEMBERS" 
                                                                                 LocalizedPage="MODERATE" />
            </div>
            <div class="card-body">
                <ul class="list-group list-group-flush">
    <asp:Repeater runat="server" ID="UserList" OnItemCommand="UserList_ItemCommand">
        <ItemTemplate>
            <li class="list-group-item">
                <span class="font-weight-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="USER" LocalizedPage="MODERATE" />:
                </span>
                <%# this.Get<YafBoardSettings>().EnableDisplayName ? this.Eval("Name") : this.Eval("DisplayName") %>
                <span class="font-weight-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ACCEPTED" LocalizedPage="MODERATE" />:
                </span>
                <%# this.Eval("Accepted") %>
                <span class="font-weight-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ACCESSMASK" LocalizedPage="MODERATE" />:
                </span>
                <%# this.Eval("Access") %>
                <YAF:ThemeButton ID="ThemeButtonEdit" runat="server" 
                                 CommandName='edit' CommandArgument='<%# this.Eval("UserID") %>'
                                 TitleLocalizedTag="EDIT" 
                                 Size="Small"
                                 Icon="edit"></YAF:ThemeButton>
                <YAF:ThemeButton ID="ThemeButtonRemove" runat="server"
                                 OnLoad="DeleteUser_Load"
                                 CommandName='remove' CommandArgument='<%#
    this.Eval("UserID") %>' 
                                 TitleLocalizedTag="REMOVE" 
                                 Size="Small"
                                 Type="Danger"
                                 Icon="trash"></YAF:ThemeButton>
            </li>
        </ItemTemplate>
    </asp:Repeater> 
               </ul>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton runat="server" ID="AddUser" 
                                 OnClick="AddUser_Click"
                                 TextLocalizedTag="INVITE" TextLocalizedPage="MODERATE"
                                 Type="Secondary"
                                 Icon="user-check"/>
            </div>
        </div>
    </div>
</div>
</asp:PlaceHolder>

<YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" UsePostBack="True" />

<div class="row">
    <div class="col">
        <div class="card mb-3 mt-3">
            <div class="card-header">
                <i class="fa fa-tasks fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="title" LocalizedPage="MODERATE" />
            </div>
            <div class="card-body">
                <asp:Repeater ID="topiclist" runat="server" OnItemCommand="topiclist_ItemCommand">
                        <ItemTemplate>
                            <YAF:TopicLine runat="server" DataRow="<%# Container.DataItem %>" AllowSelection="true" />
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <hr/>
                        </SeparatorTemplate>
                </asp:Repeater>
            </div>
            <div class="card-footer text-center">
                <!-- Move Topic Button -->

        <button type="button" title="Go to Page..."
                class="btn btn-primary dropdown-toggle"
                data-toggle="dropdown" aria-haspopup="true"
                aria-expanded="false">
            <i class="fa fa-arrows-alt fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" ID="TITLE" LocalizedTag="MOVE" LocalizedPage="MOVETOPIC"></YAF:LocalizedLabel>
        </button>
        <div class="dropdown-menu">
            <form class="px-4 py-3">
                <div class="form-group">
                    <label for='<%= this.ForumList.ClientID %>'><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="select_forum" /></label>
                    <asp:DropDownList ID="ForumList" runat="server" DataValueField="ForumID" DataTextField="Title" CssClass="form-control" />
                </div>
                <div class="dropdown-divider"></div>
                <div id="trLeaveLink" runat="server" class="form-check">
                    <label class="form-check-label">
                        <asp:CheckBox ID="LeavePointer" runat="server" />
                        <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="LEAVE_POINTER" />
                    </label>
                </div>
                <div class="dropdown-divider"></div>
                <div class="form-group" id="trLeaveLinkDays" runat="server">
                    <label for='<%= this.LinkDays.ClientID %>'><YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="POINTER_DAYS" /></label>
                    <asp:TextBox ID="LinkDays" runat="server" CssClass="Numeric" TextMode="Number" />
                </div>
                <div class="dropdown-divider"></div>
                <YAF:ThemeButton ID="Move" Type="Primary" Size="Small" runat="server" OnClick="Move_Click"
                                 TextLocalizedTag="MOVE" TextLocalizedPage="MOVETOPIC"/>
            </form>
        </div>
  
<!-- End of Move Topic Button -->
<YAF:ThemeButton ID="DeleteTopic" runat="server" 
                 TextLocalizedTag="BUTTON_DELETETOPIC" TitleLocalizedTag="BUTTON_DELETETOPIC_TT"
                 OnLoad="Delete_Load" 
                 OnClick="DeleteTopics_Click"
                 Type="Danger"
                 Icon="trash"
                 />
            </div>
        </div>
    </div>
</div>

<YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" UsePostBack="True" />

<div id="DivSmartScroller">
    
</div>
