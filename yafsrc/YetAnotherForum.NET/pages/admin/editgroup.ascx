<%@ Control Language="c#" CodeFile="editgroup.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editgroup" %>




<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
  <table class="content" cellspacing="1" cellpadding="0" width="100%">
    <tr>
      <td class="header1" colspan="11">
        Edit Group</td>
    </tr>
    <tr>
      <td class="postheader" width="50%">
        <b>Name:</b><br />
        Name of this group.</td>
      <td class="post" width="50%">
        <asp:TextBox Style="width: 300px" ID="Name" runat="server" /></td>
    </tr>
    <tr>
      <td class="postheader">
        <b>Is Start:</b><br />
        If this is checked, all new users will be a member of this group.</td>
      <td class="post">
        <asp:CheckBox ID="IsStartX" runat="server"></asp:CheckBox></td>
    </tr>    
    <tr>
      <td class="postheader">
        <b>Is Forum Moderator:</b><br />
        When this is checked, members of this group will have some admin access rights.</td>
      <td class="post">
        <asp:CheckBox ID="IsModeratorX" runat="server"></asp:CheckBox></td>
    </tr>
    <tr>
      <td class="postheader">
        <b>Is Admin:</b><br />
        Means that users in this group are admins.</td>
      <td class="post">
        <asp:CheckBox ID="IsAdminX" runat="server"></asp:CheckBox></td>
    </tr>    
    <tr runat="server" visible="false" id="IsGuestTR">
      <td class="postheader">
        <b>Is Guest:</b><br />
        This flag is internal and makes the role unavailable to .NET membership. Never assign this role to any users except the (1) guest user.
        If you do flag this role as IsGuest, the guest user must a member of it.
        Never use this flag in conjunction with any other flags.</td>
      <td class="post">
        <asp:CheckBox ID="IsGuestX" runat="server"></asp:CheckBox></td>
    </tr>       
    <tr runat="server" id="NewGroupRow">
      <td class="postheader">
        <b>Initial Access Mask:</b><br />
        The initial access mask for all forums.</td>
      <td class="post">
        <asp:DropDownList runat="server" ID="AccessMaskID" OnDataBinding="BindData_AccessMaskID" /></td>
    </tr>
    <asp:Repeater ID="AccessList" runat="server">
      <HeaderTemplate>
        <tr>
          <td class="header1" colspan="11">
            Access</td>
        </tr>
        <tr>
          <td class="header2">
            Forum</td>
          <td class="header2">
            Access Mask</td>
        </tr>
      </HeaderTemplate>
      <ItemTemplate>
        <tr>
          <td class="postheader">
            <asp:Label ID="ForumID" Visible="false" runat="server" Text='<%# Eval( "ForumID") %>'></asp:Label>
            <b>
              <%# Eval( "ForumName") %>
            </b>
            <br>
            Category:
            <%# Eval( "CategoryName") %>
          </td>
          <td class="post">
            <asp:DropDownList runat="server" ID="AccessMaskID" OnDataBinding="BindData_AccessMaskID"
              OnPreRender="SetDropDownIndex" value='<%# Eval("AccessMaskID") %>' />
            ...
          </td>
        </tr>
      </ItemTemplate>
    </asp:Repeater>
    <tr>
      <td class="postfooter" align="middle" colspan="11">
        <asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click"></asp:Button>&nbsp;
        <asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click"></asp:Button></td>
    </tr>
  </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
