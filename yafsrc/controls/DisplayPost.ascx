<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DisplayPost.ascx.cs" Inherits="yaf.controls.DisplayPost" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

<tr class=postheader>
	<%#GetIndentCell()%>
	<td width="140px" colspan='<%#GetIndentSpan()%>'>
		<a name='<%# DataRow["MessageID"] %>'/>
		<b><a href='<%# yaf.Forum.GetLink(yaf.Pages.profile,"u={0}",DataRow["UserID"]) %>'><%# DataRow["UserName"] %></a></b>
	</td>
	<td width=80%>
		<table cellspacing=0 cellpadding=0 width=100%>
		<tr>
			<td>
				<b><%# ForumPage.GetText("POSTED") %>:</b> 
				<%# ForumPage.FormatDateTime((System.DateTime)DataRow["Posted"]) %>
			</td>
			<td align=right>
				<asp:hyperlink runat='server' id='Attach'/>
				<asp:hyperlink runat='server' id='Edit'/>
				<asp:linkbutton runat='server' id='Delete'/>
				<asp:hyperlink runat='server' id='Quote'/>
			</td>
		</tr>
		</table>
	</td>
</tr>
<tr class=post>
	<td valign="top" height="100" colspan='<%#GetIndentSpan()%>'>
		<%# FormatUserBox() %>
	</td>
	<td valign="top" class="message">
		<%# FormatBody() %>
	</td>
</tr>
<tr class=postfooter>
	<td class="small" colspan='<%#GetIndentSpan()%>'>
		<a href="javascript:scroll(0,0)"><%# ForumPage.GetText("TOP") %></a>
	</td>
	<td class="postfooter">
		<asp:hyperlink runat='server' id='Pm'/>
		<asp:linkbutton runat='server' id='Email' disabled='true'/>
		<asp:hyperlink runat='server' id='Home'/>
		<asp:linkbutton runat='server' id='Msn' disabled='true'/>
		<asp:hyperlink runat='server' id='Yim'/>
		<asp:hyperlink runat='server' id='Aim'/>
		<asp:hyperlink runat='server' id='Icq'/>
	</td>
</tr>
