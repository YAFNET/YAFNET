<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Pages.cp_editbuddies" Codebehind="cp_editbuddies.ascx.cs" %>
<%@ Import Namespace="YAF.Classes.Core" %>
<%@ Register TagPrefix="YAF" TagName="BuddyList" Src="../controls/BuddyList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table width="100%" cellspacing="1" cellpadding="0" class="content">
    <tr>
        <td colspan="2" class="header1">
            <YAF:LocalizedLabel ID="BuddyList" runat="server" LocalizedTag="YOUR_BUDDYLIST" />
        </td>
    </tr>
    <tr>
        <td valign="top" rowspan="2">
            <DotNetAge:Tabs ID="BuddiesTabs" runat="server" ActiveTabEvent="Click" AsyncLoad="false"
                AutoPostBack="false" Collapsible="false" ContentCssClass="" ContentStyle="" Deselectable="false"
                EnabledContentCache="false" HeaderCssClass="" HeaderStyle="" OnClientTabAdd=""
                OnClientTabDisabled="" OnClientTabEnabled="" OnClientTabLoad="" OnClientTabRemove=""
                OnClientTabSelected="" OnClientTabShow="" SelectedIndex="0" Sortable="false"
                Spinner="">
                <Animations>
                    <DotNetAge:AnimationAttribute Name="HeightTransition" AnimationType="height" Value="toggle" />
                </Animations>
                <Views>
                    <DotNetAge:View runat="server" ID="BuddyListTab" NavigateUrl=""
                        HeaderCssClass="" HeaderStyle="" Target="_blank">
                        <YAF:BuddyList runat="server" ID="BuddyList1" />
                    </DotNetAge:View>
                    <DotNetAge:View runat="server" ID="PendingRequestsTab" NavigateUrl=""
                        HeaderCssClass="" HeaderStyle="" Target="_blank">
                        <YAF:BuddyList runat="server" ID="PendingBuddyList" />
                    </DotNetAge:View>
                    <DotNetAge:View runat="server" ID="YourRequestsTab" NavigateUrl=""
                        HeaderCssClass="" HeaderStyle="" Target="_blank">
                        <YAF:BuddyList runat="server" ID="BuddyRequested" />
                    </DotNetAge:View>
                </Views>
            </DotNetAge:Tabs>
        </td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
