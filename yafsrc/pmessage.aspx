<%@ Page language="c#" Codebehind="pmessage.aspx.cs" AutoEventWireup="false" Inherits="yaf.pmessage" %>
<%@ Register TagPrefix="RichEdit" TagName="rte" Src="rte/rte.ascx" %>
<%@ Register TagPrefix="uc1" TagName="smileys" Src="controls/smileys.ascx" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat=server>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class=content width="100%" cellspacing=1 cellpadding=0>
	<tr>
		<td class=header1 colspan=2><%= GetText("title") %></td>
	</tr>
	<tr id=ToRow runat=server>
		<td width="30%" class=postheader><%= GetText("to") %></td>
		<td width="70%" class=post>
			<asp:TextBox id=To runat="server"/>
			<asp:DropDownList runat="server" id="ToList" visible="false"/>
			<asp:button runat="server" id="FindUsers"/>
		</td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("subject") %></td>
		<td class=post><asp:TextBox id=Subject runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader valign=top>
			<%= GetText("message") %>
			<uc1:smileys runat="server" onclick="insertsmiley"/>
		</td>
		<td class=post>
			<RichEdit:rte runat="server" id="Editor" cssclass="posteditor"/>
		</td>
	</tr>
	<tr>
		<td class=postfooter colspan=2 align=middle>
			<asp:Button id=Save runat="server"/>
			<asp:Button id=Cancel runat="server"/>
		</td>
	</tr>
</table>

<yaf:savescrollpos runat="server"/>
</form>
