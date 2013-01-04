<%@ Control Language="C#" Inherits="YAF.DotNetNuke.YafDnnWhatsNew" AutoEventWireup="true" CodeBehind="YafDnnWhatsNew.ascx.cs" %>
<asp:Repeater runat="server" ID="LatestPosts" OnItemDataBound="LatestPostsItemDataBound" >
 <HeaderTemplate />
 <ItemTemplate />
 <SeparatorTemplate />
 <FooterTemplate />
</asp:Repeater>
<asp:Label id="lInfo" runat="server"></asp:Label>