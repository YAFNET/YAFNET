<%@ Page language="c#" Codebehind="posts.aspx.cs" AutoEventWireup="false" Inherits="yaf.posts" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat="server">

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
<asp:linkbutton runat=server commandname=vote commandargument='<%# DataBinder.Eval(Container.DataItem, "ChoiceID") %>'><%# DataBinder.Eval(Container.DataItem, "Choice") %></asp:linkbutton></td>
		<td class=post align=center><%# DataBinder.Eval(Container.DataItem, "Votes") %></td>
		<td class=post><nobr><img src='<%# GetThemeContents("VOTE","LCAP") %>'><img src='<%# GetThemeContents("VOTE","BAR") %>' height=12px width='<%# VoteWidth(Container.DataItem) %>%'><img src='<%# GetThemeContents("VOTE","RCAP") %>'></nobr> <%# DataBinder.Eval(Container.DataItem,"Stats") %>%</td>
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
		<asp:linkbutton id=PostReplyLink1 runat="server" cssclass="imagelink" ToolTip="Post Reply"/>
		<asp:linkbutton id=NewTopic1 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=DeleteTopic1 runat="server" onload="DeleteTopic_Load" cssclass="imagelink"/>
		<asp:linkbutton id=LockTopic1 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=UnlockTopic1 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=MoveTopic1 runat="server" cssclass="imagelink"/>
		</TD></TR></TABLE>
<table class=content cellSpacing=1 cellPadding=0 width="100%">
  <tr>
    <td class=header1 colSpan=2><asp:label id=TopicTitle runat="server"></asp:label></TD></TR>
  <tr>
    <td class=header2 colSpan=2>
      <table cellSpacing=0 cellPadding=0 width="100%">
        <tr>
			<td align=left>
				<asp:linkbutton id=PrevTopic runat=server><%# GetText("prevtopic") %></asp:linkbutton> | 
				<asp:linkbutton id=NextTopic runat=server><%# GetText("nexttopic") %></asp:linkbutton>
			</td>
          <td align=right><asp:linkbutton id=TrackTopic runat=server><%# GetText("watchtopic") %></asp:linkbutton> | 
          <asp:linkbutton id=EmailTopic runat=server><%# GetText("emailtopic") %></asp:linkbutton> | 
          <asp:linkbutton id=PrintTopic runat=server><%# GetText("printtopic") %></asp:linkbutton></TD></TR></TABLE></TD></TR>
          <asp:repeater id=MessageList runat="server">
<ItemTemplate>
		<tr class=postheader>
			<td width=140><a name='<%# DataBinder.Eval(Container.DataItem, "MessageID") %>'/><b><a href='profile.aspx?u=<%# DataBinder.Eval(Container.DataItem, "UserID") %>'><%# DataBinder.Eval(Container.DataItem, "UserName") %></a></b>
			</td>
			<td width=80%>
				<table cellspacing=0 cellpadding=0 width=100%>
					<tr>
						<td class=small align=left><b><%# GetText("posted") %>:</b> <%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %></td>
						<td align=right>
							<asp:linkbutton tooltip="Attachments" visible='<%# CanAttach((System.Data.DataRowView)Container.DataItem) %>' CommandName=Attach CommandArgument='<%# DataBinder.Eval(Container.DataItem, "MessageID") %>' runat="server"><%# GetThemeContents("BUTTONS","ATTACHMENTS") %></asp:linkbutton>
							<asp:linkbutton tooltip="Edit this post" visible='<%# CanEditPost((System.Data.DataRowView)Container.DataItem) %>' CommandName=Edit CommandArgument='<%# DataBinder.Eval(Container.DataItem, "MessageID") %>' runat="server"><%# GetThemeContents("BUTTONS","EDITPOST") %></asp:linkbutton>
							<asp:linkbutton tooltip="Delete this post" visible="<%# CanDeletePost((System.Data.DataRowView)Container.DataItem) %>" CommandName=Delete onload="DeleteMessage_Load" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "MessageID") %>' runat="server"><%# GetThemeContents("BUTTONS","DELETEPOST") %></asp:linkbutton>
							<asp:linkbutton tooltip="Reply with quote" visible="<%# ForumReplyAccess %>" CommandName=Quote CommandArgument='<%# DataBinder.Eval(Container.DataItem, "MessageID") %>' runat="server"><%# GetThemeContents("BUTTONS","QUOTEPOST") %></asp:linkbutton>
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
				<a href="javascript:scroll(0,0)"><%# GetText("top") %></a>
			</td>
			<td class="postfooter">
				<asp:linkbutton commandname=PM commandargument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>' runat=server><%# GetText("pm") %></asp:linkbutton>
			</td>
		</tr>
		<tr class="postsep"><td colspan="2" style="height:7px"></td></tr>
</ItemTemplate>
</asp:repeater>

<yaf:ForumUsers runat="server"/>

  <tr>
    <td class=footer1 colSpan=2>&nbsp;</TD></TR></TABLE>
    
<table class=command cellSpacing=0 cellPadding=0 width="100%">
  <tr>
    <td align=left class=navlinks id=PageLinks2 runat=server></td>
    <td align=right>
		<asp:linkbutton id=PostReplyLink2 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=NewTopic2 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=DeleteTopic2 runat="server" onload="DeleteTopic_Load" cssclass="imagelink"/>
		<asp:linkbutton id=LockTopic2 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=UnlockTopic2 runat="server" cssclass="imagelink"/>
		<asp:linkbutton id=MoveTopic2 runat="server" cssclass="imagelink"/>
</TD></TR>
</TABLE>
    
<br>
<table cellSpacing=0 cellPadding=0 width="100%">
  <tr>
    <td align=right>Forum Jump <yaf:forumjump runat="server"/></TD>
</TR>
<tr>
	<td align="right" valign="top" class="smallfont"><yaf:PageAccess runat="server"/></td>
	
</tr>
</TABLE>

<yaf:savescrollpos runat="server"/>
</FORM>
