<%@ Control Language="c#" Codebehind="groups.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.groups" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
  <table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
      <td class="header1" colspan="6">
        Roles</td>
    </tr>
    <asp:Repeater ID="GroupList" runat="server">
      <HeaderTemplate>
        <tr>
          <td class="header2">
            Name</td>
          <td class="header2">
            Is Start</td>
          <td class="header2">
            Is Admin</td>
          <td class="header2">
            Is Moderator</td>
          <td class="header2">
            Is Anonymous</td>
          <td class="header2">
            &nbsp;</td>
        </tr>
      </HeaderTemplate>
      <ItemTemplate>
        <tr>
          <td class="post">
            <%# Eval( "Name") %>
          </td>
          <td class="post">
            <%# BitSet(Eval( "Flags"),4) %>
          </td>
          <td class="post">
            <%# BitSet(Eval( "Flags"),1) %>
          </td>
          <td class="post">
            <%# BitSet(Eval( "Flags"),8) %>
          </td>
         <td class="post">
            <%# BitSet(Eval( "Flags"),16) %>
          </td>          
          <td class="post">
            <asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval( "GroupID") %>'>Edit</asp:LinkButton>
            |
            <asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval( "GroupID") %>'>Delete</asp:LinkButton>
          </td>
        </tr>
      </ItemTemplate>
    </asp:Repeater>
    <tr>
      <td class="footer1" colspan="7">
        <asp:LinkButton ID="NewGroup" runat="server" OnClick="NewGroup_Click">New Role</asp:LinkButton></td>
    </tr>
  </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
