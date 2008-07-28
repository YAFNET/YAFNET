<%@ Control Language="c#" CodeFile="admin.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.admin" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
  <asp:Repeater ID="ActiveList" runat="server">
    <HeaderTemplate>
      <table width="100%" cellspacing="1" cellpadding="0" class="content">
        <tr>
          <td class="header1" colspan="5">
            Who is Online</td>
        </tr>
        <tr>
          <td class="header2">
            Name</td>
          <td class="header2">
            IP Address</td>
          <td class="header2">
            Location</td>
          <td class="header2">
            Forum Location</td>
          <td class="header2">
            Topic Location</td>
        </tr>
    </HeaderTemplate>
    <ItemTemplate>
      <tr>
        <td class="post">
          <%# BBCode.EncodeHTML( Eval("UserName").ToString() ) %>
        </td>
        <td class="post">
          <%# Eval("IP") %>
        </td>
        <td class="post">
          <%# BBCode.EncodeHTML( PageContext.GetProfile( Eval("UserName").ToString() ).Location ) %>
        </td>
        <td class="post">
          <%# FormatForumLink(Eval("ForumID"),Eval("ForumName")) %>
        </td>
        <td class="post">
          <%# FormatTopicLink(Eval("TopicID"),Eval("TopicName")) %>
        </td>
      </tr>
    </ItemTemplate>
    <FooterTemplate>
      </table>
    </FooterTemplate>
  </asp:Repeater>
  <asp:Repeater ID="UserList" runat="server" OnItemCommand="UserList_ItemCommand">
    <HeaderTemplate>
      <br />
      <table width="100%" cellspacing="1" cellpadding="0" class="content">
        <tr>
          <td class="header1" colspan="5">
            Unverified Users</td>
        </tr>
        <tr>
          <td class="header2">
            Name</td>
          <td class="header2">
            Email</td>
          <td class="header2">
            Location</td>
          <td class="header2">
            Joined</td>
          <td class="header2">
            &nbsp;</td>
        </tr>
    </HeaderTemplate>
    <ItemTemplate>
      <tr>
        <td class="post">
          <%# BBCode.EncodeHTML( Eval("Name").ToString() ) %>
        </td>
        <td class="post">
          <%# Eval("Email") %>
        </td>
        <td class="post">
          <%# BBCode.EncodeHTML( PageContext.GetProfile( Eval( "Name" ).ToString() ).Location )%>
        </td>
        <td class="post">
          <%# YafDateTime.FormatDateTime(Eval("Joined")) %>
        </td>
        <td class="post">
          <asp:LinkButton OnLoad="Approve_Load" runat="server" CommandName="approve" CommandArgument='<%# Eval("UserID") %>'
            Text="Approve" />
          |
          <asp:LinkButton OnLoad="Delete_Load" runat="server" CommandName="delete" CommandArgument='<%# Eval("UserID") %>'
            Text="Delete" />
        </td>
      </tr>
    </ItemTemplate>
    <FooterTemplate>
      <tr>
        <td class="footer1" colspan="5">
          <asp:Button OnLoad="ApproveAll_Load" CommandName="approveall" Text="Approve All" runat="server" />
          <asp:Button OnLoad="DeleteAll_Load" CommandName="deleteall" runat="server" Text="Delete All More Than 14 Days Old" /></td>
      </tr>
      </table>
    </FooterTemplate>
  </asp:Repeater>
  <br />
  <table class="content" cellspacing="1" cellpadding="0" width="100%">
    <tr>
      <td class="header1" colspan="4">
        Statistics<span runat="server" id="boardSelector" Visible='<%# this.PageContext.IsHostAdmin %>' > for <asp:DropDownList ID="BoardStatsSelect" runat="server" DataTextField="Name" DataValueField="BoardID" OnSelectedIndexChanged="BoardStatsSelect_Changed" AutoPostBack="true" /></span></td>
    </tr>
    <tr>
      <td class="postheader" width="25%">
        Number of posts:</td>
      <td class="post" width="25%">
        <asp:Label ID="NumPosts" runat="server"></asp:Label></td>
      <td class="postheader" width="25%">
        Posts per day:</td>
      <td class="post" width="25%">
        <asp:Label ID="DayPosts" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td class="postheader">
        Number of topics:</td>
      <td class="post">
        <asp:Label ID="NumTopics" runat="server"></asp:Label></td>
      <td class="postheader">
        Topics per day:</td>
      <td class="post">
        <asp:Label ID="DayTopics" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td class="postheader">
        Number of users:</td>
      <td class="post">
        <asp:Label ID="NumUsers" runat="server"></asp:Label></td>
      <td class="postheader">
        Users per day:</td>
      <td class="post">
        <asp:Label ID="DayUsers" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td class="postheader">
        Board started:</td>
      <td class="post">
        <asp:Label ID="BoardStart" runat="server"></asp:Label></td>
      <td class="postheader">
        Size of database:</td>
      <td class="post">
        <asp:Label ID="DBSize" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td class="postfooter" colspan="4">
        These statistics don't count deleted topics and posts.</td>
    </tr>
  </table>
  <p id="UpgradeNotice" runat="server" visible="false">
    The installed version of Yet Another Forum.net and the version of your database
    does not match. You should go to <a href="install/" target='_"top"'>install</a>
    and update your database.
  </p>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
