<%@ Control language="c#" CodeFile="smilies.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.smilies" %>





<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">
<table class="command">
		<tr>
			<td class="navlinks">
				<YAF:pager id="Pager" runat="server"/></TD>
		</tr>
</table>
<asp:repeater runat=server id=List>
<HeaderTemplate>
	<table width="100%" cellspacing="1" cellpadding="0" class="content">
	<tr>
		<td class="header1" colspan="5">Smilies</td>
	</tr>
	<tr>
		<td class="header2">Code</td>
		<td class="header2" align="center">Smile</td>
		<td class="header2">Emotion</td>
		<td class="header2">Commands</td>
	</tr>
</HeaderTemplate>
<ItemTemplate>
	<tr>
		<td class="post"><%# Eval("Code") %></td>
		<td class="post" align="center"><img src="<%# YafForumInfo.ForumRoot %>images/emoticons/<%# Eval("Icon") %>"/></td>		
		<td class="post"><%# Eval("Emoticon") %></td>
		<td class="post">
			<asp:linkbutton runat="server" commandname="edit" commandargument='<%# Eval("SmileyID") %>' text="Edit"/>
			|
			<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# Eval("SmileyID") %>' text="Delete"/>
		</td>
	</tr>
</ItemTemplate>
<FooterTemplate>
	<tr>
		<td class="footer1" colSpan="4">
			<asp:Linkbutton runat="server" commandname="add">Add Smiley</asp:linkbutton>
			|
			<asp:LinkButton runat="server" commandname="import">Import Smiley Pack</asp:LinkButton>
		</td>
	</tr>
	</table>
</FooterTemplate>
</asp:repeater>
<table class="command">
		<tr>
			<td class="navlinks">
				<YAF:Pager id="Pager1" runat="server" linkedpager="Pager"/></TD>
		</tr>
	</table>
</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
