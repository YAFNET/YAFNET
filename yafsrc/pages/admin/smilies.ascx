<%@ Control language="c#" Inherits="yaf.pages.admin.smilies" CodeFile="smilies.ascx.cs" CodeFileBaseClass="yaf.AdminPage" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<asp:repeater runat=server id=List>
<HeaderTemplate>
	<table width=100% cellspacing=1 cellpadding=0 class=content>
	<tr>
		<td class=header1 colspan=5>Smilies</td>
	</tr>
	<tr>
		<td class=header2>Code</td>
		<td class=header2 align="center">Smile</td>
		<td class=header2>Emotion</td>
		<td class=header2>Commands</td>
	</tr>
</HeaderTemplate>
<ItemTemplate>
	<tr>
		<td class=post><%# DataBinder.Eval(Container.DataItem,"Code") %></td>
		<td class=post align="center"><img src='<%# yaf.Data.ForumRoot %>images/emoticons/<%# DataBinder.Eval(Container.DataItem,"Icon") %>'/></td>		
		<td class=post><%# DataBinder.Eval(Container.DataItem,"Emoticon") %></td>
		<td class=post>
			<asp:linkbutton runat="server" commandname="edit" commandargument='<%# DataBinder.Eval(Container.DataItem,"SmileyID") %>' text="Edit"/>
			|
			<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# DataBinder.Eval(Container.DataItem,"SmileyID") %>' text="Delete"/>
		</td>
	</tr>
</ItemTemplate>
<FooterTemplate>
	<tr>
		<td class=footer1 colSpan=4>
			<asp:linkbutton runat="server" commandname="add">Add Smiley</asp:linkbutton>
			|
			<asp:LinkButton runat="server" commandname="import">Import Smiley Pack</asp:LinkButton>
		</td>
	</tr>
	</table>
</FooterTemplate>
</asp:repeater>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
