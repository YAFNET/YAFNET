<%@ Control language="c#" Inherits="YAF.DotNetNuke.YafDnnModuleImport" CodeBehind="YafDnnModuleImport.ascx.cs" AutoEventWireup="False" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm">
    <div class="dnnFormItem">
        <dnn:label ID="lImport" runat="server" ResourceKey="lImport" Suffix=":"></dnn:label>
        <asp:Button runat="server" id="btnImportUsers" />
    </div>
    <div class="dnnFormItem">
        <asp:Label ID="lInfo" runat="server" FontBold="true"></asp:Label>
    </div>
    <div class="dnnFormItem">
      <div style="width:100%;height:10px;border-top: 1px dotted black"></div>
    </div>
    <div class="dnnFormItem">
      <dnn:label id="lblAddScheduler" runat="server"  ResourceKey="lblAddScheduler" controlname="btnAddScheduler" suffix=":"></dnn:label>
      <asp:Button id="btnAddScheduler" CommandArgument="add" runat="server" ></asp:Button>
    </div>
    <ul class="dnnActions dnnClear">
        <li>
            <asp:Button runat="server" id="Close"  CssClass="dnnPrimaryAction" />
        </li>
    </ul>
</div>