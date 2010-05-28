<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.bbcode_edit" Codebehind="bbcode_edit.ascx.cs" %>
<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:adminmenu runat="server" id="Adminmenu1">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">Add/Edit BBCode Extension</td>
		</tr>
		<tr>
			<td class="postheader" style="width: 20%"><b>Name:</b><br />Required. Simple name of this extension.</td>
			<td class="post" style="width: 80%">
				<asp:textbox id="txtName" runat="server"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postheader" style="width: 20%"><b>Exec Order:</b><br />Required. Number representing in which order the extensions are executed. Lowest executed first.</td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtExecOrder" Style="width: 50px" MaxLength="5" runat="server" Text="1"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader" style="width: 20%"><b>Description:</b><br />Required. Description of this extension to the user.</td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>
		<tr>
		    <td class="header2" colspan="2">Regular Expression</td>
		</tr>
		<tr>
			<td class="postheader" style="width: 20%"><b>Search RegEx:</b><br />Required. Regular expression to find this BBCode. BBCode is always surrounded with []. This expression system is not designed for plain text searching (use the word replace for that).</td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtSearchRegEx" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>						
		<tr>
			<td class="postheader" style="width: 20%"><b>Replace RegEx:</b><br />Required. Regular expression for replacement of the search BBCode.</td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtReplaceRegEx" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader" style="width: 20%"><b>Use Module:</b><br />Use server-side module (class) instead of replace regex?</td>
			<td class="post" style="width: 80%">
				<asp:CheckBox ID="chkUseModule" runat="server" /></td>
		</tr>		
		<tr>
			<td class="postheader" style="width: 20%"><b>Module Class Name:</b><br />Full class name including namespace.</td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtModuleClass" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader" style="width: 20%"><b>Replace Variables:</b><br />Optional field. Separate variables with semi-colon; no other punctuation or spaces allowed.</td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtVariables" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>		
		<tr>
		    <td class="header2" colspan="2">Javascript &amp; CSS (all fields are optional)</td>
		</tr>						
		<tr>
			<td class="postheader" style="width: 20%"><b>OnClick JS:</b><br />JS code called when this BBCode extension is clicked client-side. Just enter javascript into this field. &lt;script&gt; will be added automatically.</td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtOnClickJS" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>						
		<tr>
			<td class="postheader" style="width: 20%"><b>Display JS:</b><br />JS code used to help with the extension display. Just enter javascript into this field. &lt;script&gt; will be added automatically.</td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtDisplayJS" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>		
		<tr>
			<td class="postheader" style="width: 20%"><b>Edit JS:</b><br />JS code used to modify the text editor. Use "{editorid}" (without quotes) to have the text editor ID replaced in the code. Just enter javascript into this field. &lt;script&gt; will be added automatically.</td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtEditJS" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>						
		<tr>
			<td class="postheader" style="width: 20%"><b>Display CSS:</b><br />CSS to help display this extension. Just enter CSS into this field. &lt;style&gt; will be added automatically.</td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtDisplayCSS" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>		
		<tr>
			<td class="postfooter" align="center" colspan="2">
				<asp:button id="save" runat="server" text="Save" OnClick="Add_Click"></asp:button>
				<asp:button id="cancel" runat="server" text="Cancel" CausesValidation="False" OnClick="Cancel_Click"></asp:button></td>
		</tr>
	</table>
</YAF:adminmenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />
