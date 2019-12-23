<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.forums" Codebehind="forums.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                LocalizedTag="FORUMS" 
                                LocalizedPage="TEAM" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-comments fa-fw text-secondary pr-1"></i>
                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                        LocalizedTag="FORUMS" 
                                        LocalizedPage="TEAM" />
                 </div>
                <div class="card-body">
		<asp:Repeater ID="CategoryList" runat="server" OnItemCommand="CategoryList_ItemCommand">
			<HeaderTemplate>
			    <ul class="list-group">
			</HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item list-group-item-action active list-group-item-menu">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1"><%# this.HtmlEncode(this.Eval( "Name"))%></h5>
                    <small class="d-none d-md-block">
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="SORT_ORDER">
                        </YAF:LocalizedLabel>&nbsp;
                        <%# this.Eval( "SortOrder") %>
                    </small>
                </div>
                <small>
                    <YAF:ThemeButton ID="ThemeButtonEdit" runat="server" 
                                     Type="Info" 
                                     Size="Small"
                                     CommandName="edit" CommandArgument='<%# this.Eval( "CategoryID") %>'
                                     TitleLocalizedTag="EDIT" Icon="edit"
                                     TextLocalizedTag="EDIT">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" runat="server" 
                                     Type="Danger" 
                                     Size="Small"
                                     ReturnConfirmText='<%# this.GetText("ADMIN_FORUMS", "CONFIRM_DELETE_CAT") %>'
                                     CommandName="delete" CommandArgument='<%# this.Eval( "CategoryID") %>'
                                     TitleLocalizedTag="DELETE" 
                                     Icon="trash"
                                     TextLocalizedTag="DELETE"
                                     Visible='<%# !((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category").Any() %>'>
                    </YAF:ThemeButton>
                    <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                        <YAF:ThemeButton ID="ThemeButton1" runat="server" 
                                         Type="None" 
                                         CssClass="dropdown-item"
                                         CommandName="edit" CommandArgument='<%# this.Eval( "CategoryID") %>'
                                         TitleLocalizedTag="EDIT" Icon="edit"
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ThemeButton2" runat="server" 
                                         Type="None" 
                                         CssClass="dropdown-item"
                                         ReturnConfirmText='<%# this.GetText("ADMIN_FORUMS", "CONFIRM_DELETE_CAT") %>'
                                         CommandName="delete" CommandArgument='<%# this.Eval( "CategoryID") %>'
                                         TitleLocalizedTag="DELETE" 
                                         Icon="trash"
                                         TextLocalizedTag="DELETE"
                                         Visible='<%# !((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category").Any() %>'>
                        </YAF:ThemeButton>
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="NewCategory" runat="server" 
                                         OnClick="NewCategory_Click" 
                                         Type="None" 
                                         CssClass="dropdown-item"
                                         Icon="plus-square" 
                                         TextLocalizedTag="NEW_CATEGORY"></YAF:ThemeButton>
                        <YAF:ThemeButton ID="NewForum" runat="server" 
                                         OnClick="NewForum_Click" 
                                         Type="None" 
                                         CssClass="dropdown-item"
                                         Icon="plus-square" 
                                         TextLocalizedTag="NEW_FORUM"
                                         TextLocalizedPage="ADMIN_FORUMS"></YAF:ThemeButton>
                    </div>
                </small>
				</li>
				<asp:Repeater ID="ForumList" OnItemCommand="ForumList_ItemCommand" runat="server"
					DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>'>
					<ItemTemplate>
                        <li class="list-group-item list-group-item-action list-group-item-menu">
                            <div class="d-flex w-100 justify-content-between">
                                <h5 class="mb-1">
                                    <%# this.HtmlEncode(DataBinder.Eval(Container.DataItem, "[\"Name\"]")) %>
                                </h5>
                                <small class="d-none d-md-block">
                                    <YAF:LocalizedLabel runat="server" LocalizedTag="SORT_ORDER" />&nbsp;
                                    <%# DataBinder.Eval(Container.DataItem, "[\"SortOrder\"]") %>
                                </small>
                            </div>
                            <p class="mb-1">
                                <%# this.HtmlEncode(DataBinder.Eval(Container.DataItem, "[\"Description\"]")) %>
                            </p>
                            <small>
                                <YAF:ThemeButton ID="btnEdit" 
                                                 Type="Info" 
                                                 Size="Small"
                                                 CommandName="edit" CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>'
                                                 TextLocalizedTag="EDIT"
                                                 TitleLocalizedTag="EDIT" Icon="edit" runat="server"></YAF:ThemeButton>
                                <YAF:ThemeButton ID="btnDuplicate" 
                                                 Type="Info" 
                                                 Size="Small"
                                                 CommandName="copy" CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>'
                                                 TextLocalizedTag="COPY"
                                                 TitleLocalizedTag="COPY" Icon="copy" runat="server"></YAF:ThemeButton>
                                <YAF:ThemeButton ID="btnDelete" Type="Danger" Size="Small"
                                                 CommandName="delete" CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>'
                                                 TextLocalizedTag="DELETE"
                                                 TitleLocalizedTag="DELETE" Icon="trash" runat="server"></YAF:ThemeButton>
                            </small>
                            <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                                <YAF:ThemeButton ID="ThemeButton3" 
                                                 Type="None" 
                                                 CssClass="dropdown-item"
                                                 CommandName="edit" CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>'
                                                 TextLocalizedTag="EDIT"
                                                 TitleLocalizedTag="EDIT" Icon="edit" runat="server"></YAF:ThemeButton>
                                <YAF:ThemeButton ID="ThemeButton4" 
                                                 Type="None" 
                                                 CssClass="dropdown-item"
                                                 CommandName="copy" CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>'
                                                 TextLocalizedTag="COPY"
                                                 TitleLocalizedTag="COPY" Icon="copy" runat="server"></YAF:ThemeButton>
                                <YAF:ThemeButton ID="ThemeButton5" 
                                                 Type="None" 
                                                 CssClass="dropdown-item"
                                                 CommandName="delete" CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>'
                                                 TextLocalizedTag="DELETE"
                                                 TitleLocalizedTag="DELETE" Icon="trash" runat="server"></YAF:ThemeButton>
                                <div class="dropdown-divider"></div>
                                <YAF:ThemeButton ID="NewCategory" runat="server" 
                                                 OnClick="NewCategory_Click" 
                                                 Type="None" 
                                                 CssClass="dropdown-item"
                                                 Icon="plus-square" 
                                                 TextLocalizedTag="NEW_CATEGORY"></YAF:ThemeButton>
                                <YAF:ThemeButton ID="NewForum" runat="server" 
                                                 OnClick="NewForum_Click" 
                                                 Type="None" 
                                                 CssClass="dropdown-item"
                                                 Icon="plus-square" 
                                                 TextLocalizedTag="NEW_FORUM"
                                                 TextLocalizedPage="ADMIN_FORUMS"></YAF:ThemeButton>
                            </div>
						</li>
					</ItemTemplate>
				</asp:Repeater>
			</ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
		</asp:Repeater>
                </div>
                <div class="card-footer text-center">
				<YAF:ThemeButton ID="NewCategory" runat="server" 
                                 OnClick="NewCategory_Click" 
                                 Type="Primary"
				                 Icon="plus-square" 
                                 TextLocalizedTag="NEW_CATEGORY"></YAF:ThemeButton>
				<YAF:ThemeButton ID="NewForum" runat="server" 
                                 OnClick="NewForum_Click" 
                                 Type="Primary"
				                 Icon="plus-square" 
                                 TextLocalizedTag="NEW_FORUM"
                                 TextLocalizedPage="ADMIN_FORUMS"></YAF:ThemeButton>

                </div>
            </div>
        </div>
    </div>