<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.ReportPost" Codebehind="reportpost.ascx.cs" %>
<%@ Import Namespace="YAF.Classes.Core" %>
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
                    <a name="<%# DataBinder.Eval(Container.DataItem, "MessageID") %>" /><b>
                        <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# DataBinder.Eval(Container.DataItem, "UserID") %>' />
                    </b>
                    <YAF:OnlineStatusImage ID="OnlineStatusImage" runat="server" Visible='<%# PageContext.BoardSettings.ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( DataBinder.Eval(Container.DataItem, "UserID") )%>'
                        Style="vertical-align: bottom" UserID='<%# DataBinder.Eval(Container.DataItem, "UserID") %>' />
                </td>
                <td width="80%" class="postheader">
                    <b>
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="POSTED" />
                    </b>
                    <%# YafServices.DateTime.FormatDateTime( Container.DataItemToField<DateTime>("Posted") )%>
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
            <!-- editor goes here -->
        </td>
    </tr>
    <tr class="footer1">
        <td colspan="2" align="center">
            <YAF:ThemeButton ID="btnCancel" runat="server" CssClass="yafcssbigbutton rightItem"
                TextLocalizedTag="CANCEL" OnClick="BtnCancel_Click" />
            <YAF:ThemeButton ID="btnReport" runat="server" CssClass="yafcssbigbutton rightItem"
                TextLocalizedTag="SEND" OnClick="BtnReport_Click" />                
        </td>
    </tr>
</table>
