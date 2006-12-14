<%@ Control language="c#" Codebehind="version.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.version" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu id="adminmenu1" runat="server">

<table width=100% cellspacing=0 cellpadding=0 class=content>
<tr>
	<td class="post">
		<table width=100% cellspacing=0 cellpadding=0>
		<tr class=header2>
			<td>Version Check</td>
		</tr>
		<tr class="post">
			<td>
				<p>You are running Yet Another Forum.net version <%=YAF.Data.AppVersionName%>.</p>
				
				<p>The latest available version is <%=LastVersion%> released <%=LastVersionDate%>.</p>
				
				<p runat="server" id="Upgrade" visible="false">You can download the latest version from <a target="_top" href="http://sourceforge.net/project/showfiles.php?group_id=90539">here</a>.</p>
			</td>
		</tr>
		</table>
	</td>
</tr>
</table>

</YAF:adminmenu>
