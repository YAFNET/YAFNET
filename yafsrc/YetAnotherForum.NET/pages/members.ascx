<%@ Control Language="c#" CodeFile="members.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.members" %>




<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
  <tr runat="server" id="LetterRow" />
  <tr runat="server" id="LetterRowRus" />
</table>
<table class="command">
  <tr>
    <td class="navlinks">
      <YAF:Pager runat="server" ID="Pager" />
    </td>
  </tr>
</table>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
  <tr>
    <td class="header1" colspan="5">
      <%= GetText("title") %>
    </td>
  </tr>
  <tr>
    <td class="header2">
      <img runat="server" id="SortUserName" alt="" style="vertical-align:middle" />
      <asp:LinkButton runat="server" ID="UserName" /></td>
    <td class="header2">
      <img runat="server" id="SortRank" alt="" style="vertical-align:middle" />
      <asp:LinkButton runat="server" ID="Rank" /></td>
    <td class="header2">
      <img runat="server" id="SortJoined" alt="" style="vertical-align:middle" />
      <asp:LinkButton runat="server" ID="Joined" /></td>
    <td class="header2" align="center">
      <img runat="server" id="SortPosts" alt="" style="vertical-align:middle" />
      <asp:LinkButton runat="server" ID="Posts" /></td>
    <td class="header2">
      <asp:Label runat="server" id="Location" /></td>
  </tr>
  <asp:Repeater ID="MemberList" runat="server">
    <ItemTemplate>
      <tr>
        <td class="post">
					<YAF:UserLink id="UserProfileLink" runat="server" UserID='<%# Convert.ToInt32(Eval("UserID")) %>' UserName='<%# Eval("Name") %>' />
        </td>
        <td class="post">
          <%# Eval("RankName") %>
        </td>
        <td class="post">
          <%# YafDateTime.FormatDateLong((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Joined"]) %>
        </td>
        <td class="post" align="center">
          <%# String.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)["NumPosts"]) %>
        </td>
        <td class="post">
          <%# GetStringSafely(PageContext.GetProfile(DataBinder.Eval(Container.DataItem,"Name").ToString()).Location) %>
        </td>
      </tr>
    </ItemTemplate>
  </asp:Repeater>
</table>
<table class="command">
  <tr>
    <td class="navlinks">
      <YAF:Pager runat="server" LinkedPager="Pager" />
    </td>
  </tr>
</table>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
