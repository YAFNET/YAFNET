<%@ Register TagPrefix="uc1" TagName="forumjump" Src="forumjump.ascx" %>
<%@ Page language="c#" Codebehind="posts.aspx.cs" AutoEventWireup="false" Inherits="yaf.posts" %>

<form runat="server">

<p class=navlinks>
	<ASP:HYPERLINK id=HomeLink runat="server">Home</ASP:HYPERLINK>
	» <asp:hyperlink id=CategoryLink runat="server">CategoryLink</asp:hyperlink>
	» <asp:hyperlink id=ForumLink runat="server">ForumLink</asp:hyperlink> 
	» <asp:hyperlink id=TopicLink runat="server">TopicLink</asp:hyperlink>
</P>

<a name=top></a>

<asp:repeater id=Poll runat="server" visible="false">
<HeaderTemplate>
<table class=content cellspacing=1 cellpadding=0 width=100%>
	<tr>
		<td class=header1 colspan=3>Poll Question: <%# GetPollQuestion() %></td>
	</tr>
	<tr>
		<td class=header2>Choice</td>
		<td class=header2 align=center width=10%>Votes</td>
		<td class=header2 width=40%>Statistics</td>
	</tr>
</HeaderTemplate>
<ItemTemplate>
	<tr>
		<td class=post>
<asp:linkbutton runat=server commandname=vote commandargument='<%# DataBinder.Eval(Container.DataItem, "ChoiceID") %>'><%# DataBinder.Eval(Container.DataItem, "Choice") %></asp:linkbutton></td>
		<td class=post align=center><%# DataBinder.Eval(Container.DataItem, "Votes") %></td>
		<td class=post><nobr><img src='<%# ThemeFile("vote_lcap.gif") %>'><img src='<%# ThemeFile("voting_bar.gif") %>' height=12px width='<%# DataBinder.Eval(Container.DataItem,"Stats") %>%'><img src='<%# ThemeFile("vote_rcap.gif") %>'></nobr> <%# DataBinder.Eval(Container.DataItem,"Stats") %>%</td>
	</tr>
</ItemTemplate>
<FooterTemplate>
</table>
</FooterTemplate>
</asp:repeater>
<table class=command cellSpacing=0 cellPadding=0 width="100%">
  <tr>
    <td align=left class=navlinks id=PageLinks1 runat=server></td>
    <td align=right>
		<asp:linkbutton id=PostReplyLink1 runat="server" cssclass="imagelink" ToolTip="Post Reply"><img align=absmiddle title="Post reply" src='<%# ThemeFile("b_post_reply.png") %>'></asp:linkbutton>
		<asp:linkbutton id=NewTopic1 runat="server" cssclass="imagelink" ToolTip="New Topic"><img align=absmiddle title="Post new topic" src='<%# ThemeFile("b_post_topic.png") %>'></asp:linkbutton>
		<asp:linkbutton id=DeleteTopic1 runat="server" onload="DeleteTopic_Load" cssclass="imagelink"><img align=absmiddle title="Delete this topic" src='<%# ThemeFile("b_delete_topic.png") %>'></asp:linkbutton>
		<asp:linkbutton id=LockTopic1 runat="server" cssclass="imagelink"><img align=absmiddle title="Lock this topic" src='<%# ThemeFile("b_lock_topic.png") %>'></asp:linkbutton>
		<asp:linkbutton id=UnlockTopic1 runat="server" cssclass="imagelink"><img align=absmiddle title="Unlock this topic" src='<%# ThemeFile("b_unlock_topic.png") %>'></asp:linkbutton>
		<asp:linkbutton id=MoveTopic1 runat="server" cssclass="imagelink"><img align=absmiddle title="Move topic" src='<%# ThemeFile("b_move_topic.png") %>'></asp:linkbutton>
		</TD></TR></TABLE>
<table class=content cellSpacing=1 cellPadding=0 width="100%">
  <tr>
    <td class=header1 colSpan=2><asp:label id=TopicTitle runat="server"></asp:label></TD></TR>
  <tr>
    <td class=header2 colSpan=2>
      <table cellSpacing=0 cellPadding=0 width="100%">
        <tr>
          <td align=left><asp:linkbutton id=PrevTopic runat=server>Previous 
            Topic</asp:linkbutton> | <asp:linkbutton id=NextTopic runat=server>Next Topic</asp:linkbutton></TD>
          <td align=right><asp:linkbutton id=TrackTopic runat=server>Watch this topic</asp:linkbutton> | 
          <asp:linkbutton id=EmailTopic runat=server>Email this topic</asp:linkbutton> | 
          <asp:linkbutton id=PrintTopic runat=server>Print this topic</asp:linkbutton></TD></TR></TABLE></TD></TR>
          <asp:repeater id=MessageList runat="server">
<ItemTemplate>
		<tr class=postheader>
			<td width=140><a name='<%# DataBinder.Eval(Container.DataItem, "MessageID") %>'/><b><a href='profile.aspx?u=<%# DataBinder.Eval(Container.DataItem, "UserID") %>'><%# DataBinder.Eval(Container.DataItem, "UserName") %></a></b>
			</td>
			<td width=80%>
				<table cellspacing=0 cellpadding=0 width=100%>
					<tr>
						<td class=small align=left><b>Posted:</b> <%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %></td>
						<td align=right>
							<asp:linkbutton CommandName=Edit CommandArgument='<%# DataBinder.Eval(Container.DataItem, "MessageID") %>' runat="server"><img title="Edit this post" src='<%# ThemeFile("b_edit_post.png") %>'></asp:linkbutton>
							<asp:linkbutton CommandName=Delete onload="DeleteMessage_Load" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "MessageID") %>' runat="server"><img title="Delete this post" src='<%# ThemeFile("b_delete_post.png") %>'></asp:linkbutton>
							<asp:linkbutton CommandName=Quote CommandArgument='<%# DataBinder.Eval(Container.DataItem, "MessageID") %>' runat="server"><img title="Reply with quote" src='<%# ThemeFile("b_quote_post.png") %>'></asp:linkbutton>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr class=post>
			<td valign=top height=100>
				<%# FormatUserBox((System.Data.DataRowView)Container.DataItem) %>
			</td>
			<td valign=top class="message">
				<%# FormatBody(Container.DataItem) %>
			</td>
		</tr>
		<tr class=postfooter>
			<td class=small>
				<a href="javascript:scroll(0,0)">Back to top</a>
			</td>
			<td class="postfooter">
				<asp:linkbutton commandname=PM commandargument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>' runat=server>PM</asp:linkbutton>
			</td>
		</tr>
</ItemTemplate>

<SeparatorTemplate>
		<tr class="postsep"><td colspan="2" style="height:7px"></td></tr>
</SeparatorTemplate>
</asp:repeater>
  <tr>
    <td class=footer1 colSpan=2>&nbsp;</TD></TR></TABLE>
    
<table class=command cellSpacing=0 cellPadding=0 width="100%">
  <tr>
    <td align=left class=navlinks id=PageLinks2 runat=server></td>
    <td align=right>
		<asp:linkbutton id=PostReplyLink2 runat="server" cssclass="imagelink" ToolTip="Post Reply"><img align=absmiddle title="Post reply" src='<%# ThemeFile("b_post_reply.png") %>'></asp:linkbutton>
		<asp:linkbutton id=NewTopic2 runat="server" cssclass="imagelink" ToolTip="New Topic"><img align=absmiddle title="Post new topic" src='<%# ThemeFile("b_post_topic.png") %>'></asp:linkbutton>
		<asp:linkbutton id=DeleteTopic2 runat="server" onload="DeleteTopic_Load" cssclass="imagelink"><img align=absmiddle title="Delete this topic" src='<%# ThemeFile("b_delete_topic.png") %>'></asp:linkbutton>
		<asp:linkbutton id=LockTopic2 runat="server" cssclass="imagelink"><img align=absmiddle title="Lock this topic" src='<%# ThemeFile("b_lock_topic.png") %>'></asp:linkbutton>
		<asp:linkbutton id=UnlockTopic2 runat="server" cssclass="imagelink"><img align=absmiddle title="Unlock this topic" src='<%# ThemeFile("b_unlock_topic.png") %>'></asp:linkbutton>
		<asp:linkbutton id=MoveTopic2 runat="server" cssclass="imagelink"><img align=absmiddle title="Move topic" src='<%# ThemeFile("b_move_topic.png") %>'></asp:linkbutton>
</TD></TR>
</TABLE>
    
<br>
<table cellSpacing=0 cellPadding=0 width="100%">
  <tr>
    <td align=right>Forum Jump <uc1:forumjump id=Forumjump1 runat="server"></uc1:forumjump></TD>
</TR>
<tr>
	<td align=right valign=top id=AccessCell class=smallfont runat=server></td>
</tr>
</TABLE>

</FORM>
