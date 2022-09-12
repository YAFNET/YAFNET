<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Moderate.Forums" CodeBehind="Forums.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Types.Models" %>
<%@ Import Namespace="YAF.Core.Extensions" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>


<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/ModForumUser.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />


<asp:PlaceHolder runat="server" ID="ModerateUsersHolder">
<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="user-secret"
                                LocalizedTag="MEMBERS" 
                                LocalizedPage="MODERATE"/>
            </div>
            <div class="card-body">
                <asp:Repeater runat="server" ID="UserList" OnItemCommand="UserList_ItemCommand" >
        <HeaderTemplate>
            <ul class="list-group list-group-flush">
        </HeaderTemplate>
        <ItemTemplate>
            <li class="list-group-item">
                <span class="fw-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="USER" LocalizedPage="MODERATE" />:
                </span>
                <%#  this.Eval("Item1").ToType<User>().DisplayOrUserName() %>
                <span class="fw-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ACCEPTED" LocalizedPage="MODERATE" />:
                </span>
                <%# this.Eval("Item2.Accepted") %>
                <span class="fw-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ACCESSMASK" LocalizedPage="MODERATE" />:
                </span>
                <%# this.Eval("Item3.Name") %>
                <YAF:ThemeButton ID="ThemeButtonEdit" runat="server" 
                                 CommandName="edit" CommandArgument='<%# this.Eval("Item1.ID") %>'
                                 TitleLocalizedTag="EDIT" 
                                 Size="Small"
                                 Icon="edit"></YAF:ThemeButton>
                <YAF:ThemeButton ID="ThemeButtonRemove" runat="server"
                                 ReturnConfirmTag="confirm_delete_user"
                                 CommandName="remove" CommandArgument='<%# this.Eval("Item1.ID") %>' 
                                 TitleLocalizedTag="REMOVE" 
                                 Size="Small"
                                 Type="Danger"
                                 Icon="trash"></YAF:ThemeButton>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater> 
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

<div class="row">
    <div class="col">
        <div class="card mb-3 mt-3">
            <div class="card-header">
                <div class="row justify-content-between align-items-center">
                    <div class="col-auto">
                        <YAF:IconHeader runat="server"
                                        IconName="tasks"
                                        LocalizedTag="title" 
                                        LocalizedPage="MODERATE"/>
                    </div>
                    <div class="col-auto">
                        <div class="input-group input-group-sm me-2" role="group">
                            <div class="input-group-text">
                                <YAF:LocalizedLabel ID="HelpLabel2" runat="server" LocalizedTag="SHOW" />:
                            </div>
                            <asp:DropDownList runat="server" ID="PageSize"
                                              AutoPostBack="True"
                                              OnSelectedIndexChanged="PageSizeSelectedIndexChanged"
                                              CssClass="form-select">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <asp:Repeater ID="topiclist" runat="server">
                    <ItemTemplate>
                        <div class="row">
                            <div class="col-md-6">
                                <h5>
                                <asp:CheckBox runat="server" ID="topicCheck"
                                              Text="&nbsp;"
                                              CssClass="form-check d-inline-flex align-middle" />
                                <YAF:TopicContainer runat="server" ID="topicContainer" 
                                                    Item="<%# (PagedTopic)Container.DataItem %>"
                                                    AllowSelection="True" />
                    </ItemTemplate>
                    <SeparatorTemplate>
                        <hr/>
                    </SeparatorTemplate>
                </asp:Repeater>
            </div>
            <div class="card-footer text-center">
                <!-- Move Topic Button -->
                <YAF:ThemeButton runat="server"
                         TextLocalizedTag="MOVE" 
                         TextLocalizedPage="MOVETOPIC"
                         CssClass="dropdown-toggle"
                         DataToggle="dropdown"
                         Icon="arrows-alt"
                         Type="Primary"></YAF:ThemeButton>
                <div class="dropdown-menu">
                    <div class="px-4 py-3 dropdown-sm">
                        <div class="mb-3">
                            <label for="ForumList">
                                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" 
                                                    LocalizedTag="select_forum" />
                            </label>
                            <select id="ForumList" name="forumList"></select>
                            <asp:HiddenField runat="server" ID="ForumListSelected" Value="0" />
                        </div>
                        <div id="trLeaveLink" runat="server" 
                             class="form-check">
                            <asp:CheckBox ID="LeavePointer" runat="server" 
                                          Text='<%# this.GetText("LEAVE_POINTER") %>' />
                        </div>
                        <div class="mb-3" id="trLeaveLinkDays" runat="server">
                            <asp:Label AssociatedControlID="LinkDays" runat="server">
                                <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" 
                                                    LocalizedTag="POINTER_DAYS" />
                            </asp:Label>
                            <asp:TextBox ID="LinkDays" runat="server" CssClass="form-control" 
                                         TextMode="Number" />
                        </div>
                        <YAF:ThemeButton ID="Move" runat="server" 
                                         OnClick="Move_Click" 
                                         Type="Primary" 
                                         Size="Small"
                                         Icon="arrows-alt"
                                         TextLocalizedTag="MOVE" TextLocalizedPage="MOVETOPIC"/>
                    </div>
                </div>
  
                <!-- End of Move Topic Button -->
                <YAF:ThemeButton ID="DeleteTopic" runat="server"
                                 TextLocalizedTag="BUTTON_DELETETOPIC" TitleLocalizedTag="BUTTON_DELETETOPIC_TT"
                                 ReturnConfirmTag="confirm_delete"
                                 OnClick="DeleteTopics_Click"
                                 Type="Danger"
                                 Icon="trash" />
            </div>
        </div>
    </div>
</div>

<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager ID="PagerTop" runat="server"
                   OnPageChange="PagerTop_PageChange" 
                   UsePostBack="True" />
    </div>
</div>

<modal:Edit ID="ModForumUserDialog" runat="server" />