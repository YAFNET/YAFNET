<%@ Page language="c#" Codebehind="cp_avatar.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp_avatar" %>

<form runat="server" enctype="multipart/form-data">

<p class=navlinks>
	<asp:hyperlink id=HomeLink runat="server">Home</asp:hyperlink>
	&#187; <asp:hyperlink id=UserLink runat="server">UserLink</asp:hyperlink>
	&#187; <a href="cp_editprofile.aspx">Edit Profile</a>
</p>


<table width="100%" class=content cellspacing=1 cellpadding=4>
	<tr>
		<td class=header1 colspan=2>Upload Avatar</td>
	</tr>
	<tr>
		<td class=post>Select picture:</td>
		<td class=post><input type="file" id="File" runat="server"/></td>
	</tr>

	<tr>
		<td class=footer1 colspan=2 align=middle>
			<asp:Button id=UpdateProfile runat="server" Text="Save"/>
		</td>
	</tr>
</table>

</form>
