<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="true" Inherits="YAF.Controls.ProfileYourAccount"
    CodeBehind="ProfileYourAccount.ascx.cs" %>
<table align="center" cellspacing="0" cellpadding="0" class="content" id="yafprofilecontent">
    <tr>
        <td colspan="3" class="header2">
            <YAF:LocalizedLabel ID="YourAccountLocalized" runat="server" LocalizedTag="YOUR_ACCOUNT" />
        </td>
    </tr>
    <tr>
        <td width="33%" class="postheader">
            <YAF:LocalizedLabel ID="YourUsernameLocalized" runat="server" LocalizedTag="YOUR_USERNAME" />
        </td>
        <td class="post">
            <asp:Label ID="Name" runat="server" />
        </td>
        <td rowspan="6" align="center" class="post">
            <asp:Image runat="server" ID="AvatarImage" CssClass="avatarimage" AlternateText="avatar" />
        </td>
    </tr>
    <asp:PlaceHolder ID="DisplayNameHolder" runat="server">
        <tr>
            <td width="33%" class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="YOUR_USERDISPLAYNAME" />
            </td>
            <td class="post">
                <asp:Label ID="DisplayName" runat="server" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="YourEmailLocalized" runat="server" LocalizedTag="YOUR_EMAIL" />
        </td>
        <td class="post">
            <asp:Label ID="AccountEmail" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="NumPostsLocalized" runat="server" LocalizedTag="NUMPOSTS" />
        </td>
        <td class="post">
            <asp:Label ID="NumPosts" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="GroupsLocalized" runat="server" LocalizedTag="GROUPS" />
        </td>
        <td class="post">
            <asp:Repeater ID="Groups" runat="server">
                <ItemTemplate>
                    <span runat="server" style='<%# DataBinder.Eval(Container.DataItem,"Style") %>'>
                        <%# DataBinder.Eval(Container.DataItem,"Name") %></span>
                </ItemTemplate>
                <SeparatorTemplate>
                    ,
                </SeparatorTemplate>
            </asp:Repeater>
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="JoinedLocalized" runat="server" LocalizedTag="JOINED" />
        </td>
        <td class="post">
            <asp:Label ID="Joined" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="footer1" colspan="3">
            &nbsp;
        </td>
    </tr>
</table>
