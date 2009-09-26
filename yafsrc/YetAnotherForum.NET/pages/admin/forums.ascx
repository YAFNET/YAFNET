<%@ Control Language="c#" CodeFile="forums.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.forums" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="3">
				Forums
			</td>
		</tr>
		<asp:Repeater ID="CategoryList" runat="server">
			<ItemTemplate>
				<tr>
					<td class="header2">
						<%# Eval( "Name") %>
					</td>
					<td class="header2" width="10%" align="center">
						<%# Eval( "SortOrder") %>
					</td>
					<td class="header2" width="15%" style="font-weight: normal">
						<asp:LinkButton runat='server' CommandName='edit' CommandArgument='<%# Eval( "CategoryID") %>'>Edit</asp:LinkButton>
						|
						<asp:LinkButton runat='server' OnLoad="DeleteCategory_Load" CommandName='delete'
							CommandArgument='<%# Eval( "CategoryID") %>'>Delete</asp:LinkButton>
					</td>
				</tr>
				<asp:Repeater ID="ForumList" OnItemCommand="ForumList_ItemCommand" runat="server"
					DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>'>
					<ItemTemplate>
						<tr class="post">
							<td align="left">
								<b>
									<%# DataBinder.Eval(Container.DataItem, "[\"Name\"]") %></b><br />
								<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
							</td>
							<td align="center">
								<%# DataBinder.Eval(Container.DataItem, "[\"SortOrder\"]") %>
							</td>
							<td>
								<asp:LinkButton runat='server' CommandName='edit' CommandArgument='<%# Eval( "[\"ForumID\"]") %>'>Edit</asp:LinkButton>
								|
								<asp:LinkButton runat='server' OnLoad="DeleteForum_Load" CommandName='delete' CommandArgument='<%# Eval( "[\"ForumID\"]") %>'>Delete</asp:LinkButton>
							</td>
						</tr>
					</ItemTemplate>
				</asp:Repeater>
			</ItemTemplate>
		</asp:Repeater>
		<tr>
			<td class="footer1" colspan="3">
				<asp:LinkButton ID="NewCategory" runat="server" OnClick="NewCategory_Click">New Category</asp:LinkButton>
				|
				<asp:LinkButton ID="NewForum" runat="server" OnClick="NewForum_Click">New Forum</asp:LinkButton>
			</td>
		</tr>
	</table>
</YAF:AdminMenu>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
	<ContentTemplate>
		<asp:Timer ID="UpdateStatusTimer" runat="server" Enabled="false" Interval="4000" OnTick="UpdateStatusTimer_Tick" />
	
	</ContentTemplate>
</asp:UpdatePanel>

<div>
	<div id="DeleteForumMessage" style="display:none" class="ui-overlay">
		<div class="ui-widget ui-widget-content ui-corner-all">
		<h2>
			Deleting Forum</h2>
		<p>Please do not navigate away from this page while the deletion is in progress...</p>
		<div align="center">
			<asp:Image ID="LoadingImage" runat="server" alt="Processing..." />
		</div>
		<br />
		</div>
	</div>
</div>

<YAF:SmartScroller ID="SmartScroller1" runat="server" />
