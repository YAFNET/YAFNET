<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="../../../pages/mytopics.ascx.cs" Inherits="YAF.Pages.mytopics" %>
<%@ Register TagPrefix="YAF" TagName="MyTopicsList" Src="mytopicslist.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator">
</div>
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
                    <YAF:MyTopicsList runat="server" ID="ActiveTopics" Mode="1"/>
                </DotNetAge:View>
                <DotNetAge:View runat="server" ID="FavoriteTopicsTab" NavigateUrl="" HeaderCssClass=""
                    HeaderStyle="" Target="_blank">
                    <YAF:MyTopicsList runat="server" ID="FavoriteTopics" Mode="2" />
                </DotNetAge:View>
            </Views>
        </DotNetAge:Tabs>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:PlaceHolder ID="ForumJumpHolder" runat="server">
    <div id="DivForumJump" runat="server" visible="false">
        <YAF:LocalizedLabel ID="ForumJumpLabel" runat="server" LocalizedTag="FORUM_JUMP" />
        &nbsp;<YAF:ForumJump ID="ForumJump1" runat="server" />
    </div>
</asp:PlaceHolder>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>


