<%@ Control language="c#" Codebehind="editrank.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.editrank" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class=content cellSpacing=1 cellPadding=0 width="100%">
	<tr>
		<td class=header1 colspan=11>Edit Rank</td>
	</tr>
  <tr>
    <td class=postheader colspan=4><b>Name:</b><br/>Name of this rank.</td>
    <td class=post colspan=7><asp:textbox style="width:300px" id=Name runat="server"/></td></tr>
 
  <tr>
    <td class=postheader colspan=4><b>Is Start:</b><br/>Means that this is the rank that new users belong to. Only one rank should have this checked.</td>
    <td class=post colspan=7><asp:checkbox id=IsStart runat="server"></asp:checkbox></td></tr>
    
  <tr>
    <td class=postheader colspan=4><b>Is Ladder Group:</b><br/>If this is checked, this rank should be part of the ladder system where users advance as they post messages.</td>
    <td class=post colspan=7><asp:checkbox id="IsLadder" runat="server"></asp:checkbox></td></tr>

	<tr>
		<td class=postheader colspan=4><b>Minimum Posts:</b><br/>Minimum number of posts before users are advanced to this rank.</td>
		<td class=post colspan=7><asp:textbox id="MinPosts" runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader colspan=4><b>Rank Image:</b><br/>This image will be shown next to users of this rank.</td>
		<td class=post colspan=7>
			<asp:dropdownlist id=RankImage runat="server" />
			<img align="absmiddle" runat=server id="Preview" />
		</td>
	</tr>

  <tr>
    <td class=postfooter align=middle colspan=11><asp:button id=Save runat="server" Text="Save"></asp:button>&nbsp; 
<asp:button id=Cancel runat="server" Text="Cancel"></asp:button></td></tr></table>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
