<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.editmedal" Codebehind="editmedal.ascx.cs" %>

<%@ Register TagPrefix="modal" TagName="GroupEdit" Src="../../Dialogs/GroupMedalEdit.ascx" %>
<%@ Register TagPrefix="modal" TagName="UserEdit" Src="../../Dialogs/UserMedalEdit.ascx" %>


<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITMEDAL" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-trophy fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                                                                LocalizedTag="TITLE" 
                                                                                LocalizedPage="ADMIN_EDITMEDAL" />
                </div>
                <div class="card-body">
			 
				<YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="MEDAL_NAME" LocalizedPage="ADMIN_EDITMEDAL" />
             
			<p>
				<asp:TextBox CssClass="form-control" ID="Name" runat="server" MaxLength="100" />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="Name" Display="Dynamic"
					ValidationGroup="Medal" Text="Required" />
			</p><hr />
		     
				<YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="MEDAL_DESC" LocalizedPage="ADMIN_EDITMEDAL" />
		     
			<p>
				<asp:TextBox Style="height: 100px;" ID="Description" TextMode="MultiLine" CssClass="form-control"
					runat="server" />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="Description" Display="Dynamic"
					ValidationGroup="Medal" Text="Required" />
			</p><hr />
			 
				<YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="MEDAL_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
             
			<p>
				<asp:TextBox  CssClass="form-control" ID="Message" runat="server" MaxLength="100" />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="Message" Display="Dynamic"
					ValidationGroup="Medal" Text="Required" />
			</p><hr />
             
				<YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="MEDAL_CATEGORY" LocalizedPage="ADMIN_EDITMEDAL" />
		     
			<p>
				<asp:TextBox  CssClass="form-control" ID="Category" MaxLength="50" runat="server" />
            </p>
	         
				<YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="MEDAL_IMAGE" LocalizedPage="ADMIN_EDITMEDAL" />
             
			<p>
				<img style="vertical-align: top;" runat="server" id="MedalPreview" alt="Preview" />
                <asp:DropDownList  CssClass="custom-select" ID="MedalImage" runat="server" alt="Preview" />
			</p><hr />
		     
				<YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="RIBBON_IMAGE" LocalizedPage="ADMIN_EDITMEDAL" />
             
			<p>
				<img style="vertical-align: top;" runat="server" id="RibbonPreview" alt="Preview" />
				<asp:DropDownList CssClass="custom-select" ID="RibbonImage" runat="server" />
			</p><hr />
		     
				<YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="SMALL_IMAGE" LocalizedPage="ADMIN_EDITMEDAL" />
             
			<p>
				<img style="vertical-align: top;" runat="server" id="SmallMedalPreview" alt="Preview" />
				<asp:DropDownList ID="SmallMedalImage" runat="server" CssClass="custom-select" />
			</p><hr />
		     
				<YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="SMALL_RIBBON" LocalizedPage="ADMIN_EDITMEDAL" />
             
			<p>
				<img style="vertical-align: top;" runat="server" id="SmallRibbonPreview" alt="Preview" />
				<asp:DropDownList ID="SmallRibbonImage" runat="server" CssClass="custom-select" />
			</p><hr />
		     
				<YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="SORT_ORDER" LocalizedPage="ADMIN_EDITMEDAL" />
             
			<p>
				<asp:TextBox ID="SortOrder" MaxLength="5" runat="server" CssClass="form-control" TextMode="Number" />
		    </p><hr />
			 
				<YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="SHOW_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
             
			<div class="custom-control custom-switch">
				<asp:CheckBox ID="ShowMessage" runat="server" Checked="true" Text="&nbsp;" />
		    </div><hr />
			 
				<YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="ALLOW_RIBBON" LocalizedPage="ADMIN_EDITMEDAL" />
             
			<div class="custom-control custom-switch">
				<asp:CheckBox ID="AllowRibbon" runat="server" Checked="true" Text="&nbsp;" />
		    </div><hr />

			 
				<YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="ALLOW_HIDING" LocalizedPage="ADMIN_EDITMEDAL" />
             
			<div class="custom-control custom-switch">
				<asp:CheckBox ID="AllowHiding" runat="server" Checked="true" Text="&nbsp;" />
		    </div><hr />
             
				<YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="ALLOW_REORDER" LocalizedPage="ADMIN_EDITMEDAL" />
             
			<div class="custom-control custom-switch">
				<asp:CheckBox ID="AllowReOrdering" runat="server" Checked="true" Text="&nbsp;" />
		    </div>
          </div>
                <div class="card-footer text-center">
				<YAF:ThemeButton ID="Save" runat="server" OnClick="SaveClick" Type="Primary"            
				                Icon="save" TextLocalizedTag="SAVE" />&nbsp;
				<YAF:ThemeButton ID="Cancel" runat="server" OnClick="CancelClick" Type="Secondary"
				                Icon="times" TextLocalizedTag="CANCEL" />
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-trophy fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_EDITMEDAL" />
                </div>
                <div class="card-body">
		<asp:Repeater ID="GroupList" runat="server" OnItemCommand="GroupListItemCommand">
			<HeaderTemplate>
                <ul class="list-group">
            </HeaderTemplate>
			<ItemTemplate>
                <li class="list-group-item list-group-item-action">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1 text-break">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="GROUP" />:
                        <%# this.FormatGroupLink(Container.DataItem) %>
                    </h5>
                </div>
                <p class="mb-1">
                    <span class="font-weight-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                            LocalizedTag="MESSAGE" 
                                            LocalizedPage="COMMON" />
                    </span>
                    <%# this.Eval("Message") %>
                </p>
                <small>
                    <YAF:ThemeButton ID="ThemeButtonEdit" runat="server"
                                     Type="Info" 
                                     Size="Small"
                                     CommandName='edit' 
                                     CommandArgument='<%# this.Eval( "GroupID") %>'
                                     TitleLocalizedTag="EDIT"
                                     Icon="edit"
                                     TextLocalizedTag="EDIT">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" runat="server" 
                                     Type="Danger" 
                                     Size="Small"
                                     CommandName='delete' 
                                     CommandArgument='<%# this.Eval( "GroupID") %>'
                                     TitleLocalizedTag="DELETE"
                                     Icon="trash"
                                     TextLocalizedTag="DELETE"
                                     ReturnConfirmText='<%# this.GetText("ADMIN_EDITMEDAL", "CONFIRM_REMOVE_USER") %>'>
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
                    <i class="fa fa-trophy fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="HEADER3" LocalizedPage="ADMIN_EDITMEDAL" />
                    </div>
                <div class="card-body">
        <asp:Repeater ID="UserList" runat="server" OnItemCommand="UserListItemCommand">
			<HeaderTemplate>
                <ul class="list-group">
			</HeaderTemplate>
			<ItemTemplate>
                <li class="list-group-item list-group-item-action">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1 text-break">
                        <%# this.FormatUserLink(Container.DataItem) %>
                    </h5>
                    <small class="d-none d-md-block">
                        <span class="font-weight-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" 
                                                LocalizedTag="DATE_AWARDED" 
                                                LocalizedPage="ADMIN_EDITMEDAL" />:
                        </span>
                        <%# this.Get<IDateTime>().FormatDateTimeTopic((DateTime)this.Eval("DateAwarded")) %>
                    </small>
                </div>
                <p class="mb-1">
                    <span class="font-weight-bold">
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" 
                                            LocalizedTag="MESSAGE" 
                                            LocalizedPage="COMMON" />
                    </span>
                    <%# this.Eval("Message") %>
                </p>
                <small>
                    <YAF:ThemeButton runat="server" 
                                     CommandName="edit" 
                                     CommandArgument='<%# this.Eval("UserID") %>' 
                                     TextLocalizedTag="EDIT"
                                     Type="Info" 
                                     Size="Small"
                                     Icon="edit">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton runat="server" 
                                     CommandName="remove" 
                                     CommandArgument='<%# this.Eval("UserID") %>' 
                                     TextLocalizedTag="REMOVE"
                                     Type="Danger" 
                                     Size="Small"
                                     Icon="trash">
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