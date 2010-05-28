<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.avatar" Codebehind="avatar.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator"></div>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="2">
            <YAF:LocalizedLabel ID="TitleLabel" LocalizedTag="TITLE" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="post">
            <asp:DataList runat="server" ID="directories" Width="100%" RepeatColumns="5" OnItemDataBound="Directories_Bind"
                OnItemCommand="ItemCommand">
                <ItemStyle BorderWidth="1" BorderStyle="Dotted" Width="20%" />
                <ItemTemplate>
                    <asp:LinkButton ID="dirName" runat="server" CommandName="directory" />
                </ItemTemplate>
            </asp:DataList>
        </td>
    </tr>
    <tr>
        <td class="post">
            <asp:DataList runat="server" ID="files" Width="100%" RepeatColumns="5" OnItemDataBound="Files_Bind" OnItemCommand="ItemCommand">
                <ItemStyle BorderWidth="1" BorderStyle="Dotted" CssClass="post" Width="20%" />
                <HeaderTemplate>
                    <asp:LinkButton ID="up" runat="server" CommandName="directory" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Literal ID="fname" runat="server" />
                </ItemTemplate>
            </asp:DataList>
        </td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
