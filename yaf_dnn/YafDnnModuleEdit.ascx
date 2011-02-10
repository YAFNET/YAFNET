<%@ Control language="c#" Inherits="YAF.DotNetNuke.YafDnnModuleEdit" CodeBehind="YafDnnModuleEdit.ascx.cs" AutoEventWireup="False" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div style="width:100%;height:10px;border-top: 1px dotted black"></div>
<table cellpadding="2" cellspacing="2" style="margin-left:20px">
    <tr>
      <td class="SubHead" style="width:50%">
        <dnn:label id="BoardName" runat="server" controlname="BoardID" Style="margin-bottom:10px"></dnn:label>
      </td>
      <td style="width:50%">
        <asp:dropdownlist autopostback="true" cssclass="NormalTextBox" runat="server" id="BoardID"/>&nbsp;
        <asp:Button runat="server" id="create" cssclass="CommandButton" />
      </td>
    </tr>
    <tr>
      <td class="SubHead">
        <dnn:label id="Category" runat="server" controlname="CategoryID"></dnn:label>
      </td>
      <td>
        <asp:dropdownlist cssclass="NormalTextBox" runat="server" id="CategoryID"/>
      </td>
   </tr>
   <tr>
      <td class="SubHead">
        <dnn:label id="InheritLanguage" runat="server" controlname="InheritDnnLanguage"></dnn:label>
      </td>
      <td>
        <asp:CheckBox runat="server" id="InheritDnnLanguage"  />
      </td>
   </tr>
   <tr>
      <td class="SubHead">
        <dnn:label id="SyncProfile" runat="server" controlname="AutoSyncProfile"></dnn:label>
      </td>
      <td>
        <asp:CheckBox runat="server" id="AutoSyncProfile"  />
      </td>
   </tr>
   <tr>
      <td>
      </td>
      <td>
       <asp:Button runat="server" id="update" cssclass="CommandButton" />
       &nbsp;
       <asp:Button runat="server" id="cancel" cssclass="CommandButton" />
      </td>
    </tr>
</table>
<div style="width:100%;height:10px;border-top: 1px dotted black"></div>
