<%@ Control language="c#" CodeFile="editaccessmask.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editaccessmask" %>





<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">

<table class=content cellSpacing=1 cellPadding=0 width="100%">
<tr class="header1">
	<td colspan="2">Edit Access Mask</td>
</tr>
<tr>
	<td class="postheader" width="50%"><b>Name:</b><br/>Name of this access mask.</td>
	<td class="post" width="50%"><asp:textbox runat="server" id="Name" cssclass="edit"/><asp:RequiredFieldValidator runat="server" Text="<br />Enter name please!" ControlToValidate="Name" Display="Dynamic" /></td>
</tr>
<tr>
	<td class="postheader"><b>Read Access:</b></td>
	<td class="post"><asp:checkbox runat="server" id="ReadAccess"/></td>
</tr>
<tr>
	<td class="postheader"><b>Post Access:</b></td>
	<td class="post"><asp:checkbox runat="server" id="PostAccess"/></td>
</tr>
<tr>
	<td class="postheader"><b>Reply Access:</b></td>
	<td class="post"><asp:checkbox runat="server" id="ReplyAccess"/></td>
</tr>
<tr>
	<td class="postheader"><b>Priority Access:</b></td>
	<td class="post"><asp:checkbox runat="server" id="PriorityAccess"/></td>
</tr>
<tr>
	<td class="postheader"><b>Poll Access:</b></td>
	<td class="post"><asp:checkbox runat="server" id="PollAccess"/></td>
</tr>
<tr>
	<td class="postheader"><b>Vote Access:</b></td>
	<td class="post"><asp:checkbox runat="server" id="VoteAccess"/></td>
</tr>
<tr>
	<td class="postheader"><b>Moderator Access:</b></td>
	<td class="post"><asp:checkbox runat="server" id="ModeratorAccess"/></td>
</tr>
<tr>
	<td class="postheader"><b>Edit Access:</b></td>
	<td class="post"><asp:checkbox runat="server" id="EditAccess"/></td>
</tr>
<tr>
	<td class="postheader"><b>Delete Access:</b></td>
	<td class="post"><asp:checkbox runat="server" id="DeleteAccess"/></td>
</tr>
<tr>
	<td class="postheader"><b>Upload Access:</b></td>
	<td class="post"><asp:checkbox runat="server" id="UploadAccess"/></td>
</tr>

<tr class="postfooter">
    <td align="middle" colspan="2">
		<asp:button id=Save runat="server" Text="Save" onclick="Save_Click" />
		<asp:Button id=Cancel runat="server" Text="Cancel" onclick="Cancel_Click" />
	</td>
</tr>
</table>

</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
