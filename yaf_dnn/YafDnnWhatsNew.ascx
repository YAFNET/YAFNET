<%@ Control Language="C#" Inherits="YAF.DotNetNuke.YafDnnWhatsNew" AutoEventWireup="true" CodeBehind="YafDnnWhatsNew.ascx.cs" %>
<%@ Import Namespace="YAF.Utils" %>
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
      <YAF:LocalizedLabel ID="ByLabel" runat="server" LocalizedTag="BY" LocalizedPage="DEFAULT" />
      <asp:HyperLink id="LastUserLink" runat="server" /> <YAF:DisplayDateTime id="DisplayDateTime" runat="server" DateTime='<%# Container.DataItemToField<DateTime>("LastPosted") %>'></YAF:DisplayDateTime>
    </li>
  </ItemTemplate>
  <footerTemplate>
    </ul>
  </FooterTemplate>
</asp:Repeater>
<asp:Label id="lInfo" runat="server"></asp:Label>