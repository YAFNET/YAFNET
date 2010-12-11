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
<br style="clear: both" />
       <asp:Panel id="TopicsTabs" runat="server">
               <ul>
                 <li><a href="#ActiveTopicsTab"><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ActiveTopics" LocalizedPage="MyTopics" /></a></li>
                 <asp:PlaceHolder ID="FavoriteTopicsTabTitle" runat="server">
		         <li><a href="#FavoriteTopicsTab"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="FavoriteTopics" LocalizedPage="MyTopics" /></a></li>
                 </asp:PlaceHolder>		        
               </ul>
                <div id="ActiveTopicsTab">
                   <YAF:MyTopicsList runat="server" ID="ActiveTopics" CurrentMode="Active"/>
                </div>
                <asp:PlaceHolder ID="FavoriteTopicsTabContent" runat="server">
                <div id="FavoriteTopicsTab">
                  <YAF:MyTopicsList runat="server" ID="FavoriteTopics" CurrentMode="Favorite" />
                </div>
                </asp:PlaceHolder>
             </asp:Panel>
        <asp:HiddenField runat="server" ID="hidLastTab" Value="0" />
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
