<%@ Control language="c#" Codebehind="posts.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.posts" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Register TagPrefix="yaf" TagName="displaypost" Src="../controls/DisplayPost.ascx" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<a name=top></a>

<asp:repeater id=Poll runat="server" visible="false">
<HeaderTemplate>
<table class=content cellspacing=1 cellpadding=0 width=100%>
	<tr>
		<td class=header1 colspan=3><%= GetText("question") %>: <%# GetPollQuestion() %></td>
	</tr>
	<tr>
		<td class=header2><%= GetText("choice") %></td>
		<td class=header2 align=center width=10%><%= GetText("votes") %></td>
		<td class=header2 width=40%><%= GetText("statistics") %></td>
	</tr>
</HeaderTemplate>
<ItemTemplate>
	<tr>
		<td class=post>
		<yaf:mylinkbutton runat=server enabled=<%#CanVote%> commandname=vote commandargument='<%# DataBinder.Eval(Container.DataItem, "ChoiceID") %>' text='<%# DataBinder.Eval(Container.DataItem, "Choice") %>'/></td>
		<td class=post align=center><%# DataBinder.Eval(Container.DataItem, "Votes") %></td>
		<td class=post><nobr><img src='<%# GetThemeContents("VOTE","LCAP") %>'><img src='<%# GetThemeContents("VOTE","BAR") %>' height=12px width='<%# VoteWidth(Container.DataItem) %>%'><img src='<%# GetThemeContents("VOTE","RCAP") %>'></nobr> <%# DataBinder.Eval(Container.DataItem,"Stats") %>%</td>
	</tr>
</ItemTemplate>
<FooterTemplate>
</table><br/>
</FooterTemplate>
</asp:repeater>
<table class='command' cellspacing='0' cellpadding='0' width='100%'>
<tr>
	<td align=left class=navlinks><yaf:pager runat="server" id="Pager"/></td>
	<td align='right'>
		<asp:linkbutton id=PostReplyLink1 runat="server" cssclass="imagelink" ToolTip="Post Reply"/>
		<asp:linkbutton id=NewTopic1 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=DeleteTopic1 runat="server" onload="DeleteTopic_Load" cssclass="imagelink"/>
		<asp:linkbutton id=LockTopic1 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=UnlockTopic1 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=MoveTopic1 runat="server" cssclass="imagelink"/>
	</td>
</tr>
</table>

<table class=content cellSpacing=1 cellPadding=0 width="100%" border="0">
<tr class="header1">
	<td colspan="3">
		<table border="0" class="header1" width="100%" cellspacing="0" cellpadding="0">
		<tr>
			<td><asp:label id=TopicTitle runat="server"/></td>
			<td align="right">
				<asp:hyperlink id="MyTest" runat="server">Options</asp:hyperlink>
				<asp:placeholder runat="server" id="ViewOptions">
				&middot;
				<asp:hyperlink id="View" runat="server">View</asp:hyperlink>
				</asp:placeholder>
			</td>
		</tr>
		</table>
	</td>
</tr>
<tr class='header2'>
	<td colspan='3'>
		<table cellspacing='0' cellpadding='0' width='100%'>
		<tr>
			<td class='header2' align=left>
				<asp:linkbutton id=PrevTopic runat=server><%# GetText("prevtopic") %></asp:linkbutton>
				&middot;
				<asp:linkbutton id=NextTopic runat=server><%# GetText("nexttopic") %></asp:linkbutton>
			</td>
			<td class='header2' align=right runat="server" visible="false">
				<asp:linkbutton id=TrackTopic runat=server><%# GetText("watchtopic") %></asp:linkbutton>
				&middot;
				<asp:linkbutton id=EmailTopic runat=server><%# GetText("emailtopic") %></asp:linkbutton>
				&middot;
				<asp:linkbutton id=PrintTopic runat=server><%# GetText("printtopic") %></asp:linkbutton>
 				&middot;
 				<asp:hyperlink id=RssTopic runat=server><%# GetText("rsstopic") %></asp:hyperlink>
			</td>
		</tr>
		</table>
	</td>
</tr>

<asp:repeater id=MessageList runat="server">
<ItemTemplate>
	<%# GetThreadedRow(Container.DataItem) %>

	<yaf:displaypost runat="server" datarow=<%# Container.DataItem %> visible=<%#IsCurrentMessage(Container.DataItem)%> isthreaded=<%#IsThreaded%>/>
</ItemTemplate>
</asp:repeater>

<yaf:ForumUsers runat="server"/>

</table>
    
<table class=command cellSpacing=0 cellPadding=0 width="100%">
  <tr>
    <td align=left class=navlinks><yaf:pager runat="server" linkedpager="Pager"/></td>
    <td align=right>
		<asp:linkbutton id=PostReplyLink2 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=NewTopic2 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=DeleteTopic2 runat="server" onload="DeleteTopic_Load" cssclass="imagelink"/>
		<asp:linkbutton id=LockTopic2 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=UnlockTopic2 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=MoveTopic2 runat="server" cssclass="imagelink"/>
</td></tr>
</table>
    
<br>
<table cellSpacing=0 cellPadding=0 width="100%">
<tr>
	<td align=right><%= GetText("FORUM_JUMP") %> <yaf:forumjump runat="server"/></td>
</tr>
<tr>
	<td align="right" valign="top" class="smallfont"><yaf:PageAccess runat="server"/></td>
</tr>
</table>

<yaf:savescrollpos runat="server"/>

<yaf:PopMenu runat="server" id="MyTestMenu" control="MyTest"/>
<yaf:PopMenu runat="server" id="ViewMenu" control="View"/>
