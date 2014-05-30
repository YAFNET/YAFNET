<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.moderate.reportedposts"CodeBehind="reportedposts.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="ReportedPosts" Src="../../controls/ReportedPosts.ascx" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:Repeater ID="List" runat="server">
    <HeaderTemplate>
        <table class="content" width="100%">
            <tr>
                <td colspan="3" class="header1" align="left">
                    <%# PageContext.PageForumName %>
                    -
                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="REPORTED" />
                </td>
            </tr>
        </table>
    </HeaderTemplate>
    <ItemTemplate>
        <table class="content" width="100%">
            <tr class="header2">
                <td colspan="3">
                    <YAF:LocalizedLabel ID="TopicLabel" runat="server" LocalizedTag="TOPIC" />
                    &nbsp;<a id="TopicLink" href='<%# YafBuildLink.GetLink(ForumPages.posts, "t={0}", Eval("TopicID")) %>'
                        runat="server"><%# Eval("Topic") %></a>
                </td>
            </tr>
            <tr class="postheader">
                <td class="postformheader">
                    <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# this.Eval("UserID").ToType<int>() %>' />
                    <YAF:ThemeButton ID="AdminUserButton" runat="server" CssClass="yaflittlebutton" Visible='<%# this.PageContext.IsAdmin %>'
                    TextLocalizedTag="ADMIN_USER" TextLocalizedPage="PROFILE" 
                    NavigateUrl='<%# YafBuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", Convert.ToInt32(Eval("UserID")) ) %>'>
                </YAF:ThemeButton>
                </td>
                <td>
                    <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="POSTED" />
                    </strong>
                    <%# this.Get<IDateTime>().FormatDateShort((DateTime) DataBinder.Eval(Container.DataItem, "[\"Posted\"]")) %>
                    <br />
                    <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NUMBERREPORTED" />
                    </strong>
                    <%# DataBinder.Eval(Container.DataItem, "[\"NumberOfReports\"]") %>
                    <label id="Label1" runat="server" visible='<%# YAF.Utils.General.CompareMessage(DataBinder.Eval(Container.DataItem, "[\"OriginalMessage\"]"),DataBinder.Eval(Container.DataItem, "[\"Message\"]"))%>'>
                        <strong>
                            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MODIFIED" />
                        </strong>
                    </label>
                </td>
                <td>
                    <YAF:ThemeButton ID="ReportAsSpamBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM"
                        TextLocalizedTag="REPORT_SPAM" CommandName="spam" CommandArgument='<%# FormatMessage((System.Data.DataRowView)Container.DataItem) %>'
                        visible='<%# !this.Get<YafBoardSettings>().SpamServiceType.Equals(0)%>' />
                    <YAF:ThemeButton ID="CopyOverBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM"
                        TextLocalizedTag="COPYOVER" CommandName="CopyOver" Visible='<%# YAF.Utils.General.CompareMessage(DataBinder.Eval(Container.DataItem, "[\"OriginalMessage\"]"),DataBinder.Eval(Container.DataItem, "[\"Message\"]"))%>'
                        CommandArgument='<%# Eval("MessageID") %>' />
                    <YAF:ThemeButton ID="DeleteBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM"
                        TextLocalizedTag="DELETE" CommandName="Delete" CommandArgument='<%# Eval("MessageID") %>'
                        OnLoad="Delete_Load" />
                    <YAF:ThemeButton ID="ResolveBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM"
                        TextLocalizedTag="RESOLVED" CommandName="Resolved" CommandArgument='<%# Eval("MessageID") %>' />
                    <YAF:ThemeButton ID="ViewBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM"
                        TextLocalizedTag="VIEW" CommandName="View" CommandArgument='<%# Eval("MessageID") %>' />
                    <YAF:ThemeButton ID="ViewHistoryBtn" runat="server" CssClass="yaflittlebutton" TextLocalizedPage="MODERATE_FORUM"
                        TextLocalizedTag="HISTORY" CommandName="ViewHistory" CommandArgument='<%# PageContext.PageForumID + "," + Eval("MessageID") %>' />
                </td>
            </tr>
            <tr>
                <td valign="top" width="140" class="postformheader">
                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="ORIGINALMESSAGE" />
                    &nbsp;
                </td>
                <td valign="top" class="post message" colspan="2">
                    <%# FormatMessage((System.Data.DataRowView)Container.DataItem)%>
                </td>
            </tr>
            <tr class="postheader" style="vertical-align:top">
                <td class="postformheader">
                    <YAF:LocalizedLabel ID="ReportedByLabel" runat="server" LocalizedTag="REPORTEDBY" />
                </td>
                <td colspan="2">
                    <YAF:ReportedPosts ID="ReportersList" runat="server" MessageID='<%# DataBinder.Eval(Container.DataItem, "[\"MessageID\"]") %>'
                        ResolvedBy='<%# DataBinder.Eval(Container.DataItem, "[\"ResolvedBy\"]") %>' Resolved='<%# DataBinder.Eval(Container.DataItem, "[\"Resolved\"]") %>'
                        ResolvedDate='<%# DataBinder.Eval(Container.DataItem, "[\"ResolvedDate\"]") %>' />
                </td>
            </tr>
        </table>
    </ItemTemplate>
    <SeparatorTemplate>
        <br />
    </SeparatorTemplate>
</asp:Repeater>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
