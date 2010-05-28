<%@ Import Namespace="System.Data" %>
<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.ReportedPosts" Codebehind="ReportedPosts.ascx.cs" %>
<asp:Repeater ID="ReportedPostsRepeater" runat="server">
    <HeaderTemplate>
        <table width="100%" class="content">
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td class="header2" colspan="2">
                <span class="YafReported_Complainer">
                    <YAF:LocalizedLabel ID="ReportedByLabel" runat="server" LocalizedTag="REPORTEDBY">
                    </YAF:LocalizedLabel>
                    <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# Container.DataItemToField<int>("UserID") %>'>
                    </YAF:UserLink>
		            <YAF:ThemeButton ID="PM" runat="server" CssClass="yaflittlebutton"            
				    TextLocalizedTag="PM"  ImageThemeTag="PM" 
				    NavigateUrl='<%# YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", Container.DataItemToField<int>("UserID"))%>' />
                </span>
            </td>
        </tr>
        <tr>
            <td>
                <span class="YafReported_DateTime">
                    <asp:Label runat="server" ID="ReportedDateTime"></asp:Label></span>
            </td>
            <td class="post">
                <asp:Literal ID="MessageBody" runat="server"></asp:Literal>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
