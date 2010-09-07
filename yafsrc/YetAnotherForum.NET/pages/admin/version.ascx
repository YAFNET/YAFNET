<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.version" Codebehind="version.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="adminmenu1" runat="server">
	<table width="100%" cellspacing="0" cellpadding="0" class="content">
		<tr>
			<td class="header1">
				Version Check
			</td>
		</tr>
		<tr class="post">
			<td>
				<p>
					You are running YetAnotherForum.NET version <strong>
						<%#YafForumInfo.AppVersionName%></strong> (Date:
					<%#YafServices.DateTime.FormatDateShort( YafForumInfo.AppVersionDate ) %>).</p>
				<p>
					The latest final version available is <strong>
						<%#LastVersion%></strong> released <strong>
							<%#LastVersionDate%></strong> .</p>
				<p runat="server" id="Upgrade" visible="false">
					You can download the latest version from <a target="_top" href="http://sourceforge.net/project/showfiles.php?group_id=90539">
						here</a>.</p>
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
