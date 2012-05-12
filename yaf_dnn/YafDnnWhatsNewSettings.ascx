<%@ Control Language="C#" AutoEventWireup="false" Inherits="YAF.DotNetNuke.YafDnnWhatsNewSettings" Codebehind="YafDnnWhatsNewSettings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div style="margin: 0 auto"><asp:Label id="lInfo" runat="server"></asp:Label></div>
<ul>
    <li class="SubHead" style="list-style-type: none;margin: 5px 0px 0px 0px;padding: 5px 0px 5px 18px;">
      <dnn:label id="lblTab" runat="server" controlname="ddLTabs" suffix=":"></dnn:label>
       <asp:DropDownList id="YafInstances" Width="325" runat="server"
                datavaluefield="ModuleID" datatextfield="ModuleTitle">
             </asp:DropDownList>
    </li>
    <li class="SubHead" style="list-style-type: none;margin: 5px 0px 0px 0px;padding: 5px 0px 5px 18px;">
      <dnn:label id="lblMaxResult" runat="server" controlname="txtMaxResult" suffix=":"></dnn:label>
      <asp:TextBox ID="txtMaxResult" runat="server"></asp:TextBox>
    </li>
    <li class="SubHead" style="list-style-type: none;margin: 5px 0px 0px 0px;padding: 5px 0px 5px 18px;">
      <dnn:label id="lblRelativeTime" runat="server" controlname="UseRelativeTime" suffix=":"></dnn:label>
      <asp:CheckBox ID="UseRelativeTime" runat="server" Checked="true"></asp:CheckBox>
    </li>
     <li class="SubHead" style="list-style-type: none;margin: 5px 0px 0px 0px;padding: 5px 0px 5px 18px;">
      <dnn:label id="labelHtmlHeader" runat="server" controlname="HtmlHeader" suffix=":"></dnn:label>
      <asp:TextBox ID="HtmlHeader" runat="server" TextMode="MultiLine" CssClass="NormalTextBox" Rows="2"></asp:TextBox>
    </li>
    <li class="SubHead" style="list-style-type: none;margin: 5px 0px 0px 0px;padding: 5px 0px 5px 18px;">
      <dnn:label id="labelHtmlItem" runat="server" controlname="HtmlItem" suffix=":"></dnn:label>
      <asp:TextBox ID="HtmlItem" runat="server" TextMode="MultiLine" CssClass="NormalTextBox" Rows="5"></asp:TextBox>
    </li>
    <li class="SubHead" style="list-style-type: none;margin: 5px 0px 0px 0px;padding: 5px 0px 5px 18px;">
      <dnn:label id="labelHtmlFooter" runat="server" controlname="HtmlFooter" suffix=":"></dnn:label>
      <asp:TextBox ID="HtmlFooter" runat="server" TextMode="MultiLine" CssClass="NormalTextBox" Rows="2"></asp:TextBox>
    </li>
</ul>

