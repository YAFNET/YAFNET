<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.deletemessage" Codebehind="deletemessage.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Register TagPrefix="uc1" TagName="smileys" Src="../controls/smileys.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" cellspacing="1" cellpadding="4" width="100%" align="center">
    <tr>
        <td class="header1" align="center" colspan="2">
            <asp:Label ID="Title" runat="server" />
        </td>
    </tr>
    <tr id="SubjectRow" runat="server">
        <td class="postformheader" width="20%">
            <YAF:LocalizedLabel runat="server" LocalizedTag="subject" />
        </td>
        <td class="post" width="80%">
            <asp:Label runat="server" ID="Subject" />
        </td>
    </tr>
    <tr id="PreviewRow" runat="server" visible="false">
        <td class="postformheader" valign="top">
            <YAF:LocalizedLabel runat="server" LocalizedTag="previewtitle" />
        </td>
        <td class="post" valign="top">
            <YAF:MessagePost ID="MessagePreview" runat="server">
            </YAF:MessagePost>
        </td>
    </tr>
    <tr id="DeleteReasonRow" runat="server">
        <td class="postformheader" width="20%">
            <% = GetReasonText() %>
        </td>
        <td class="post" width="80%">
            <asp:TextBox ID="ReasonEditor" runat="server" CssClass="edit" MaxLength="100" />
        </td>
    </tr>
    <tr id="EraseRow" runat="server" visible="false">
        <td class="postformheader" width="20%">
        </td>
        <td class="post" width="80%">
            <asp:CheckBox ID="EraseMessage" runat="server" Checked="false" /><YAF:LocalizedLabel
                runat="server" LocalizedTag="erasemessage" />
        </td>
    </tr>
    <tr>
        <td align="center" colspan="2" class="footer1">
            <asp:Button ID="Delete" CssClass="pbutton" runat="server" OnClick="ToogleDeleteStatus_Click" />
            <asp:Button ID="Cancel" CssClass="pbutton" runat="server" OnClick="Cancel_Click" />
            <br>
        </td>
    </tr>
</table>
<br />
<asp:Repeater ID="LinkedPosts" runat="server" Visible="false">
    <HeaderTemplate>
        <table class="content linkedPostsContent" cellspacing="1" cellpadding="0" width="100%" align="center">
            <tr>
                <td class="header2" align="center" colspan="1">
                    <asp:CheckBox ID="DeleteAllPosts" OnCheckedChanged="DeleteAllPosts_CheckedChanged1"
                        AutoPostBack="True" runat="server" />
                    Delete All Posts?
                </td>
                <td class="header2" align="center" colspan="1">
                    <%# GetActionText() %>
                </td>
            </tr>
    </HeaderTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr class="postheader">
            <td width="140">
                <strong><a href="<%# YafBuildLink.GetLink(ForumPages.profile,"u={0}",DataBinder.Eval(Container.DataItem, "UserID")) %>">
                    <%# DataBinder.Eval(Container.DataItem, "UserName") %></a></strong>
            </td>
            <td width="80%" class="small" align="left">
                <strong>
                    <YAF:LocalizedLabel runat="server" LocalizedTag="posted" />
                </strong>
                <%# this.Get<IDateTime>().FormatDateTime( ( DateTime ) ( ( System.Data.DataRowView ) Container.DataItem ) ["Posted"] )%>
            </td>
        </tr>
        <tr class="post">
            <td>
                &nbsp;
            </td>
            <td valign="top" class="message">
                <YAF:MessagePostData ID="MessagePost1" runat="server" DataRow="<%# ((System.Data.DataRowView )Container.DataItem).Row %>"
                    ShowAttachments="false" ShowSignature="false">
                </YAF:MessagePostData>
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="postheader">
            <td width="140">
                <strong><a href="<%# YafBuildLink.GetLink(ForumPages.profile,"u={0}",DataBinder.Eval(Container.DataItem, "UserID")) %>">
                    <%# DataBinder.Eval(Container.DataItem, "UserName") %></a></strong>
            </td>
            <td width="80%" class="small" align="left">
                <strong>
                    <YAF:LocalizedLabel runat="server" LocalizedTag="posted" />
                </strong>
                <%# this.Get<IDateTime>().FormatDateTime((DateTime) ((System.Data.DataRowView ) Container.DataItem ) ["Posted"] )%>
            </td>
        </tr>
        <tr class="post_alt">
            <td>
                &nbsp;
            </td>
            <td valign="top" class="message">
                <YAF:MessagePostData ID="MessagePostAlt" runat="server" DataRow="<%#((System.Data.DataRowView )Container.DataItem).Row %>"
                    ShowAttachments="false" ShowSignature="false">
                </YAF:MessagePostData>
            </td>
        </tr>
    </AlternatingItemTemplate>
</asp:Repeater>
<!--
<iframe runat="server" Visible="false" id="LastPostsIFrame" name="lastposts" width="100%" height="300" frameborder="0" marginheight="2" marginwidth="2" scrolling="yes"></iframe>
-->
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
