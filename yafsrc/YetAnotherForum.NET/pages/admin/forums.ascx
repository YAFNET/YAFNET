<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.forums" Codebehind="forums.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="FORUMS" LocalizedPage="TEAM" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-newspaper fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="FORUMS" LocalizedPage="TEAM" />
                 </div>
                <div class="card-body">
		<asp:Repeater ID="CategoryList" runat="server" OnItemCommand="CategoryList_ItemCommand">
			<HeaderTemplate>
			    <div class="table-responsive">
                    <table class="table">
			</HeaderTemplate>
            <ItemTemplate>
				<tr class="table-active">
					<td>
						<%# this.HtmlEncode(this.Eval( "Name"))%>
					</td>
					<td>
						<%# this.Eval( "SortOrder") %>
					</td>
					<td class="text-lg-right">
                    <YAF:ThemeButton ID="ThemeButtonEdit" Type="Info" CssClass="btn-sm"
                        CommandName='edit' CommandArgument='<%# this.Eval( "CategoryID") %>'
                        TitleLocalizedTag="EDIT" Icon="edit"
                        TextLocalizedTag="EDIT"
                        runat="server">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" CssClass="btn-sm"
                        OnLoad="DeleteCategory_Load"  CommandName='delete' CommandArgument='<%# this.Eval( "CategoryID") %>'
                        TitleLocalizedTag="DELETE" Icon="trash"
                        TextLocalizedTag="DELETE"
                        runat="server">
                    </YAF:ThemeButton>
                    </td>
				</tr>
				<asp:Repeater ID="ForumList" OnItemCommand="ForumList_ItemCommand" runat="server"
					DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>'>
					<ItemTemplate>
						<tr>
							<td>
								<strong>
									<%# this.HtmlEncode(DataBinder.Eval(Container.DataItem, "[\"Name\"]")) %></strong><br />
								<%# this.HtmlEncode(DataBinder.Eval(Container.DataItem, "[\"Description\"]")) %>
							</td>
							<td>
								<%# DataBinder.Eval(Container.DataItem, "[\"SortOrder\"]") %>
							</td>
							<td>
					    <span class="float-right">
                             <YAF:ThemeButton ID="btnEdit" Type="Info" CssClass="btn-sm"
                                 CommandName='edit' CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>'
                                 TextLocalizedTag="EDIT"
                                 TitleLocalizedTag="EDIT" Icon="edit" runat="server"></YAF:ThemeButton>
							 <YAF:ThemeButton ID="btnDuplicate" Type="Info" CssClass="btn-sm"
                                 CommandName='copy' CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>'
                                 TextLocalizedTag="COPY"
                                 TitleLocalizedTag="COPY" Icon="copy" runat="server"></YAF:ThemeButton>
                             <YAF:ThemeButton ID="btnDelete" Type="Danger" CssClass="btn-sm"
                                 CommandName='delete' CommandArgument='<%# this.Eval( "[\"ForumID\"]") %>'
                                 TextLocalizedTag="DELETE"
                                 TitleLocalizedTag="DELETE" Icon="trash" runat="server"></YAF:ThemeButton>

					    </span>
                    </td>
						</tr>
					</ItemTemplate>
				</asp:Repeater>
			</ItemTemplate>
            <FooterTemplate>
                </table></div>
            </FooterTemplate>
		</asp:Repeater>
                </div>
                <div class="card-footer text-lg-center">
				<YAF:ThemeButton ID="NewCategory" runat="server" OnClick="NewCategory_Click" Type="Primary"
				                 Icon="plus-square" TextLocalizedTag="NEW_CATEGORY"></YAF:ThemeButton>
				&nbsp;
				<YAF:ThemeButton ID="NewForum" runat="server" OnClick="NewForum_Click" Type="Primary"
				                 Icon="plus-square" TextLocalizedTag="NEW_FORUM"></YAF:ThemeButton>

                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>

<YAF:SmartScroller ID="SmartScroller1" runat="server" />
