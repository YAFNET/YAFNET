<%@ Control Language="c#" AutoEventWireup="false" Codebehind="reguser.ascx.cs" Inherits="yaf.pages.admin.reguser" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<yaf:PageLinks runat="server" id="PageLinks" />
<yaf:adminmenu runat="server" id="Adminmenu1">
  <TABLE class="content" cellSpacing="1" cellPadding="0" width="100%">
    <TR>
      <TD class="header1" colSpan="2">New User</TD>
    </TR>
    <TR>
      <TD class="header2" align="center" colSpan="2">Registration Details</TD>
    </TR>
    <TR>
      <TD class="postheader" width="50%">User Name:</TD>
      <TD class="post">
        <asp:TextBox id="UserName" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator id="Requiredfieldvalidator1" runat="server" NAME="Requiredfieldvalidator1" EnableClientScript="False"
          ControlToValidate="UserName" ErrorMessage="User Name is required."></asp:RequiredFieldValidator></TD>
    </TR>
    <TR>
      <TD class="postheader">Password:</TD>
      <TD class="post">
        <asp:TextBox id="Password" runat="server" TextMode="Password"></asp:TextBox>
        <asp:RequiredFieldValidator id="Requiredfieldvalidator2" runat="server" NAME="Requiredfieldvalidator2" EnableClientScript="False"
          ControlToValidate="Password" ErrorMessage="Password is required."></asp:RequiredFieldValidator></TD>
    </TR>
    <TR>
      <TD class="postheader">Retype Password:</TD>
      <TD class="post">
        <asp:TextBox id="Password2" runat="server" TextMode="Password"></asp:TextBox>
        <asp:CompareValidator id="Comparevalidator1" runat="server" NAME="Comparevalidator1" EnableClientScript="False"
          ControlToValidate="Password2" ErrorMessage="Passwords didnt match." ControlToCompare="Password"></asp:CompareValidator></TD>
    </TR>
    <TR>
      <TD class="postheader">Email Address:</TD>
      <TD class="post">
        <asp:TextBox id="Email" runat="server"></asp:TextBox></TD>
    </TR>
    <TR>
      <TD class="header2" align="center" colSpan="2">Profile Information</TD>
    </TR>
    <TR>
      <TD class="postheader">Location:</TD>
      <TD class="post">
        <asp:TextBox id="Location" runat="server"></asp:TextBox></TD>
    </TR>
    <TR>
      <TD class="postheader">Home Page:</TD>
      <TD class="post">
        <asp:TextBox id="HomePage" runat="server"></asp:TextBox></TD>
    </TR>
    <TR>
      <TD class="header2" align="center" colSpan="2">Forum Preferences</TD>
    </TR>
    <TR>
      <TD class="postheader">Time Zone:</TD>
      <TD class="post">
        <asp:DropDownList id="TimeZones" runat="server" DataValueField="Value" DataTextField="Name"></asp:DropDownList></TD>
    </TR>
    <TR>
      <TD class="footer1" align="center" colSpan="2">
        <asp:Button id="ForumRegister" runat="server" text="Register"></asp:Button>
        <asp:button id="cancel" runat="server" text="Cancel"></asp:button></TD>
    </TR>
  </TABLE>
</yaf:adminmenu>
<yaf:savescrollpos runat="server" id="Savescrollpos1" />
