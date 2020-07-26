<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Moderating" CodeBehind="Moderating.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />


<asp:PlaceHolder runat="server" ID="ModerateUsersHolder">
<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-user-secret fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                                                                 LocalizedTag="MEMBERS" 
                                                                                 LocalizedPage="MODERATE" />
            </div>
            <div class="card-body">
                
    <asp:Repeater runat="server" ID="UserList" OnItemCommand="UserList_ItemCommand" >
        <HeaderTemplate>
            <ul class="list-group list-group-flush">
        </HeaderTemplate>
        <ItemTemplate>
            <li class="list-group-item">
                <span class="font-weight-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="USER" LocalizedPage="MODERATE" />:
                </span>
                <%#  this.Eval(this.Get<BoardSettings>().EnableDisplayName ? "Item1.Name" : "Item1.DisplayName") %>
                <span class="font-weight-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ACCEPTED" LocalizedPage="MODERATE" />:
                </span>
                <%# this.Eval("Item2.Accepted") %>
                <span class="font-weight-bold">
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ACCESSMASK" LocalizedPage="MODERATE" />:
                </span>
                <%# this.Eval("Item3.Name") %>
                <YAF:ThemeButton ID="ThemeButtonEdit" runat="server" 
                                 CommandName="edit" CommandArgument='<%# this.Eval("Item1.ID") %>'
                                 TitleLocalizedTag="EDIT" 
                                 Size="Small"
                                 Icon="edit"></YAF:ThemeButton>
                <YAF:ThemeButton ID="ThemeButtonRemove" runat="server"
                                 ReturnConfirmText='<%#this.GetText("moderate", "confirm_delete_user") %>'
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

<YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" UsePostBack="True" />

<div class="row">
    <div class="col">
        <div class="card mb-3 mt-3">
            <div class="card-header">
                <i class="fa fa-tasks fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="title" LocalizedPage="MODERATE" />
            </div>
            <div class="card-body">
                <asp:Repeater ID="topiclist" runat="server">
                        <ItemTemplate>
                            <div class="row">
                                <div class="col-md-6">
                                    <h5>
                                        <asp:CheckBox runat="server" ID="topicCheck"
                                                      Text="&nbsp;"
                                                      CssClass="custom-control custom-checkbox d-inline-flex" />
                                        <YAF:TopicContainer runat="server" ID="topicContainer" DataRow="<%# Container.DataItem %>" AllowSelection="True"></YAF:TopicContainer>
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
                <div class="form-group">
                    <asp:Label AssociatedControlID="ForumList" runat="server">
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="select_forum" />
                    </asp:Label>
                    <asp:DropDownList ID="ForumList" runat="server" 
                                      DataValueField="ForumID" 
                                      DataTextField="Title" 
                                      CssClass="select2-image-select" />
                </div>
                <div class="dropdown-divider"></div>
                <div id="trLeaveLink" runat="server" 
                     class="custom-control custom-checkbox">
                    <asp:CheckBox ID="LeavePointer" runat="server" 
                                  Text='<%# this.GetText("LEAVE_POINTER") %>' />
                </div>
                <div class="dropdown-divider"></div>
                <div class="form-group" id="trLeaveLinkDays" runat="server">
                    <asp:Label AssociatedControlID="LinkDays" runat="server">
                        <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="POINTER_DAYS" />
                    </asp:Label>
                    <asp:TextBox ID="LinkDays" runat="server" CssClass="form-control" TextMode="Number" />
                </div>
                <div class="dropdown-divider"></div>
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
                                 ReturnConfirmText='<%# this.GetText("moderate", "confirm_delete") %>'
                                 OnClick="DeleteTopics_Click"
                                 Type="Danger"
                                 Icon="trash"
                 />
            </div>
        </div>
    </div>
</div>

<YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" UsePostBack="True" />