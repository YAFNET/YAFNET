<%@ Control Language="C#" Inherits="YAF.DotNetNuke.YafDnnWhatsNew" AutoEventWireup="true" CodeBehind="YafDnnWhatsNew.ascx.cs" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Register TagPrefix="YAF" assembly="YAF.Controls" Namespace="YAF.Controls" %>
<asp:Repeater runat="server" ID="LatestPosts" OnItemDataBound="LatestPostsItemDataBound">
 <HeaderTemplate />
 <ItemTemplate />
 <SeparatorTemplate />
 <FooterTemplate />
</asp:Repeater>
<asp:Label id="lInfo" runat="server"></asp:Label>