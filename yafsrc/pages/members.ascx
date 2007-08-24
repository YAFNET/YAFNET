<%@ Control Language="c#" Codebehind="members.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.members" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>
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
      <img runat="server" id="SortUserName" align="absmiddle" />
      <asp:LinkButton runat="server" ID="UserName" /></td>
    <td class="header2">
      <img runat="server" id="SortRank" align="absmiddle" />
      <asp:LinkButton runat="server" ID="Rank" /></td>
    <td class="header2">
      <img runat="server" id="SortJoined" align="absmiddle" />
      <asp:LinkButton runat="server" ID="Joined" /></td>
    <td class="header2" align="center">
      <img runat="server" id="SortPosts" align="absmiddle" />
      <asp:LinkButton runat="server" ID="Posts" /></td>
    <td class="header2">
      <img runat="server" id="SortLocation" align="absmiddle" />
      <asp:LinkButton runat="server" ID="Location" /></td>
  </tr>
  <asp:Repeater ID="MemberList" runat="server">
    <ItemTemplate>
      <tr>
        <td class="post">
          <a href='<%# YAF.Classes.Utils.yaf_BuildLink.GetLink(YAF.Classes.Utils.ForumPages.profile,"u={0}",Eval("UserID")) %>'>
            <%# Server.HtmlEncode(Convert.ToString(Eval("Name"))) %>
          </a>
        </td>
        <td class="post">
          <%# Eval("RankName") %>
        </td>
        <td class="post">
          <%# yaf_DateTime.FormatDateLong((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Joined"]) %>
        </td>
        <td class="post" align="center">
          <%# String.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)["NumPosts"]) %>
        </td>
        <td class="post">
          <%# GetStringSafely(DataBinder.Eval(Container.DataItem,"Location")) %>
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
