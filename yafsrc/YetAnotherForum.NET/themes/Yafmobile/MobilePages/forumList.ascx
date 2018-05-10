<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="../../../controls/forumList.ascx.cs"
    Inherits="YAF.Controls.ForumList" EnableViewState="false" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Register TagPrefix="YAF" TagName="ForumSubForumList" Src="../../../controls/ForumSubForumList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumLastPost" Src="../../../controls/ForumLastPost.ascx" %>
<asp:Repeater ID="ForumList1" runat="server" OnItemCreated="ForumList1_ItemCreated">
    <ItemTemplate>
        <tr class="forumRow post">
            <td class="forumIconCol">
                <YAF:ThemeImage ID="ThemeForumIcon" runat="server" />
                <img id="ForumImage1" class="" src="" alt="" visible="false" runat="server" style="border-width:0px;" />	
            </td>
            <td class="forumLinkCol">
                <div class="forumheading">
                    <%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
                </div>
                <div class="forumviewing">
                    <%# GetViewing(Container.DataItem) %>
                </div>
                <div class="subforumheading" runat="server" Visible='<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]").ToString().IsSet() %>'>
                    <%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
                </div>
                <YAF:ForumSubForumList ID="SubForumList" runat="server" DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>'
                    Visible='<%# HasSubforums( (System.Data.DataRow)Container.DataItem ) %>' />
                <YAF:ForumLastPost DataRow="<%# Container.DataItem %>" Visible='<%# (((System.Data.DataRow)Container.DataItem)["RemoteURL"] == DBNull.Value) %>'
                    ID="lastPost" runat="server" />
            </td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr class="forumRow_Alt post_alt">
            <td>
                <YAF:ThemeImage ID="ThemeForumIcon" runat="server" />
                <img id="ForumImage1" class="" src="" alt="" visible="false" runat="server" style="border-width:0px;" />	
            </td>
            <td class="forumLinkCol">
                <div class="forumheading">
                    <%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
                </div>
                <div class="forumviewing">
                    <%# GetViewing(Container.DataItem) %>
                </div>
                <div class="subforumheading" runat="server" Visible='<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]").ToString().IsSet() %>'>
                    <%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
                </div>        
                <YAF:ForumSubForumList ID="ForumSubForumListAlt" runat="server" DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>'
                    Visible='<%# HasSubforums( (System.Data.DataRow)Container.DataItem ) %>' />
                <YAF:ForumLastPost DataRow="<%# Container.DataItem %>" Visible='<%# (((System.Data.DataRow)Container.DataItem)["RemoteURL"] == DBNull.Value) %>'
                    ID="lastPost" runat="server" />
            </td>
        </tr>
    </AlternatingItemTemplate>
</asp:Repeater>
