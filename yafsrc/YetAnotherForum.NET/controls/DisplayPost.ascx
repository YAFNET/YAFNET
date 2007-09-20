<%@ Control Language="c#" AutoEventWireup="True" CodeFile="DisplayPost.ascx.cs" Inherits="YAF.Controls.DisplayPost"
	EnableViewState="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<tr class="postheader">
	<%#GetIndentCell()%>
	<td width="140px" id="NameCell" runat="server">
		<a name="<%# DataRow["MessageID"] %>" />
		<b>
		<YAF:UserLink ID="UserProfileLink" runat="server" UserID='<%#DataRow["UserID"]%>' UserName='<%#DataRow["UserName"]%>' />
		</b>
	</td>
	<td width="80%">
		<table cellspacing="0" cellpadding="0" width="100%">
			<tr>
				<td class="postheader">
					<b><a href='<%# YAF.Classes.Utils.YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.posts,"m={0}#{0}",DataRow["MessageID"]) %>'>
						#<%# Convert.ToInt32((DataRow["Position"]))+1 %></a>
						<%# PageContext.Localization.GetText("POSTED") %>
						:</b>
					<%# YafDateTime.FormatDateTime((System.DateTime)DataRow["Posted"]) %>
				</td>
				<td class="postheader" align="right">
					<asp:HyperLink runat="server" ID="Attach" />
					<asp:HyperLink runat="server" ID="Edit" />
					<asp:HyperLink runat="server" ID="MovePost" />
					<asp:HyperLink runat="server" ID="Delete" />
					<asp:HyperLink runat="server" ID="UnDelete" />
					<asp:HyperLink runat="server" ID="Quote" />
				</td>
			</tr>
		</table>
	</td>
</tr>
<tr class="<%#GetPostClass()%>">
	<td valign="top" height="<%# GetUserBoxHeight() %>" class="UserBox" colspan='<%#GetIndentSpan()%>'>
		<%# FormatUserBox() %>
	</td>
	<td valign="top" class="message">
		<div class="postdiv">
			<%# FormatBody() %>
		</div>
	</td>
</tr>
<tr class="postfooter">
	<td class="small" colspan='<%#GetIndentSpan()%>'>
		<a href="javascript:scroll(0,0)">
			<%# PageContext.Localization.GetText("TOP") %>
		</a>
	</td>
	<td class="postfooter">
		<table border="0" cellpadding="0" cellspacing="0" width="100%">
			<tr>
				<td>
					<asp:HyperLink runat='server' ID='Pm' />
					<asp:HyperLink runat='server' ID='Email' />
					<asp:HyperLink runat='server' ID='Home' />
					<asp:HyperLink runat='server' ID='Blog' />
					<asp:HyperLink runat='server' ID='Msn' />
					<asp:HyperLink runat='server' ID='Yim' />
					<asp:HyperLink runat='server' ID='Aim' />
					<asp:HyperLink runat='server' ID='Icq' />
				</td>
				<td align="right">
					&nbsp;<asp:LinkButton ID="ReportButton" CommandName="ReportAbuse" CommandArgument='<%# DataRow["MessageID"] %>'
						runat="server"></asp:LinkButton>
					<asp:LinkButton ID="ReportSpamButton" CommandName="ReportSpam" CommandArgument='<%# DataRow["MessageID"] %>'
						runat="server"></asp:LinkButton>
					<span id="AdminInformation" runat="server" class="smallfont"></span>
				</td>
			</tr>
		</table>
	</td>
</tr>
<tr class="postsep">
	<td colspan="3" style="height: 5px">
	</td>
</tr>
<YAF:PopMenu runat="server" ID="PopMenu1" Control="UserName" />
