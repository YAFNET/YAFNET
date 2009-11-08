<%@ Control Language="c#" CodeFile="editcategory.ascx.cs" AutoEventWireup="True"
    Inherits="YAF.Pages.Admin.editcategory" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                Edit Category:
                <asp:Label ID="CategoryNameTitle" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td class="postheader">
                <b>Category Name:</b><br />
                Name of this category.</td>
            <td class="post">
                <asp:TextBox ID="Name" runat="server" MaxLength="50" CssClass="edit"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="postheader">
                <b>Category Image:</b><br />
                This image will be shown next to this category.</td>
            <td class="post">
                <asp:DropDownList ID="CategoryImages" runat="server" />
                <img align="middle" runat="server" id="Preview" />
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <b>Sort Order:</b><br />
                Order the display of this category. Number, lower first.</td>
            <td class="post">
                <asp:TextBox ID="SortOrder" runat="server" MaxLength="5"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="postfooter" colspan="2" align="center">
                <asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click"></asp:Button>&nbsp;
                <asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click"></asp:Button></td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
