<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.moderate.unapprovedposts"CodeBehind="unapprovedposts.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:Repeater ID="List" runat="server">
    <HeaderTemplate>
        <table class="content" cellspacing="1" cellpadding="0" width="100%">
            <tr>
                <td colspan="2" class="header1" align="left">
                    <YAF:LocalizedLabel runat="server" LocalizedTag="UNAPPROVED" />
                </td>
            </tr>
    </HeaderTemplate>
    <FooterTemplate>
        <tr>
            <td class="postfooter" colspan="2">
                &nbsp;
            </td>
        </tr>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr class="header2">
            <td colspan="2">
                <YAF:LocalizedLabel ID="TopicLabel" runat="server" LocalizedTag="TOPIC" />
                &nbsp;<a id="TopicLink" href='<%# YafBuildLink.GetLink(ForumPages.posts, "t={0}", Eval("TopicID")) %>'
                    runat="server" Visible='<%# Eval("MessageCount").ToType<int>() > 0 %>'><%# Eval("Topic") %></a>
                <asp:Label id="TopicName" runat="server" Visible='<%# Eval("MessageCount").ToType<int>() == 0 %>' Text='<%# Eval("Topic") %>'></asp:Label>
            </td>
        </tr>
        <tr class="postheader">
            <td>
                <YAF:UserLink ID="UserName" runat="server" UserID='<%# Convert.ToInt32(Eval("UserID")) %>' />
                <YAF:ThemeButton ID="AdminUserButton" runat="server" CssClass="yaflittlebutton" Visible='<%# this.PageContext.IsAdmin %>'
                    TextLocalizedTag="ADMIN_USER" TextLocalizedPage="PROFILE" 
                    NavigateUrl='<%# YafBuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", Convert.ToInt32(Eval("UserID")) ) %>'>
                </YAF:ThemeButton>
            </td>
            <td>
                <strong>Posted:</strong>
                <%# this.Get<IDateTime>().FormatDateTimeShort( Convert.ToDateTime( Eval( "Posted" ) ) )%>
            </td>
        </tr>
        <tr class="post">
            <td valign="top" width="140">
                &nbsp;
            </td>
            <td valign="top" class="message">
                <%# FormatMessage((System.Data.DataRowView)Container.DataItem)%>
            </td>
        </tr>
        <tr class="postfooter">
            <td class="small postTop">
                <a onclick="ScrollToTop();" class="postTopLink" href="javascript: void(0)">            
                  <YAF:ThemeImage ID="ThemeImage1" LocalizedTitlePage="POSTS" LocalizedTitleTag="TOP"  runat="server" ThemeTag="TOTOPPOST" />
                </a>
            </td>
            <td class="postfooter" style="float: left">
                <YAF:ThemeButton ID="ApproveBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM"
                    TextLocalizedTag="APPROVE" CommandName="Approve" CommandArgument='<%# Eval("MessageID") %>' />
                <YAF:ThemeButton ID="DeleteBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM"
                    TextLocalizedTag="DELETE" CommandName="Delete" CommandArgument='<%# Eval("MessageID") %>'
                    OnLoad="Delete_Load" />
            </td>
        </tr>
    </ItemTemplate>
    <SeparatorTemplate>
        <tr class="postsep">
            <td colspan="2" style="height: 7px">
            </td>
        </tr>
    </SeparatorTemplate>
</asp:Repeater>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
