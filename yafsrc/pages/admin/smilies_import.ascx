<%@ Control language="c#" Codebehind="smilies_import.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.smilies_import" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">

<table class=content width=100% cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan="2">Import Smiley Pack</td>
</tr>
<tr>
	<td class=postheader width=50%><b>Choose .pak file:</b><br/>You'll have to unpack the smiley package and upload all files to your smiley directory (images/emoticons).</td>
	<td class=post width=50%><asp:dropdownlist id=File runat="server"/></td>
</tr>
<tr>
	<td class=postheader width=50%><b>Delete existing smilies:</b><br/>Will delete all existing smilies if you check this.</td>
	<td class=post width=50%><asp:checkbox id=DeleteExisting runat="server"/></td>
</tr>
<tr>
	<td class=footer1 colspan="2" align=center>
		<asp:button id=import runat=server text="Import"/>
		<asp:button id=cancel runat=server text="Cancel"/>
	</td>
</tr>
</table>

</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
