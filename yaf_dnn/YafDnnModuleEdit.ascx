<%@ Control language="c#" Inherits="YAF.DotNetNuke.YafDnnModuleEdit" CodeBehind="YafDnnModuleEdit.ascx.cs" AutoEventWireup="False" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm">
    <div class="dnnFormItem">
        <dnn:label id="BoardName" runat="server" controlname="BoardID" Suffix=":"></dnn:label>
        <asp:dropdownlist autopostback="true" runat="server" id="BoardID" />&nbsp;
        <asp:Button runat="server" id="create" cssclass="wizButton" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="Category" runat="server" controlname="CategoryID" Suffix=":"></dnn:label>
        <asp:dropdownlist runat="server" id="CategoryID"/>
    </div>
    <div class="dnnFormItem">
        <dnn:label id="RemoveTabNameLabel" runat="server" controlname="RemoveTabName" Suffix=":"></dnn:label>
        <asp:dropdownlist runat="server" id="RemoveTabName" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="InheritLanguage" runat="server" controlname="InheritDnnLanguage" Suffix=":"></dnn:label>
        <asp:CheckBox runat="server" id="InheritDnnLanguage"  />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="SyncProfile" runat="server" controlname="AutoSyncProfile" Suffix=":"></dnn:label>
        <asp:CheckBox runat="server" id="AutoSyncProfile"  />
    </div>
    <ul class="dnnActions dnnClear">
        <li>
            <asp:Button runat="server" id="update" cssclass="dnnPrimaryAction" />
        </li>
        <li>
            <asp:Button runat="server" id="cancel" cssclass="dnnSecondaryAction" />
        </li>
    </ul>
</div>
