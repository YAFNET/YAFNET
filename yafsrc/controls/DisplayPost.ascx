<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DisplayPost.ascx.cs" Inherits="yaf.controls.DisplayPost" EnableViewState="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<tr class=postheader>
	<%#GetIndentCell()%>
	<td width="140px" id="NameCell" runat="server">
		<a name='<%# DataRow["MessageID"] %>'/>
		<b><asp:hyperlink id="UserName" runat="server" href='<%# yaf.Forum.GetLink(yaf.Pages.profile,"u={0}",DataRow["UserID"]) %>'><%# DataRow["UserName"] %></asp:hyperlink></b>
	</td>
	<td width=80%>
		<table cellspacing=0 cellpadding=0 width=100%>
		<tr>
			<td class=postheader>
				<b><%# ForumPage.GetText("POSTED") %>:</b> 
				<%# ForumPage.FormatDateTime((System.DateTime)DataRow["Posted"]) %>
			</td>
			<td class=postheader align=right>
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
		<table border="0" cellpadding="0" cellspacing="0" width="100%">
		<tr>
			<td>
			<asp:hyperlink runat='server' id='Pm'/>
			<asp:hyperlink runat='server' id='Email'/>
			<asp:hyperlink runat='server' id='Home'/>
			<asp:hyperlink runat='server' id='Blog'/>
			<asp:hyperlink runat='server' id='Msn'/>
			<asp:hyperlink runat='server' id='Yim'/>
			<asp:hyperlink runat='server' id='Aim'/>
			<asp:hyperlink runat='server' id='Icq'/>
			</td>
			<td align="right" id="AdminInfo" runat="server">&nbsp;</td>
		</tr>
		</table>
	</td>
</tr>
<tr class="postsep"><td colspan="3" style="height:5px"></td></tr>

<yaf:PopMenu runat="server" id="PopMenu1" control="UserName"/>
