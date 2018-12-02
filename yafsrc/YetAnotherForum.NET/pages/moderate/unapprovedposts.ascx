<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.moderate.unapprovedposts" CodeBehind="unapprovedposts.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel runat="server" LocalizedTag="UNAPPROVED" /></h1>
    </div>
</div>
<asp:Repeater ID="List" runat="server">
    <ItemTemplate>
        <div class="row">
            <div class="col-xl-12">
                <div class="card mb-3">
        <div class="card-header">
            <i class="fa fa-file-alt fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="TopicLabel" runat="server" LocalizedTag="TOPIC" />
            &nbsp;<a id="TopicLink" href='<%# YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.Eval("TopicID")) %>'
                     runat="server" Visible='<%# this.Eval("MessageCount").ToType<int>() > 0 %>'><%# this.Eval("Topic") %></a>
            <asp:Label id="TopicName" runat="server" Visible='<%# this.Eval("MessageCount").ToType<int>() == 0 %>' Text='<%# this.Eval("Topic") %>'></asp:Label>
        </div>
        <div class="card-body text-center">
            <p class="card-text">
                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="INFO" LocalizedPage="ADMIN_RESTARTAPP" />
            </p>
        </div>
        <tr class="header2">
            <td colspan="2">
                
            </td>
        </tr>
        <tr class="postheader">
            <td>
                <YAF:UserLink ID="UserName" runat="server" UserID='<%# Convert.ToInt32(this.Eval("UserID")) %>' />
                <YAF:ThemeButton ID="AdminUserButton" runat="server" CssClass="yaflittlebutton" Visible='<%# this.PageContext.IsAdmin %>'
                    TextLocalizedTag="ADMIN_USER" TextLocalizedPage="PROFILE" 
                    NavigateUrl='<%# YafBuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", Convert.ToInt32(this.Eval("UserID")) ) %>'>
                </YAF:ThemeButton>
            </td>
            <td>
                <strong>Posted:</strong>
                <%# this.Get<IDateTime>().FormatDateTimeShort( Convert.ToDateTime( this.Eval( "Posted" ) ) )%>
            </td>
        </tr>
        <tr class="post">
            <td valign="top" width="140">
                &nbsp;
            </td>
            <td valign="top" class="message">
                <%#
    this.FormatMessage((System.Data.DataRowView)Container.DataItem)%>
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
                    TextLocalizedTag="APPROVE" CommandName="Approve" CommandArgument='<%# this.Eval("MessageID") %>' />
                <YAF:ThemeButton ID="DeleteBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM"
                    TextLocalizedTag="DELETE" CommandName="Delete" CommandArgument='<%# this.Eval("MessageID") %>'
                                 ReturnConfirmText='<%# this.GetText("ASK_DELETE") %>'
                                 Icon="trash" Type="Danger" />
            </td>
        </tr>
        <div class="card-footer text-center">
                
        </div>
        </div>
        </div>
        </div>
    </ItemTemplate>
</asp:Repeater>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
