<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.ReportPost"CodeBehind="reportpost.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="2">
            <YAF:LocalizedLabel ID="ReportPostLabel" runat="server" LocalizedTag="HEADER" />
        </td>
    </tr>
    <asp:Repeater ID="MessageList" runat="server">
        <ItemTemplate>
            <tr class="postheader">
                <td width="140px" id="NameCell" valign="top">
                    <YAF:LocalizedLabel ID="PostedByLabel" runat="server" LocalizedTag="POSTEDBY" />
                    <a name="<%# DataBinder.Eval(Container.DataItem, "MessageID") %>" /><strong>
                        <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# DataBinder.Eval(Container.DataItem, "UserID") %>' />
                    </strong>
                    <YAF:OnlineStatusImage ID="OnlineStatusImage" runat="server" Visible='<%# PageContext.BoardSettings.ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( DataBinder.Eval(Container.DataItem, "UserID") )%>'
                        Style="vertical-align: bottom" UserID='<%# DataBinder.Eval(Container.DataItem, "UserID") %>' />
                </td>
                <td width="80%" class="postheader">
                    <strong>
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="POSTED" />
                    </strong>
                    <%# this.Get<IDateTime>().FormatDateTime( Container.DataItemToField<DateTime>("Posted") )%>
                </td>
            </tr>
            <tr>
                <td class="post" colspan="2">
                    <YAF:MessagePostData ID="MessagePreview" runat="server" ShowAttachments="false" ShowSignature="false"
                        DataRow="<%# ((System.Data.DataRowView)Container.DataItem).Row %>">
                    </YAF:MessagePostData>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr>
        <td class="postformheader" style="width: 100px" valign="top">
            <YAF:LocalizedLabel ID="EnterReportTextLabel" runat="server" LocalizedTag="ENTER_TEXT" />
        </td>
        <td id="EditorLine" class="post" runat="server">
            <asp:Label ID="IncorrectReportLabel" runat="server"></asp:Label>
            <!-- editor goes here -->
        </td>
    </tr>
    <tr class="footer1">
        <td></td>
        <td>
             <YAF:ThemeButton ID="btnReport" runat="server" CssClass="yafcssbigbutton leftItem"
                TextLocalizedTag="SEND" TitleLocalizedTag="SEND_TITLE" OnClick="BtnReport_Click" />
             <YAF:ThemeButton ID="btnCancel" runat="server" CssClass="yafcssbigbutton leftItem"
                TextLocalizedTag="CANCEL" TitleLocalizedTag="CANCEL_TITLE" OnClick="BtnCancel_Click" />
        </td>
    </tr>
</table>
