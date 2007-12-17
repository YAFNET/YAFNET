<%@ Control Language="c#" CodeFile="index.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.moderate.index" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" cellspacing="1" cellpadding="0" width="100%">
	<tr>
		<td class="header1" colspan="4">
			<YAF:LocalizedLabel runat="server" LocalizedTag="FORUMS" />
		</td>
	</tr>
	<asp:Repeater ID="CategoryList" runat="server">
		<ItemTemplate>
			<tr>
				<td class="header2">
					<%# Eval( "Name") %>
				</td>
				<td class="header2" width="15%" align="center">
					<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="UNAPPROVED" />
				</td>
				<td class="header2" width="15%" align="center">
					<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="REPORTED" />
				</td>
				<td class="header2" width="15%" align="center">
				<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="REPORTEDSPAM" />
				</td>
			</tr>
			<asp:Repeater ID="ForumList" runat="server" OnItemCommand="ForumList_ItemCommand"
				DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>'>
				<ItemTemplate>
					<tr class="post">
						<td align="left">
							<b>
								<%# DataBinder.Eval(Container.DataItem, "[\"Name\"]") %>
							</b>
							<br />
							<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
						</td>
						<td align="center">
							<asp:LinkButton ID="ViewUnapprovedPostsBtn" runat='server' CommandName='viewunapprovedposts'
								CommandArgument='<%# Eval( "[\"ForumID\"]") %>' Text='<%# Eval( "[\"MessageCount\"]") %>'
								Font-Bold='<%# ((Convert.ToInt32(Eval( "[\"MessageCount\"]")) > 0) ? true : false) %>'></asp:LinkButton>
						</td>
						<td align="center">
							<asp:LinkButton ID="ViewReportedPostsBtn" runat='server' CommandName='viewreportedposts'
								CommandArgument='<%# Eval( "[\"ForumID\"]") %>' Text='<%# Eval( "[\"ReportCount\"]") %>'
								Font-Bold='<%# ((Convert.ToInt32(Eval( "[\"ReportCount\"]")) > 0) ? true : false) %>'></asp:LinkButton>
						</td>
						<td align="center">
							<asp:LinkButton ID="ViewReportedSpamBtn" runat='server' CommandName='viewreportedspam'
								CommandArgument='<%# Eval( "[\"ForumID\"]") %>' Text='<%# Eval( "[\"SpamCount\"]") %>'
								Font-Bold='<%# ((Convert.ToInt32(Eval( "[\"SpamCount\"]")) > 0) ? true : false) %>'></asp:LinkButton>
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</ItemTemplate>
	</asp:Repeater>
	<tr>
		<td class="postfooter" colspan="4">
			&nbsp;</td>
	</tr>
</table>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
