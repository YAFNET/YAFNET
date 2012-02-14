﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.moderate.unapprovedposts"CodeBehind="unapprovedposts.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
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
                    runat="server"><%# Eval("Topic") %></a>
            </td>
        </tr>
        <tr class="postheader">
            <td>
                <YAF:UserLink ID="UserName" runat="server" UserID='<%# Convert.ToInt32(Eval("UserID")) %>' />
            </td>
            <td>
                <b>Posted:</b>
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
                <a onclick="ScrollTop();" href="javascript: void(0)">            
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
