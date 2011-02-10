<%@ Control language="c#" Inherits="YAF.DotNetNuke.YafDnnModuleImport" CodeBehind="YafDnnModuleImport.ascx.cs" AutoEventWireup="False" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<table cellpadding="2" cellspacing="2" style="margin-left:20px">
  <tr>
    <td class="SubHead" style="width:50%">
      <asp:Label ID="lImport" runat="server" ResourceKey="lImport"></asp:Label>
    </td>
    <td>
      <asp:Button runat="server" id="btnImportUsers" />
    </td>
  </tr>
  <tr>
    <td colspan="2">
      <asp:Label ID="lInfo" runat="server" FontBold="true"></asp:Label>
    </td>
  </tr>
  <tr>
    <td colspan="2">
      <div style="width:100%;height:10px;border-top: 1px dotted black"></div>
    </td>
  </tr>
  <tr>
    <td class="SubHead" style="width:50%">
      <dnn:label id="lblAddScheduler" runat="server"  ResourceKey="lblAddScheduler" controlname="btnAddScheduler" suffix=":" CssClass="SubHead"></dnn:label>
    </td>
    <td>
      <asp:Button id="btnAddScheduler" CommandArgument="add" runat="server" ></asp:Button>
    </td>
  </tr>
  <tr>
    <td colspan="2">
      <div style="width:100%;height:10px;border-top: 1px dotted black"></div>
    </td>
  </tr>
  <tr>
    <td></td>
    <td>
      <asp:Button runat="server" id="Close" cssclass="CommandButton" />
    </td>
  </tr>
</table>