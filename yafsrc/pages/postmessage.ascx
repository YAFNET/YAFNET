<%@ Control language="c#" Codebehind="postmessage.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.postmessage" %>
<%@ Register TagPrefix="uc1" TagName="smileys" Src="../controls/smileys.ascx" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Register TagPrefix="rte" Namespace="yaf" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class="rte" cellSpacing="1" cellPadding="4" width="100%" align="center">
	<tr>
		<td class=header1 align=middle colSpan=2><asp:label id=Title runat="server"/></td>
	</tr>

	<tr id=PreviewRow runat=server visible=false>
		<td class=postheader valign=top><%= GetText("previewtitle") %></td>
		<td class=post valign=top id=PreviewCell runat=server></td>
	</tr>

	<tr id=SubjectRow runat="server">
		<td class=postheader width="30%"><%= GetText("subject") %></td>
		<td class=post width="60%"><asp:textbox id=Subject runat="server" cssclass="edit"/></td>
	</tr>
	<tr id=FromRow runat="server">
		<td class=postheader width="30%"><%= GetText("from") %></td>
		<td class=post width="60%"><asp:textbox id="From" runat="server" cssclass="edit"/></td>
	</tr>
	<tr id=PriorityRow runat="server">
		<td class=postheader width="30%"><%= GetText("priority") %></td>
		<td class=post width="60%">
			<asp:dropdownlist id=Priority runat="server"/>
		</td>
	</tr>
	<tr id=CreatePollRow runat="server">
		<td class="postheader" width="30%"><asp:linkbutton id=CreatePoll runat="server"/></td>
		<td class="post" width="60%">&nbsp;</td>
	</tr>
	<tr id=PollRow1 runat="server" visible="false">
		<td class="postheader" width="30%"><em><%= GetText("pollquestion") %></em></td>
		<td class="post" width="60%"><asp:textbox maxlength="50" id=Question runat="server" cssclass="edit"/></td>
	</tr>
	<tr id=PollRow2 runat="server" visible="false">
		<td class="postheader" width="30%"><em><%= GetText("choice1") %></em></td>
		<td class="post" width="60%"><asp:TextBox maxlength="50" id=PollChoice1 runat="server" cssclass="edit"/></td>
	</tr>
	<tr id="PollRow3" runat="server" visible=false>
		<td class="postheader" width="30%"><em><%= GetText("choice2") %></em></td>
		<td class="post" width="60%"><asp:TextBox maxlength="50" id=PollChoice2 runat="server" cssclass="edit"/></td>
	</tr>
	<tr id="PollRow4" runat="server" visible=false>
		<td class="postheader" width="30%"><em><%= GetText("choice3") %></em></td>
		<td class="post" width="60%"><asp:TextBox maxlength="50" id=PollChoice3 runat="server" cssclass="edit"/></td>
	</tr>
	<tr id="PollRow5" runat="server" visible=false>
		<td class="postheader" width="30%"><em><%= GetText("choice4") %></em></td>
		<td class="post" width="60%"><asp:TextBox maxlength="50" id=PollChoice4 runat="server" cssclass="edit"/></td>
	</tr>
	<tr id="PollRow6" runat="server" visible=false>
		<td class="postheader" width="30%"><em><%= GetText("choice5") %></em></td>
		<td class="post" width="60%"><asp:TextBox maxlength="50" id=PollChoice5 runat="server" cssclass="edit"/></td>
	</tr>
	<tr id="PollRow7" runat="server" visible=false>
		<td class="postheader" width="30%"><em><%= GetText("choice6") %></em></td>
		<td class="post" width="60%"><asp:TextBox maxlength="50" id=PollChoice6 runat="server" cssclass="edit"/></td>
	</tr>
	<tr id="PollRow8" runat="server" visible=false>
		<td class="postheader" width="30%"><em><%= GetText("choice7") %></em></td>
		<td class="post" width="60%"><asp:TextBox maxlength="50" id=PollChoice7 runat="server" cssclass="edit"/></td>
	</tr>
	<tr id="PollRow9" runat="server" visible=false>
		<td class="postheader" width="30%"><em><%= GetText("choice8") %></em></td>
		<td class="post" width="60%"><asp:TextBox maxlength="50" id=PollChoice8 runat="server" cssclass="edit"/></td>
	</tr>
	<tr id="PollRow10" runat="server" visible=false>
		<td class="postheader" width="30%"><em><%= GetText("choice9") %></em></td>
		<td class="post" width="60%"><asp:TextBox maxlength="50" id=PollChoice9 runat="server" cssclass="edit"/></td>
	</tr>
	<tr>
		<td class="postheader" vAlign=top><%= GetText("message") %>
			<uc1:smileys runat="server" onclick="insertsmiley"/>
		</td>
		<td class="post" id="RichEdit">
			<rte:RichEdit runat="server" id="Message" />
		</td>
	</tr>
	<!--tr>
		<td class=postheader>&nbsp;</td>
		<td class=post>
			<input type=button value=" B " style="font-weight:bold" onclick="makebold()">
			<input type=button value=" I " style="font-weight:bold;font-style:italic" onclick="makeitalic()">
			<input type=button value=" U " style="font-weight:bold;text-decoration:underline" onclick="makeunderline()">
			<input type=button value=" URL " onclick="makeurl()">
			<input type=button value=" QUOTE " onclick="makequote()">
			<input type=button value=" IMG " onclick="makeimg()">
			<input type=button value=" CODE " onclick="makecode()">
		</td>
	</tr-->
	<tr>
		<td align=middle colSpan=2 class=footer1>
			<asp:Button id=Preview runat="server"/>
			<asp:button id=PostReply runat="server"/>
			<asp:Button id=Cancel runat="server"/>
		</td>
	</tr>
</table>

<br/>

<asp:repeater id="LastPosts" runat="server" visible="false">
<HeaderTemplate>
	<table class="content" cellSpacing="1" cellPadding="0" width="100%" align="center">
		<tr>
			<td class=header2 align=middle colSpan=2><%# GetText("last10") %></td>
		</tr>
</HeaderTemplate>
<FooterTemplate>
	</table>
</FooterTemplate>
<ItemTemplate>
		<tr class=postheader>
			<td width=140><b><a href='<%# yaf.Forum.GetLink(yaf.Pages.profile,"u={0}",DataBinder.Eval(Container.DataItem, "UserID")) %>'><%# DataBinder.Eval(Container.DataItem, "UserName") %></a></b>
			</td>
			<td width=80% class=small align=left><b><%# GetText("posted") %></b> <%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %></td>
		</tr>
		<tr class=post>
			<td>&nbsp;</td>
			<td valign=top class="message">
				<%# FormatBody(Container.DataItem) %>
			</td>
		</tr>
</ItemTemplate>
</asp:repeater>

<yaf:savescrollpos runat="server"/>
