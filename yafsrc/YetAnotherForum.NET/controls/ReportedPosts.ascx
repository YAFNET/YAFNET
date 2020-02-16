
<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.ReportedPosts" Codebehind="ReportedPosts.ascx.cs" %>


<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<asp:Repeater ID="ReportedPostsRepeater" runat="server" Visible="false">
    <HeaderTemplate>
        <div class="alert alert-secondary" role="alert">
    </HeaderTemplate>
    <ItemTemplate>
        <span class="font-weight-bold">
            <YAF:LocalizedLabel ID="ReportedByLabel" runat="server" 
                                LocalizedTag="REPORTEDBY"></YAF:LocalizedLabel>
        </span>
        <YAF:UserLink ID="UserLink1" runat="server" 
                      UserID='<%# Container.DataItemToField<int>("UserID") %>'>
        </YAF:UserLink>
		<YAF:ThemeButton ID="PM" runat="server" 
                         Size="Small"
                         TextLocalizedTag="PM"
                         NavigateUrl='<%# BuildLink.GetLinkNotEscaped(ForumPages.PostMessage, "u={0}", Container.DataItemToField<int>("UserID"))%>'
                         Type="Secondary" Icon="envelope" />
        
        <span class="font-weight-bold pr-3">
            <asp:Label runat="server" ID="ReportedDateTime"></asp:Label>
        </span>
        <asp:Literal ID="MessageBody" runat="server"></asp:Literal>

    </ItemTemplate>
    <FooterTemplate>
        </div>
    </FooterTemplate>
</asp:Repeater>
