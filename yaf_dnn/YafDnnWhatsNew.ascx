<%@ Control Language="C#" Inherits="YAF.DotNetNuke.YafDnnWhatsNew" AutoEventWireup="true" CodeBehind="YafDnnWhatsNew.ascx.cs" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" %>

<asp:Repeater runat="server" ID="LatestPosts" OnItemDataBound="LatestPostsItemDataBound">
  <headerTemplate>
    <ul>
  </HeaderTemplate>
  <ItemTemplate>
    <li class="YafPosts">
      <asp:HyperLink ID="ImageMessageLink" runat="server">
        <YAF:ThemeImage ID="LastPostedImage" runat="server" LocalizedTitlePage="DEFAULT" LocalizedTitleTag="GO_LAST_POST" Style="border: 0" />
      </asp:HyperLink>
      <strong><asp:HyperLink ID="TextMessageLink" runat="server" /></strong> (<asp:HyperLink ID="ForumLink" runat="server" />)<br />
      <YAF:LocalizedLabel ID="ByLabel" runat="server" LocalizedTag="BY" />
      <asp:HyperLink id="LastUserLink" runat="server" /> <em><asp:Label ID="LastPostedDateLabel" runat="server" /></em>
    </li>
  </ItemTemplate>
  <footerTemplate>
    </ul>
  </FooterTemplate>
</asp:Repeater>
<asp:Label id="lInfo" runat="server"></asp:Label>