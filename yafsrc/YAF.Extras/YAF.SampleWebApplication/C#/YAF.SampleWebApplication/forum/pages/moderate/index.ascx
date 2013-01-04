<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.moderate.index" Codebehind="index.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" cellspacing="1" cellpadding="0" width="100%">
	<tr>
		<td class="header1" colspan="3">
			<YAF:LocalizedLabel runat="server" LocalizedTag="MODERATEINDEX_TITLE" />
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
				<YAF:LocalizedLabel ID="ReportedCountLabel" runat="server" LocalizedTag="REPORTED" />
				</td>
			</tr>
			<asp:Repeater ID="ForumList" runat="server" OnItemCommand="ForumList_ItemCommand"
				DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>'>
				<ItemTemplate>
					<tr class="post">
						<td align="left">
							<strong>
							<%# DataBinder.Eval(Container.DataItem, "[\"Name\"]") %>
							</strong>
							<br />
							<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
						</td>
						<td align="center">
							<asp:LinkButton ID="ViewUnapprovedPostsBtn" runat='server' CommandName='viewunapprovedposts'
								CommandArgument='<%# Eval( "[\"ForumID\"]") %>' Text='<%# Eval( "[\"MessageCount\"]") %>'
								Font-Bold='<%# ((Convert.ToInt32(Eval( "[\"MessageCount\"]")) > 0) ? true : false) %>'></asp:LinkButton>
						</td>				
						<td align="center">
							<asp:LinkButton ID="ViewReportedBtn" runat='server' CommandName='viewreportedposts'
								CommandArgument='<%# Eval( "[\"ForumID\"]") %>' Text='<%# Eval( "[\"ReportedCount\"]") %>'
								Font-Bold='<%# ((Convert.ToInt32(Eval( "[\"ReportedCount\"]")) > 0) ? true : false) %>'></asp:LinkButton>
						</td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</ItemTemplate>
	</asp:Repeater>
    <asp:PlaceHolder id="InfoPlaceHolder" runat="server" Visible="false">
      <tr class="post">
        <td style="text-align:center">
          <em><YAF:LocalizedLabel ID="NoCountInfo" LocalizedTag="NOMODERATION" LocalizedPage="MODERATE" runat="server"></YAF:LocalizedLabel></em>
        </td>
      </tr>
    </asp:PlaceHolder>
	  <tr>
		<td class="footer1" colspan="3">
			&nbsp;</td>
	  </tr>
</table>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
