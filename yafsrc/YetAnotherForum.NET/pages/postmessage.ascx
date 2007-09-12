<%@ Control Language="c#" CodeFile="postmessage.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.postmessage" %>
<%@ Register TagPrefix="uc1" TagName="smileys" Src="../controls/smileys.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" cellspacing="1" cellpadding="4" width="100%" align="center">
  <tr>
    <td class="header1" align="center" colspan="2">
      <asp:Label ID="Title" runat="server" /></td>
  </tr>
  <tr id="PreviewRow" runat="server" visible="false">
    <td class="postformheader" valign="top">
      <%= GetText("previewtitle") %>
    </td>
    <td class="post" valign="top" id="PreviewCell" runat="server">
    </td>
  </tr>
  <tr id="SubjectRow" runat="server">
    <td class="postformheader" width="20%">
      <%= GetText("subject") %>
    </td>
    <td class="post" width="80%">
      <asp:TextBox ID="Subject" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr id="BlogRow" visible="false" runat="server">
    <td class="postformheader" width="20%">
      Post to blog?</td>
    <td class="post" width="80%">
      <asp:CheckBox ID="PostToBlog" runat="server" />
      Blog Password:
      <asp:TextBox ID="BlogPassword" runat="server" TextMode="Password" Width="400" /><asp:HiddenField
        ID="BlogPostID" runat="server" />
    </td>
  </tr>
  <tr id="FromRow" runat="server">
    <td class="postformheader" width="20%">
      <%= GetText("from") %>
    </td>
    <td class="post" width="80%">
      <asp:TextBox ID="From" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr id="PriorityRow" runat="server">
    <td class="postformheader" width="20%">
      <%= GetText("priority") %>
    </td>
    <td class="post" width="80%">
      <asp:DropDownList ID="Priority" runat="server" />
    </td>
  </tr>
  <tr id="PersistencyRow" runat="server">
    <td class="postformheader" width="20%">
      <%= GetText("PERSISTENCY")%>
    </td>
    <td class="post" width="80%">
      <asp:CheckBox ID="Persistency" runat="server" /> (<%= GetText("PERSISTENCY_INFO") %>)
    </td>
  </tr>
  <tr id="CreatePollRow" runat="server">
    <td class="postformheader" width="20%">
      <asp:LinkButton ID="CreatePoll" runat="server" OnClick="CreatePoll_Click" /></td>
    <td class="post" width="80%">
      &nbsp;</td>
  </tr>
  <tr id="PollRow1" runat="server" visible="false">
    <td class="postformheader" width="20%">
      <em>
        <%= GetText("pollquestion") %>
      </em>
    </td>
    <td class="post" width="80%">
      <asp:TextBox MaxLength="50" ID="Question" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr id="PollRow2" runat="server" visible="false">
    <td class="postformheader" width="20%">
      <em>
        <%= GetText("choice1") %>
      </em>
    </td>
    <td class="post" width="80%">
      <asp:TextBox MaxLength="50" ID="PollChoice1" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr id="PollRow3" runat="server" visible="false">
    <td class="postformheader" width="20%">
      <em>
        <%= GetText("choice2") %>
      </em>
    </td>
    <td class="post" width="80%">
      <asp:TextBox MaxLength="50" ID="PollChoice2" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr id="PollRow4" runat="server" visible="false">
    <td class="postformheader" width="20%">
      <em>
        <%= GetText("choice3") %>
      </em>
    </td>
    <td class="post" width="80%">
      <asp:TextBox MaxLength="50" ID="PollChoice3" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr id="PollRow5" runat="server" visible="false">
    <td class="postformheader" width="20%">
      <em>
        <%= GetText("choice4") %>
      </em>
    </td>
    <td class="post" width="80%">
      <asp:TextBox MaxLength="50" ID="PollChoice4" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr id="PollRow6" runat="server" visible="false">
    <td class="postformheader" width="20%">
      <em>
        <%= GetText("choice5") %>
      </em>
    </td>
    <td class="post" width="80%">
      <asp:TextBox MaxLength="50" ID="PollChoice5" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr id="PollRow7" runat="server" visible="false">
    <td class="postformheader" width="20%">
      <em>
        <%= GetText("choice6") %>
      </em>
    </td>
    <td class="post" width="80%">
      <asp:TextBox MaxLength="50" ID="PollChoice6" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr id="PollRow8" runat="server" visible="false">
    <td class="postformheader" width="20%">
      <em>
        <%= GetText("choice7") %>
      </em>
    </td>
    <td class="post" width="80%">
      <asp:TextBox MaxLength="50" ID="PollChoice7" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr id="PollRow9" runat="server" visible="false">
    <td class="postformheader" width="20%">
      <em>
        <%= GetText("choice8") %>
      </em>
    </td>
    <td class="post" width="80%">
      <asp:TextBox MaxLength="50" ID="PollChoice8" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr id="PollRow10" runat="server" visible="false">
    <td class="postformheader" width="20%">
      <em>
        <%= GetText("choice9") %>
      </em>
    </td>
    <td class="post" width="80%">
      <asp:TextBox MaxLength="50" ID="PollChoice9" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr id="PollRowExpire" runat="server" visible="false">
    <td class="postformheader" width="20%">
      <em>
        <%= GetText("poll_expire") %>
      </em>
    </td>
    <td class="post" width="80%">
      <asp:TextBox MaxLength="10" ID="PollExpire" runat="server" CssClass="edit" Width="400" />
      <%= GetText("poll_expire_explain") %>
    </td>
  </tr>
  <tr>
    <td class="postformheader" width="20%" valign="top">
      <%= GetText("message") %>
      <br />
      <uc1:smileys runat="server" onclick="insertsmiley" />
    </td>
    <td class="post" id="EditorLine" width="80%" runat="server">
      <!-- editor goes here -->
    </td>
  </tr>
  <!--tr>
		<td class=postheader>&nbsp;</td>
		<td class=post>
			<input type=button value=" B " style="font-weight:bold" onclick="makebold()">
			<input type=button value=" I " style="font-weight:bold;font-style:italic" onclick="makeitalic()">
			<input type=button value=" U " style="font-weight:bold;text-decoration:underline" onclick="makeunderline()">
			<input type=button value=" URL " onclick="makeurl()">
			<input type=button value=" QUOTE " onclick="makequote()">
			<input type=button value=" IMG " onclick="makeimg()">
			<input type=button value=" CODE " onclick="makecode()">
		</td>
	</tr-->
  <tr id="EditReasonRow" runat="server">
    <td class="postformheader" width="20%">
      <%= GetText("EditReason") %>
    </td>
    <td class="post" width="80%">
      <asp:TextBox ID="ReasonEditor" runat="server" CssClass="edit" Width="400" /></td>
  </tr>
  <tr>
    <td align="center" colspan="2" class="footer1">
      <asp:Button ID="Preview" CssClass="pbutton" runat="server" OnClick="Preview_Click" />
      <asp:Button ID="PostReply" CssClass="pbutton" runat="server" OnClick="PostReply_Click" />
      <asp:Button ID="Cancel" CssClass="pbutton" runat="server" OnClick="Cancel_Click" />
    </td>
  </tr>
</table>
<br />
<asp:Repeater ID="LastPosts" runat="server" Visible="false">
  <HeaderTemplate>
    <table class="content" cellspacing="1" cellpadding="0" width="100%" align="center">
      <tr>
        <td class="header2" align="center" colspan="2">
          <%# GetText("last10") %>
        </td>
      </tr>
  </HeaderTemplate>
  <FooterTemplate>
    </table>
  </FooterTemplate>
  <ItemTemplate>
    <tr class="postheader">
      <td width="140">
        <b><a href="<%# YAF.Classes.Utils.YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.profile,"u={0}",Eval( "UserID")) %>">
          <%# Eval( "UserName") %>
        </a></b>
      </td>
      <td width="80%" class="small" align="left">
        <b>
          <%# GetText("posted") %>
        </b>
        <%# YafDateTime.FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %>
      </td>
    </tr>
    <tr class="post">
      <td>
        &nbsp;</td>
      <td valign="top" class="message">
        <%# FormatBody(Container.DataItem) %>
      </td>
    </tr>
  </ItemTemplate>
  <AlternatingItemTemplate>
    <tr class="postheader">
      <td width="140">
        <b><a href="<%# YAF.Classes.Utils.YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.profile,"u={0}",Eval( "UserID")) %>">
          <%# Eval( "UserName") %>
        </a></b>
      </td>
      <td width="80%" class="small" align="left">
        <b>
          <%# GetText("posted") %>
        </b>
        <%# YafDateTime.FormatDateTime( ( System.DateTime ) ( ( System.Data.DataRowView ) Container.DataItem ) ["Posted"] )%>
      </td>
    </tr>
    <tr class="post_alt">
      <td>
        &nbsp;</td>
      <td valign="top" class="message">
        <%# FormatBody(Container.DataItem) %>
      </td>
    </tr>
  </AlternatingItemTemplate>
</asp:Repeater>
<iframe runat="server" visible="false" id="LastPostsIFrame" name="lastposts" width="100%"
  height="300" frameborder="0" marginheight="2" marginwidth="2" scrolling="yes"></iframe>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
