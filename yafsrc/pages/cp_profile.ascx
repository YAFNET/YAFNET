<%@ Control Language="c#" Codebehind="cp_profile.ascx.cs" AutoEventWireup="True"
  Inherits="YAF.Pages.cp_profile" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table width="100%" cellspacing="1" cellpadding="0" class="content">
  <tbody>
    <tr>
      <td colspan="2" class="header1">
        <%= GetText("control_panel") %>
        :
        <asp:Label ID="TitleUserName" runat="server" /></td>
    </tr>
    <tr>
      <td valign="top" class="post" width="150">
        <YAF:ProfileMenu runat="server" />
      </td>
      <% // DefaultView %>
      <td valign="top" class="post">
        <table align="center" cellspacing="0" cellpadding="0" width="100%">
          <tr>
            <td colspan="3" class="header2">
              <%= GetText("your_account") %>
            </td>
          </tr>
          <tr>
            <td width="33%">
              <%= GetText("your_username") %>
            </td>
            <td>
              <asp:Label ID="Name" runat="server" /></td>
            <td valign="top" rowspan="5">
              <img runat="server" id="AvatarImage" align="right" /></td>
          </tr>
          <tr>
            <td>
              <%= GetText("your_email") %>
            </td>
            <td>
              <asp:Label ID="AccountEmail" runat="server" /></td>
          </tr>
          <tr>
            <td>
              <%= GetText("numposts") %>
            </td>
            <td>
              <asp:Label ID="NumPosts" runat="server" /></td>
          </tr>
          <tr>
            <td>
              <%= GetText("groups") %>
            </td>
            <td>
              <asp:Repeater ID="Groups" runat="server">
                <ItemTemplate>
                  <%# DataBinder.Eval(Container.DataItem,"Name") %>
                </ItemTemplate>
                <SeparatorTemplate>
                  ,
                </SeparatorTemplate>
              </asp:Repeater>
            </td>
          </tr>
          <tr>
            <td>
              <%= GetText("joined") %>
            </td>
            <td>
              <asp:Label ID="Joined" runat="server" /></td>
          </tr>
        </table>
      </td>
      <% // end %>
    </tr>
    <tr>
      <td class="footer1" colspan="2">
        &nbsp;</td>
    </tr>
  </tbody>
</table>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
