<%@ Reference Page="~/error.aspx" %>
<%@ Control language="c#" Inherits="yaf.pages.error" CodeFile="error.ascx.cs" CodeFileBaseClass="yaf.pages.ForumPage" %>

<table class=content width=100% cellspacing=1 cellpadding=0>
<tr>
	<td class=header1>Error</td>
</tr>
<tr>
	<td class=post id=errormsg runat=server></td>
</tr>
</table>