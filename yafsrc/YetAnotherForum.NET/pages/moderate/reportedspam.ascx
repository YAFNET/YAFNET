<%@ Control Language="C#" AutoEventWireup="true" CodeFile="reportedspam.ascx.cs"
	Inherits="YAF.Pages.moderate.reportedspam" %>
<%@ Import Namespace="YAF.Classes.Core"%>
<YAF:PageLinks ID="PageLinks" runat="server" />
<table cellpadding="0" cellspacing="1" class="content" width="100%">
	<asp:Repeater ID="List" runat="server">
	<HeaderTemplate>
		
			<tr>
				<td colspan="2" class="header1" align="left">
					<%# PageContext.PageForumName %>
					-
					<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="REPORTEDSPAM" />
				</td>
			</tr>
	</HeaderTemplate>
	<FooterTemplate>
		<tr>
			<td class="postfooter" colspan="2">
				&nbsp;</td>
		</tr>
		
	</FooterTemplate>
	<ItemTemplate>
		<tr class="header2">
			<td colspan="2">
				<%# Eval("Topic") %>
			</td>
		</tr>
		<tr class="postheader">
			<td>
				<YAF:UserLink ID="UserLink1" runat="server" UserID='<%# Convert.ToInt32(Eval("UserID")) %>'
					UserName='<%# Convert.ToString(Eval("UserName")) %>' />
			</td>
			<td>
				<b>
					<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="POSTED" />
				</b>
				<%# YafServices.DateTime.FormatDateTime((System.DateTime) DataBinder.Eval(Container.DataItem, "[\"Posted\"]")) %>
				<b>
					<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NUMBERREPORTED" />
				</b>
				<%# DataBinder.Eval(Container.DataItem, "[\"NumberOfReports\"]") %>
				<label id="Label1" runat="server" visible='<%# General.CompareMessage(DataBinder.Eval(Container.DataItem, "[\"OriginalMessage\"]"),DataBinder.Eval(Container.DataItem, "[\"Message\"]"))%>'>
					<b>
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MODIFIED" />
					</b>
				</label>			
			</td>
		</tr>
		<tr  class="postheader">
		<td>
			<YAF:LocalizedLabel ID="ReportedByLabel" runat="server" LocalizedTag="REPORTEDBY" />			 
		</td>
		<td>
			<YAF:ReportedPosts id="ReportersList"  runat="server" MessageID='<%# DataBinder.Eval(Container.DataItem, "[\"MessageID\"]") %>' />					
		</td>
		</tr>			
		
		<tr class="post">
			<td valign="top" width="140">
				&nbsp;</td>
			<td valign="top" class="message">
			    <%# FormatMessage((System.Data.DataRowView)Container.DataItem)%>
			</td>
		</tr>
		<tr class="postfooter">
			<td class="small">
				<a href="javascript:scroll(0,0)">
					<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="TOP" />
				</a>
			</td>
			<td class="postfooter" style="float:left">					
				<YAF:ThemeButton ID="CopyOverBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="COPYOVER"
					CommandName="CopyOver" Visible='<%# General.CompareMessage(DataBinder.Eval(Container.DataItem, "[\"OriginalMessage\"]"),DataBinder.Eval(Container.DataItem, "[\"Message\"]"))%>'
					CommandArgument='<%# Eval("MessageID") %>' />
					
				<YAF:ThemeButton ID="DeleteBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="DELETE"				
					CommandName="Delete" CommandArgument='<%# Eval("MessageID") %>' OnLoad="Delete_Load" />
					
				<YAF:ThemeButton ID="ResolveBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="RESOLVED"
					CommandName="Resolved" CommandArgument='<%# Eval("MessageID") %>' />
					
			    <YAF:ThemeButton ID="ViewBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="VIEW" 
					CommandName="View" CommandArgument='<%# Eval("MessageID") %>' />		
			</td>
		</tr>
	</ItemTemplate>
	<SeparatorTemplate>
		<tr class="postsep">
			<td colspan="2" style="height: 7px">
			</td>
		</tr>
	</SeparatorTemplate>
	</asp:Repeater>
</table>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
