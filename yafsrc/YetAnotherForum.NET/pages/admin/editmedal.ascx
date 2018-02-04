<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.editmedal" Codebehind="editmedal.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITMEDAL" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-trophy fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITMEDAL" />
                </div>
                <div class="card-body">
			<h4>
				<YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="MEDAL_NAME" LocalizedPage="ADMIN_EDITMEDAL" />
            </h4>
			<p>
				<asp:TextBox CssClass="form-control" ID="Name" runat="server" MaxLength="100" />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="Name" Display="Dynamic"
					ValidationGroup="Medal" Text="Required" />
			</p><hr />
		    <h4>
				<YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="MEDAL_DESC" LocalizedPage="ADMIN_EDITMEDAL" />
		    </h4>
			<p>
				<asp:TextBox Style="height: 100px;" ID="Description" TextMode="MultiLine" CssClass="form-control"
					runat="server" />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="Description" Display="Dynamic"
					ValidationGroup="Medal" Text="Required" />
			</p><hr />
			<h4>
				<YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="MEDAL_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
            </h4>
			<p>
				<asp:TextBox  CssClass="form-control" ID="Message" runat="server" MaxLength="100" />
				<asp:RequiredFieldValidator runat="server" ControlToValidate="Message" Display="Dynamic"
					ValidationGroup="Medal" Text="Required" />
			</p><hr />
            <h4>
				<YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="MEDAL_CATEGORY" LocalizedPage="ADMIN_EDITMEDAL" />
		    </h4>
			<p>
				<asp:TextBox  CssClass="form-control" ID="Category" MaxLength="50" runat="server" />
            </p>
	        <h4>
				<YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="MEDAL_IMAGE" LocalizedPage="ADMIN_EDITMEDAL" />
            </h4>
			<p>
				<img style="vertical-align: top;" runat="server" id="MedalPreview" alt="Preview" />
                <asp:DropDownList  CssClass="custom-select" ID="MedalImage" runat="server" alt="Preview" />
			</p><hr />
		    <h4>
				<YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="RIBBON_IMAGE" LocalizedPage="ADMIN_EDITMEDAL" />
            </h4>
			<p>
				<img style="vertical-align: top;" runat="server" id="RibbonPreview" alt="Preview" />
				<asp:DropDownList CssClass="custom-select" ID="RibbonImage" runat="server" />
			</p><hr />
		    <h4>
				<YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="SMALL_IMAGE" LocalizedPage="ADMIN_EDITMEDAL" />
            </h4>
			<p>
				<img style="vertical-align: top;" runat="server" id="SmallMedalPreview" alt="Preview" />
				<asp:DropDownList ID="SmallMedalImage" runat="server" CssClass="custom-select" />
			</p><hr />
		    <h4>
				<YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="SMALL_RIBBON" LocalizedPage="ADMIN_EDITMEDAL" />
            </h4>
			<p>
				<img style="vertical-align: top;" runat="server" id="SmallRibbonPreview" alt="Preview" />
				<asp:DropDownList ID="SmallRibbonImage" runat="server" CssClass="custom-select" />
			</p><hr />
		    <h4>
				<YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="SORT_ORDER" LocalizedPage="ADMIN_EDITMEDAL" />
            </h4>
			<p>
				<asp:TextBox ID="SortOrder" MaxLength="5" runat="server" CssClass="form-control" TextMode="Number" />
		    </p><hr />
			<h4>
				<YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="SHOW_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
            </h4>
			<p>
				<asp:CheckBox ID="ShowMessage" runat="server" Checked="true" CssClass="form-control" />
		    </p><hr />
			<h4>
				<YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="ALLOW_RIBBON" LocalizedPage="ADMIN_EDITMEDAL" />
            </h4>
			<p>
				<asp:CheckBox ID="AllowRibbon" runat="server" Checked="true" CssClass="form-control" />
		    </p><hr />

			<h4>
				<YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="ALLOW_HIDING" LocalizedPage="ADMIN_EDITMEDAL" />
            </h4>
			<p>
				<asp:CheckBox ID="AllowHiding" runat="server" Checked="true" CssClass="form-control" />
		    </p><hr />
            <h4>
				<YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="ALLOW_REORDER" LocalizedPage="ADMIN_EDITMEDAL" />
            </h4>
			<p>
				<asp:CheckBox ID="AllowReOrdering" runat="server" Checked="true" CssClass="form-control" />
		    </p>
          </div>
                <div class="card-footer text-lg-center">
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
				<div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                   <table class="table">
                       <thead>
				    <tr>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="GROUP" />
					</th>
                        <th>
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="MESSAGE" LocalizedPage="COMMON" />
					</th>
                        <th>
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="COMMAND" />
                    </th>
				</tr>
                           </thead>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td>
						<%# this.FormatGroupLink(Container.DataItem) %>
					</td>
					<td>
						<%# this.Eval("Message") %>
					</td>
					<td>
					    <span class="float-right">
					    <YAF:ThemeButton ID="ThemeButtonEdit" Type="Info" CssClass="btn-sm"
                            CommandName='edit' CommandArgument='<%# this.Eval( "GroupID") %>'
                            TitleLocalizedTag="EDIT"
                            Icon="edit"
                            TextLocalizedTag="EDIT"
                            runat="server">
					    </YAF:ThemeButton>
						<YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" CssClass="btn-sm"
                                    CommandName='delete' CommandArgument='<%# this.Eval( "GroupID") %>'
                                    TitleLocalizedTag="DELETE"
                                    Icon="trash"
                                    TextLocalizedTag="DELETE"
                                    OnLoad="GroupRemoveLoad"  runat="server">
                                </YAF:ThemeButton>
					</td>
				</tr>
			</ItemTemplate>
            <FooterTemplate>
                </table></div>
            </FooterTemplate>
		</asp:Repeater>
		<asp:PlaceHolder runat="server" ID="AddGroupRow" Visible="false">
			</div>
                <div class="card-footer text-lg-center">
				   <YAF:ThemeButton runat="server" OnClick="AddGroupClick" ID="AddGroup" Type="Primary"            
				                   Icon="plus-square" TextLocalizedTag="ADD_GROUP"></YAF:ThemeButton>
			    </div>
             </div>
		</asp:PlaceHolder>
		<asp:PlaceHolder runat="server" ID="AddGroupPanel" Visible="false">
            <h3><asp:Label runat="server" ID="GroupMedalEditTitle" /></h3>
                <h4>
					<YAF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="MEDAL_GROUP" LocalizedPage="ADMIN_EDITMEDAL" />
                </h4>
				<p>
					<asp:DropDownList runat="server" ID="AvailableGroupList" CssClass="custom-select" />
				</p><hr />
			    <h4>
					<YAF:HelpLabel ID="HelpLabel15" runat="server" LocalizedTag="OVERRIDE_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
                </h4>
				<p>
					<asp:TextBox ID="GroupMessage" runat="server" MaxLength="100" CssClass="form-control" />
				</p><hr />
				<h4>
					<YAF:HelpLabel ID="HelpLabel16" runat="server" LocalizedTag="OVERRIDE_ORDER" LocalizedPage="ADMIN_EDITMEDAL" />
                </h4>
				<p>
					<asp:TextBox ID="GroupSortOrder" runat="server" CssClass="form-control" TextMode="Number" />
				</p><hr />
			    <h4>
					<YAF:HelpLabel ID="HelpLabel17" runat="server" LocalizedTag="ONLY_RIBBON" LocalizedPage="ADMIN_EDITMEDAL" />
                </h4>
				<p>
					<asp:CheckBox runat="server" ID="GroupOnlyRibbon" Checked="false" CssClass="form-control"  />
				</p><hr />
				<h4>
                    <YAF:HelpLabel ID="HelpLabel18" runat="server" LocalizedTag="HIDE" LocalizedPage="ADMIN_EDITMEDAL" />
				</h4>
				<p>
					<asp:CheckBox runat="server" ID="GroupHide" Checked="false" CssClass="form-control"  />
				</p>
                </div>
                <div class="card-footer text-lg-center">
					<YAF:ThemeButton runat="server"  OnClick="AddGroupSaveClick" ID="AddGroupSave" Type="Primary"            
					                Icon="save" TextLocalizedTag="SAVE" />&nbsp;
					<YAF:ThemeButton runat="server"  OnClick="AddGroupCancelClick" ID="AddGroupCancel" Type="Secondary"
					                Icon="times" TextLocalizedTag="CANCEL" />
				</div>
              </div>
		</asp:PlaceHolder>
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
				<div class="table-responsive"><table class="table">
                    <tr>
                      <thead>
                        <th>
					    <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="USERNAME" LocalizedPage="ACTIVEUSERS" />
						(<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="USER" LocalizedPage="MODERATE" />)
                    </th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="MESSAGE" LocalizedPage="COMMON" />
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="DATE_AWARDED" LocalizedPage="ADMIN_EDITMEDAL" />
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="COMMAND" />
                        </thead>
                    </tr>
			</HeaderTemplate>
			<ItemTemplate>

					<td>
						<%# this.FormatUserLink(Container.DataItem) %>
					</td>
					<td>
						<%# this.Eval("Message") %>
					</td>
					<td>
						<%# this.Get<IDateTime>().FormatDateTimeTopic((DateTime)this.Eval("DateAwarded")) %>
					</td>
					<td>
					    <span class="float-right">
						<asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# this.Eval("UserID") %>'  Type="Info" CssClass="btn-sm"> <i class="fa fa-edit fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="EDIT" /></asp:LinkButton>
						&nbsp;<asp:LinkButton runat="server" CommandName="remove" CommandArgument='<%# this.Eval("UserID") %>'  Type="Danger" CssClass="btn-sm"
							OnLoad="UserRemoveLoad"> <i class="fa fa-trash fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="REMOVE" /></asp:LinkButton>

					    </span>
                    </td>
				</tr>
			</ItemTemplate>
            <FooterTemplate>
                </table></div>
            </FooterTemplate>
		</asp:Repeater>
		<asp:PlaceHolder runat="server" id="AddUserRow" visible="false">
			</div>
                <div class="card-footer text-lg-center">
				   <YAF:ThemeButton runat="server" OnClick="AddUserClick" ID="AddUser" Type="Primary"
				                    Icon="plus-square" TextLocalizedTag="ADD_USER"></YAF:ThemeButton>
			    </div>
             </div>
		</asp:PlaceHolder>
		<asp:PlaceHolder runat="server" ID="AddUserPanel" Visible="false">
			  <h3><asp:Label runat="server" ID="UserMedalEditTitle" /></h3>
				<h4>
					<YAF:HelpLabel ID="HelpLabel23" runat="server" LocalizedTag="MEDAL_USER" LocalizedPage="ADMIN_EDITMEDAL" />
                </h4>
				<p>
					<asp:TextBox ID="UserName" runat="server" CssClass="form-control" />
					<asp:DropDownList  runat="server" ID="UserNameList" Visible="false" CssClass="custom-select" />
					<asp:Button runat="server" ID="FindUsers" Text="Find Users" OnClick="FindUsersClick" Type="Info" CssClass="btn-sm" />
					<asp:Button runat="server" ID="Clear" Text="Clear" OnClick="ClearClick" Visible="false" Type="Info" CssClass="btn-sm" />
					<asp:TextBox Visible="false" ID="UserID" runat="server" CssClass="form-control" />
				</p><hr />
			    <h4>
					<YAF:HelpLabel ID="HelpLabel19" runat="server" LocalizedTag="OVERRIDE_MESSAGE" LocalizedPage="ADMIN_EDITMEDAL" />
				</h4>
				<p>
					<asp:TextBox ID="UserMessage" runat="server" MaxLength="100" CssClass="form-control"  />
				</p><hr />
				<h4>
					<YAF:HelpLabel ID="HelpLabel20" runat="server" LocalizedTag="OVERRIDE_ORDER" LocalizedPage="ADMIN_EDITMEDAL" />
				</h4>
				<p>
					<asp:TextBox ID="UserSortOrder" runat="server" CssClass="form-control" TextMode="Number"  />
				</p><hr />
				<h4>
					<YAF:HelpLabel ID="HelpLabel21" runat="server" LocalizedTag="ONLY_RIBBON" LocalizedPage="ADMIN_EDITMEDAL" />
				</h4>
				<p>
					<asp:CheckBox runat="server" ID="UserOnlyRibbon" Checked="false" CssClass="form-control"  />
				</p><hr />
				<h4>
					<YAF:HelpLabel ID="HelpLabel22" runat="server" LocalizedTag="HIDE" LocalizedPage="ADMIN_EDITMEDAL" />
				</h4>
				<p>
					<asp:CheckBox runat="server" ID="UserHide" Checked="false" CssClass="form-control" />
				</p>
                </div>
                <div class="card-footer text-lg-center">
					<YAF:ThemeButton runat="server" OnClick="AddUserSaveClick" ID="AddUserSave" Type="Primary"
					                Icon="save" TextLocalizedTag="SAVE" />&nbsp;
					<YAF:ThemeButton runat="server" OnClick="AddUserCancelClick" ID="AddUserCancel" Type="Secondary"
					                Icon="times" TextLocalizedTag="CANCEL" />
                </div>
            </div>
		</asp:PlaceHolder>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
