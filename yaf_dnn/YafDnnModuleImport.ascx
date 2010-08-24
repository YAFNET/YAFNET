<%@ Control language="c#" Inherits="YAF.DotNetNuke.YafDnnModuleImport" CodeBehind="YafDnnModuleImport.ascx.cs" AutoEventWireup="False" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div>
    <asp:Label ID="lImport" runat="server" Text="Import all Users of your Portal and also syncronize Roles:"></asp:Label>
    <asp:Button runat="server" id="btnImportUsers" />
</div>

<div><asp:Label ID="lInfo" runat="server" FontBold="true"></asp:Label></div>

<hr />

<div>
  <dnn:label id="lblAddScheduler" runat="server"  ResourceKey="lblAddScheduler" controlname="btnAddScheduler" suffix=":" CssClass="SubHead" Text="YAF Import Scheduler"></dnn:label>
  <asp:Button id="btnAddScheduler" Text="Install Yaf User Importer Scheduler" CommandArgument="add" runat="server" ></asp:Button>
</div>

<hr />

<p><asp:linkbutton runat="server" id="Close" cssclass="CommandButton" /></p>
