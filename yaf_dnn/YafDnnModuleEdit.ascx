<%@ Control language="c#" Inherits="YAF.DotNetNuke.YafDnnModuleEdit" CodeBehind="YafDnnModuleEdit.ascx.cs" AutoEventWireup="False" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div style="width:100%;height:10px;border-top: 1px dotted black"></div>
<table cellpadding="2" cellspacing="2" style="margin-left:20px">
    <tr>
      <td class="SubHead" style="width:33%">
        <dnn:label id="BoardName" runat="server" controlname="BoardID" Style="margin-bottom:10px" Suffix=":"></dnn:label>
      </td>
      <td>
        <asp:dropdownlist autopostback="true" cssclass="NormalTextBox" runat="server" id="BoardID" style="width:150px"/>&nbsp;
        <asp:Button runat="server" id="create" cssclass="wizButton" />
      </td>
    </tr>
    <tr>
      <td class="SubHead">
        <dnn:label id="Category" runat="server" controlname="CategoryID" Suffix=":"></dnn:label>
      </td>
      <td>
        <asp:dropdownlist cssclass="NormalTextBox" runat="server" id="CategoryID" style="width:150px"/>
      </td>
   </tr>
   <tr>
      <td class="SubHead">
        <dnn:label id="RemoveTabNameLabel" runat="server" controlname="RemoveTabName" Suffix=":"></dnn:label>
      </td>
      <td>
        <asp:dropdownlist cssclass="NormalTextBox" runat="server" id="RemoveTabName" style="width:150px"/>
      </td>
   </tr>
   <tr>
      <td class="SubHead">
        <dnn:label id="Theme" runat="server" controlname="ThemeID" Suffix=":"></dnn:label>
      </td>
      <td>
        <asp:CheckBox runat="server" id="OverrideTheme" AutoPostBack="True" /><br /><asp:dropdownlist cssclass="NormalTextBox" runat="server" id="ThemeID" style="width:150px" Enabled="false"/>
      </td>
   </tr>
   <tr>
      <td class="SubHead">
        <dnn:label id="InheritLanguage" runat="server" controlname="InheritDnnLanguage" Suffix=":"></dnn:label>
      </td>
      <td>
        <asp:CheckBox runat="server" id="InheritDnnLanguage"  />
      </td>
   </tr>
   <tr>
      <td class="SubHead">
        <dnn:label id="SyncProfile" runat="server" controlname="AutoSyncProfile" Suffix=":"></dnn:label>
      </td>
      <td>
        <asp:CheckBox runat="server" id="AutoSyncProfile"  />
      </td>
   </tr>
   <tr>
      <td>
      </td>
      <td>
       <asp:Button runat="server" id="update" cssclass="wizButton" />
       &nbsp;
       <asp:Button runat="server" id="cancel" cssclass="wizButton" />
      </td>
    </tr>
</table>
<div style="width:100%;height:10px;border-top: 1px dotted black"></div>
