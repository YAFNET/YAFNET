<%@ Page language="c#" Codebehind="version.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.version" %>

<form runat="server">

<table width=100% cellspacing=0 cellpadding=0 class=content>
<tr>
	<td class="post">
		<table width=100% cellspacing=0 cellpadding=0>
		<tr class=header2>
			<td>Version Check</td>
		</tr>
		<tr class="post">
			<td>
				<p>You are running Yet Another Forum.net version <%=yaf.pages.ForumPage.AppVersionName%>.</p>
				
				<p>The latest available version is <%=LastVersion%> released <%=LastVersionDate%>.</p>
				
				<p runat="server" id="Upgrade" visible="false">You can download the latest version from <a target="_top" href="http://sourceforge.net/project/showfiles.php?group_id=90539">here</a>.</p>
			</td>
		</tr>
		</table>
	</td>
</tr>
</table>

</form>
