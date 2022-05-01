<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.EditMedal" Codebehind="EditMedal.ascx.cs" %>

<%@ Register TagPrefix="modal" TagName="GroupEdit" Src="../../Dialogs/GroupMedalEdit.ascx" %>
<%@ Register TagPrefix="modal" TagName="UserEdit" Src="../../Dialogs/UserMedalEdit.ascx" %>


<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>
<%@ Import Namespace="YAF.Types.Models" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="medal"
                                    LocalizedPage="ADMIN_EDITMEDAL"></YAF:IconHeader>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel1" runat="server"
                                           AssociatedControlID="Name"
                                           LocalizedTag="MEDAL_NAME" LocalizedPage="ADMIN_EDITMEDAL" />
                            <asp:TextBox ID="Name" runat="server"
                                         MaxLength="100"
                                         CssClass="form-control"
                                         required="required" />
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedTag="MSG_NAME" />
                            </div>
                        </div>
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel4" runat="server"
                                           AssociatedControlID="Category"
                                           LocalizedTag="MEDAL_CATEGORY" LocalizedPage="ADMIN_EDITMEDAL" />
                            <asp:TextBox  ID="Category" MaxLength="50" CssClass="form-control"  runat="server" />
                        </div>
                    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel2" runat="server"
                                       AssociatedControlID="Description"
                                       LocalizedTag="MEDAL_DESC" LocalizedPage="ADMIN_EDITMEDAL" />


                        <asp:TextBox ID="Description" runat="server"
                                     TextMode="MultiLine"
                                     CssClass="form-control"
                                     Height="100" />
                    </div>
                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel3" runat="server"
                                           AssociatedControlID="Message"
                                           LocalizedTag="MEDAL_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
                            <asp:TextBox  ID="Message" runat="server"
                                          MaxLength="100"
                                          CssClass="form-control" />
                        </div>
                        <div class="mb-3 col-md-4">
                            <YAF:HelpLabel ID="HelpLabel10" runat="server"
                                           AssociatedControlID="ShowMessage"
                                           LocalizedTag="SHOW_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />

                            <div class="form-check form-switch">
                                <asp:CheckBox ID="ShowMessage" runat="server" Checked="true" Text="&nbsp;" />
                            </div>
                        </div>
                    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel5" runat="server"
                                       AssociatedControlID="MedalImage"
                                       LocalizedTag="MEDAL_IMAGE" LocalizedPage="ADMIN_EDITMEDAL" />
                        <YAF:ImageListBox ID="MedalImage" runat="server" CssClass="select2-image-select" />
                    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel12" runat="server"
                                       AssociatedControlID="AllowHiding"
                                       LocalizedTag="ALLOW_HIDING" LocalizedPage="ADMIN_EDITMEDAL" />
                        <div class="form-check form-switch">
                            <asp:CheckBox ID="AllowHiding" runat="server"
                                          Checked="true"
                                          Text="&nbsp;" />
                        </div>
                    </div>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="Save" runat="server"
                                     CssClass="me-2"
                                     OnClick="SaveClick"
                                     CausesValidation="True"
                                     Type="Primary"
                                     Icon="save"
                                     TextLocalizedTag="SAVE" />
                    <YAF:ThemeButton ID="Cancel" runat="server"
                                     OnClick="CancelClick"
                                     Type="Secondary"
                                     Icon="times"
                                     TextLocalizedTag="CANCEL" />
                </div>
            </div>
    </div>
</div>
<asp:PlaceHolder runat="server" ID="UserAndGroupsHolder">
<div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="medal"
                                    LocalizedTag="HEADER2"
                                    LocalizedPage="ADMIN_EDITMEDAL"></YAF:IconHeader>
                </div>
                <div class="card-body">
        <asp:Repeater ID="GroupList" runat="server" OnItemCommand="GroupListItemCommand">
            <HeaderTemplate>
                <ul class="list-group">
            </HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item list-group-item-action list-group-item-menu">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1 text-break">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="GROUP" />:
                        <%# this.FormatGroupLink(Container.DataItem) %>
                    </h5>
                </div>
                <p class="mb-1">
                    <span class="fw-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server"
                                            LocalizedTag="MESSAGE"
                                            LocalizedPage="COMMON" />
                    </span>
                    <%# !this.Eval("Item2.Message").IsNullOrEmptyField() ? this.Eval("Item2.Message") :  this.Eval("Item1.Message")%>
                </p>
                <small>
                    <div class="btn-group btn-group-sm">
                        <YAF:ThemeButton ID="ThemeButtonEdit" runat="server"
                                         Type="Info"
                                         Size="Small"
                                         CommandName="edit"
                                         CommandArgument='<%# this.Eval( "Item2.GroupID") %>'
                                         TitleLocalizedTag="EDIT"
                                         Icon="edit"
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ThemeButtonDelete" runat="server"
                                         Type="Danger"
                                         Size="Small"
                                         CommandName="delete"
                                         CommandArgument='<%# this.Eval( "Item2.GroupID") %>'
                                         TitleLocalizedTag="REMOVE"
                                         Icon="trash"
                                         TextLocalizedTag="REMOVE"
                                         ReturnConfirmText='<%# this.GetText("ADMIN_EDITMEDAL", "CONFIRM_REMOVE_USER") %>'>
                        </YAF:ThemeButton>
                    </div>
                </small>
                    <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                        <YAF:ThemeButton ID="ThemeButton1" runat="server"
                                         Type="None"
                                         CssClass="dropdown-item"
                                         CommandName="edit"
                                         CommandArgument='<%# this.Eval( "Item2.GroupID") %>'
                                         TitleLocalizedTag="EDIT"
                                         Icon="edit"
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ThemeButton2" runat="server"
                                         Type="None"
                                         CssClass="dropdown-item"
                                         CommandName="delete"
                                         CommandArgument='<%# this.Eval( "Item2.GroupID") %>'
                                         TitleLocalizedTag="REMOVE"
                                         Icon="trash"
                                         TextLocalizedTag="REMOVE"
                                         ReturnConfirmText='<%# this.GetText("ADMIN_EDITMEDAL", "CONFIRM_REMOVE_USER") %>'>
                        </YAF:ThemeButton>
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton runat="server"
                                         OnClick="AddGroupClick"
                                         ID="AddGroup"
                                         Type="None"
                                         CssClass="dropdown-item"
                                         Icon="plus-square"
                                         TextLocalizedTag="ADD_GROUP">
                        </YAF:ThemeButton>
                    </div>
            </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
                </div>
                <div class="card-footer text-center">
                   <YAF:ThemeButton runat="server"
                                    OnClick="AddGroupClick"
                                    ID="AddGroup"
                                    Type="Primary"
                                    Icon="plus-square"
                                    TextLocalizedTag="ADD_GROUP">
                   </YAF:ThemeButton>
                </div>
            </div>
        </div>
</div>
<div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="medal"
                                    LocalizedTag="HEADER3"
                                    LocalizedPage="ADMIN_EDITMEDAL"></YAF:IconHeader>
                    </div>
                <div class="card-body">
        <asp:Repeater ID="UserList" runat="server" OnItemCommand="UserListItemCommand">
            <HeaderTemplate>
                <ul class="list-group">
            </HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item list-group-item-action list-group-item-menu">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1 text-break">
                        <%# this.FormatUserLink(Container.DataItem) %>
                    </h5>
                    <small class="d-none d-md-block">
                        <span class="fw-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server"
                                                LocalizedTag="DATE_AWARDED"
                                                LocalizedPage="ADMIN_EDITMEDAL" />:
                        </span>
                        <%# this.Get<IDateTimeService>().FormatDateTimeTopic(((Tuple<Medal, UserMedal, User>)Container.DataItem).Item2.DateAwarded) %>
                    </small>
                </div>
                <p class="mb-1">
                    <span class="fw-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server"
                                            LocalizedTag="MESSAGE"
                                            LocalizedPage="COMMON" />
                    </span>
                    <%# ((Tuple<Medal, UserMedal, User>)Container.DataItem).Item2.Message.IsSet() ? ((Tuple<Medal, UserMedal, User>)Container.DataItem).Item2.Message :  ((Tuple<Medal, UserMedal, User>)Container.DataItem).Item1.Message%>
                </p>
                <small>
                    <div class="btn-group btn-group-sm">
                        <YAF:ThemeButton runat="server"
                                         CommandName="edit"
                                         CommandArgument="<%# ((Tuple<Medal, UserMedal, User>)Container.DataItem).Item2.UserID %>"
                                         TextLocalizedTag="EDIT"
                                         Type="Info"
                                         Size="Small"
                                         Icon="edit">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton runat="server"
                                         CommandName="remove"
                                         CommandArgument="<%# ((Tuple<Medal, UserMedal, User>)Container.DataItem).Item2.UserID %>"
                                         TextLocalizedTag="REMOVE"
                                         Type="Danger"
                                         Size="Small"
                                         Icon="trash">
                        </YAF:ThemeButton>
                    </div>
                </small>
                    <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                        <YAF:ThemeButton runat="server"
                                         CommandName="edit"
                                         CommandArgument="<%# ((Tuple<Medal, UserMedal, User>)Container.DataItem).Item2.UserID %>"
                                         TextLocalizedTag="EDIT"
                                         Type="None"
                                         CssClass="dropdown-item"
                                         Icon="edit">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton runat="server"
                                         CommandName="remove"
                                         CommandArgument="<%# ((Tuple<Medal, UserMedal, User>)Container.DataItem).Item2.UserID %>"
                                         TextLocalizedTag="REMOVE"
                                         Type="None"
                                         CssClass="dropdown-item"
                                         Icon="trash">
                        </YAF:ThemeButton>
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton runat="server"
                                         OnClick="AddUserClick"
                                         ID="AddUser"
                                         Type="None"
                                         CssClass="dropdown-item"
                                         Icon="plus-square"
                                         TextLocalizedTag="ADD_USER">
                        </YAF:ThemeButton>
                    </div>
            </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
                </div>
                <div class="card-footer text-center">
                   <YAF:ThemeButton runat="server"
                                    OnClick="AddUserClick"
                                    ID="AddUser"
                                    Type="Primary"
                                    Icon="plus-square"
                                    TextLocalizedTag="ADD_USER">
                   </YAF:ThemeButton>
                </div>
             </div>
        </div>
    </div>


<modal:GroupEdit ID="GroupEditDialog" runat="server" />
<modal:UserEdit ID="UserEditDialog" runat="server" />
</asp:PlaceHolder>