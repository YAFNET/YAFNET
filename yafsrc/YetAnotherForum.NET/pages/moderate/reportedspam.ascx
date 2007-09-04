<%@ Control Language="C#" AutoEventWireup="true" CodeFile="reportedspam.ascx.cs"
    Inherits="YAF.Pages.moderate.reportedspam" %>




<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="True" />
<yaf:pagelinks runat="server" id="PageLinks" />
<asp:Repeater ID="List" runat="server">
    <HeaderTemplate>
        <table class="content" cellspacing="1" cellpadding="0" width="100%">
            <tr>
                <td colspan="2" class="header1" align="left">
                    <%# PageContext.PageForumName %>
                    -
                    <%# GetText("MODERATE_FORUM","REPORTED") %>
                </td>
            </tr>
    </HeaderTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr class="header2">
            <td colspan="2">
                <%# Eval("Topic") %>
            </td>
        </tr>
        <tr class="postheader">
            <td>
                <asp:HyperLink ID="UserName" runat="server" href='<%# YafBuildLink.GetLink(ForumPages.profile,"u={0}",DataBinder.Eval(Container.DataItem, "[\"UserID\"]")) %>'> <%# Eval("UserName") %> </asp:HyperLink></b>
            </td>
            <td>
                <b>
                    <%# PageContext.Localization.GetText("POSTED") %>
                </b>
                <%# YafDateTime.FormatDateTime((System.DateTime) DataBinder.Eval(Container.DataItem, "[\"Posted\"]")) %>
                <b>
                    <%# PageContext.Localization.GetText("NUMBERREPORTED")%>
                </b>
                <%# DataBinder.Eval(Container.DataItem, "[\"NumberOfReports\"]") %>
                <label id="Label1" runat="server" visible='<%# CompareMessage(DataBinder.Eval(Container.DataItem, "[\"OriginalMessage\"]"),DataBinder.Eval(Container.DataItem, "[\"Message\"]"))%>'>
                <b>
                    <%# PageContext.Localization.GetText("MODIFIED") %>
                </b>
                </label>
            </td>
        </tr>
        <tr class="post">
            <td valign="top" width="140">
                &nbsp;</td>
            <td valign="top" class="message">
                <%# FormatMessage((System.Data.DataRowView)Container.DataItem) %>
            </td>
        </tr>
        <tr class="postfooter">
            <td class="small">
                <a href="javascript:scroll(0,0)">
                    <%# GetText("MODERATE_FORUM","TOP") %>
                </a>
            </td>
            <td class="postfooter">
                <asp:LinkButton ID="ViewBtn" runat="server" Text='<%# GetText("MODERATE_FORUM","VIEW") %>'
                    CommandName="View" CommandArgument='<%# Eval("MessageID") %>' />&nbsp;
                <asp:LinkButton ID="CopyOverBtn" runat="server" Text='<%# GetText("MODERATE_FORUM","COPYOVER") %>'
                    CommandName="CopyOver" Visible='<%# CompareMessage(DataBinder.Eval(Container.DataItem, "[\"OriginalMessage\"]"),DataBinder.Eval(Container.DataItem, "[\"Message\"]"))%>' CommandArgument='<%# Eval("MessageID") %>' />&nbsp;
                <asp:LinkButton ID="ResolveBtn" runat="server" Text='<%# GetText("MODERATE_FORUM","RESOLVED") %>'
                    CommandName="Resolved" CommandArgument='<%# Eval("MessageID") %>' />&nbsp;
                <asp:LinkButton ID="DeleteBtn" runat="server" Text='<%# GetText("MODERATE_FORUM","DELETE") %>'
                    CommandName="Delete" CommandArgument='<%# Eval("MessageID") %>' OnLoad="Delete_Load" />&nbsp;
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
<yaf:smartscroller id="SmartScroller1" runat="server" />
