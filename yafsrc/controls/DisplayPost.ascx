<%@ Control Language="c#" AutoEventWireup="True" Codebehind="DisplayPost.ascx.cs" Inherits="YAF.Controls.DisplayPost" EnableViewState="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>

<tr class="postheader">
	<%#GetIndentCell()%>
	<td width="140px" id="NameCell" runat="server">
		<a name="<%# DataRow["MessageID"] %>"/>
		<b><asp:hyperlink id="UserName" runat="server" href='<%# YAF.Forum.GetLink(YAF.ForumPages.profile,"u={0}",DataRow["UserID"]) %>'><%# Server.HtmlEncode(DataRow["UserName"].ToString()) %></asp:hyperlink></b>
	</td>
	<td width="80%">
		<table cellspacing="0" cellpadding="0" width="100%">
		<tr>
			<td class="postheader">
			    <b><a href='<%# YAF.Forum.GetLink(YAF.ForumPages.posts,"m={0}#{0}",DataRow["MessageID"]) %>'> #<%# Convert.ToInt32((DataRow["Position"]))+1 %></a> <%# ForumPage.GetText("POSTED") %>:</b> 
			    <%# ForumPage.FormatDateTime((System.DateTime)DataRow["Posted"]) %>
			</td>
			<td class="postheader" align="right">
				<asp:hyperlink runat="server" id="Attach"/>
				<asp:hyperlink runat="server" id="Edit"/>
				<asp:hyperlink runat="server" id="MovePost"/>
				<asp:hyperlink runat="server" id="Delete"/>
				<asp:hyperlink runat="server" id="UnDelete"/>
				<asp:hyperlink runat="server" id="Quote"/>
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

<YAF:PopMenu runat="server" id="PopMenu1" control="UserName"/>
