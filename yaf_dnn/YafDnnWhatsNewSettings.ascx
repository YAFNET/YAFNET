<%@ Control Language="C#" AutoEventWireup="false" Inherits="YAF.DotNetNuke.YafDnnWhatsNewSettings" Codebehind="YafDnnWhatsNewSettings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm">
    <div class="dnnFormItem">
        <asp:Label id="lInfo" runat="server"></asp:Label>
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblTab" runat="server" controlname="ddLTabs" suffix=":"></dnn:label>
        <asp:DropDownList id="YafInstances" Width="325" runat="server"
                datavaluefield="ModuleID" datatextfield="ModuleTitle">
             </asp:DropDownList>
    <div class="dnnFormItem">
        <dnn:label id="lblMaxResult" runat="server" controlname="txtMaxResult" suffix=":"></dnn:label>
        <asp:TextBox ID="txtMaxResult" runat="server"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:label id="lblRelativeTime" runat="server" controlname="UseRelativeTime" suffix=":"></dnn:label>
        <asp:CheckBox ID="UseRelativeTime" runat="server" Checked="true"></asp:CheckBox>
    </div>
    <div class="dnnFormItem">
        <dnn:label id="labelHtmlHeader" runat="server" controlname="HtmlHeader" suffix=":"></dnn:label>
        <asp:TextBox ID="HtmlHeader" runat="server" TextMode="MultiLine" Rows="2"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:label id="labelHtmlItem" runat="server" controlname="HtmlItem" suffix=":"></dnn:label>
        <asp:TextBox ID="HtmlItem" runat="server" TextMode="MultiLine" Rows="5"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:label id="labelHtmlFooter" runat="server" controlname="HtmlFooter" suffix=":"></dnn:label>
        <asp:TextBox ID="HtmlFooter" runat="server" TextMode="MultiLine" Rows="2"></asp:TextBox>
    </div>
</div>