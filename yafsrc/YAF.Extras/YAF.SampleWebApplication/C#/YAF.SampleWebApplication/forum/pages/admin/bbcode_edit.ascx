<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.bbcode_edit" Codebehind="bbcode_edit.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:adminmenu runat="server" id="Adminmenu1">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
              <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BBCODE_EDIT" />
            </td>
		</tr>
        <tr>
	      <td class="header2" colspan="2">
              <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HEADER1" LocalizedPage="ADMIN_BBCODE_EDIT" />
          </td>
		</tr>
		<tr>
			<td class="postheader" style="width: 20%">
              <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="BBCODE_NAME" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" style="width: 80%">
				<asp:textbox id="txtName" runat="server"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postheader" style="width: 20%">
              <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="EXEC_ORDER" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtExecOrder" Style="width: 50px" MaxLength="5" runat="server" Text="1"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader" style="width: 20%">
              <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="BBCODE_DESC" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>
		<tr>
		    <td class="header2" colspan="2">
               <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_BBCODE_EDIT" />
            </td>
		</tr>
		<tr>
			<td class="postheader" style="width: 20%">
              <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="SEARCH_REGEX" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtSearchRegEx" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>						
		<tr>
			<td class="postheader" style="width: 20%">
              <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="REPLACE_REGEX" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtReplaceRegEx" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader" style="width: 20%">
              <YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="USE_MODULE" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" style="width: 80%">
				<asp:CheckBox ID="chkUseModule" runat="server" /></td>
		</tr>		
		<tr>
			<td class="postheader" style="width: 20%">
              <YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="CLASS_NAME" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtModuleClass" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader" style="width: 20%">
              <YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="REPLACE_VARIABLES" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtVariables" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>		
		<tr>
		    <td class="header2" colspan="2">
              <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HEADER3" LocalizedPage="ADMIN_BBCODE_EDIT" />
            </td>
		</tr>						
		<tr>
			<td class="postheader" style="width: 20%">
              <YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="ONCLICK_JS" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtOnClickJS" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>						
		<tr>
			<td class="postheader" style="width: 20%">
              <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="DISPLAY_JS" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtDisplayJS" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>		
		<tr>
			<td class="postheader" style="width: 20%">
              <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="EDIT_JS" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtEditJS" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>						
		<tr>
			<td class="postheader" style="width: 20%">
              <YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="DISPLAY_CSS" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" style="width: 80%">
				<asp:TextBox ID="txtDisplayCSS" runat="server" TextMode="MultiLine"></asp:TextBox></td>
		</tr>		
		<tr>
			<td class="postfooter" align="center" colspan="2">
				<asp:Button id="save" runat="server"  OnClick="Add_Click" CssClass="pbutton"></asp:Button>
				<asp:Button id="cancel" runat="server"  CausesValidation="False" OnClick="Cancel_Click" CssClass="pbutton"></asp:Button></td>
		</tr>
	</table>
</YAF:adminmenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />
