<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.mytopics" Codebehind="mytopics.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="MyTopicsList" Src="../controls/MyTopicsList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator">
</div>
<%--<table class="command" cellspacing="0" cellpadding="0" width="100%" style="padding-bottom: 10px;">
    <tr>
        <td align="right">
            <YAF:LocalizedLabel ID="SinceLabel" runat="server" LocalizedTag="SINCE" />
            <asp:DropDownList ID="Since" runat="server" AutoPostBack="True" OnSelectedIndexChanged="Since_SelectedIndexChanged" />
        </td>
    </tr>
</table>--%>
<asp:UpdatePanel ID="TopicsUpdatePanel" runat="server">
    <ContentTemplate>
        <br style="clear: both" />
        <DotNetAge:Tabs ID="TopicsTabs" runat="server" ActiveTabEvent="Click" AsyncLoad="false"
            AutoPostBack="false" Collapsible="false" ContentCssClass="" ContentStyle="" Deselectable="false"
            EnabledContentCache="false" HeaderCssClass="" HeaderStyle="" OnClientTabAdd=""
            OnClientTabDisabled="" OnClientTabEnabled="" OnClientTabLoad="" OnClientTabRemove=""
            OnClientTabSelected="" OnClientTabShow="" SelectedIndex="0" Sortable="false"
            Spinner="">
            <Views>
                <DotNetAge:View runat="server" ID="ActiveTopicsTab" NavigateUrl="" HeaderCssClass=""
                    HeaderStyle="" Target="_blank">
                    <YAF:MyTopicsList runat="server" ID="ActiveTopics" CurrentMode="Active"/>
                </DotNetAge:View>
                <DotNetAge:View runat="server" ID="FavoriteTopicsTab" NavigateUrl="" HeaderCssClass=""
                    HeaderStyle="" Target="_blank">
                    <YAF:MyTopicsList runat="server" ID="FavoriteTopics" CurrentMode="Favorite" />
                </DotNetAge:View>
            </Views>
        </DotNetAge:Tabs>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:PlaceHolder ID="ForumJumpHolder" runat="server">
    <div id="DivForumJump">
        <YAF:LocalizedLabel ID="ForumJumpLabel" runat="server" LocalizedTag="FORUM_JUMP" />
        &nbsp;<YAF:ForumJump ID="ForumJump1" runat="server" />
    </div>
</asp:PlaceHolder>
<div id="DivIconLegend">
    <YAF:IconLegend ID="IconLegend1" runat="server" />
</div>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
