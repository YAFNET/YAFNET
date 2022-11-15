<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumSubForumList" Codebehind="ForumSubForumList.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>


<asp:Repeater ID="SubforumList" runat="server" OnItemDataBound="SubForumList_ItemCreated" OnPreRender="SubForumList_OnPreRender">
    <HeaderTemplate>
        <div class="card my-1">
        <div class="card-body ps-2 py-1">
        <ul class="list-inline">
            <li class="list-inline-item">
                <span class="fw-bold small text-secondary">
                    <YAF:LocalizedLabel ID="SubForums" LocalizedTag="SUBFORUMS" runat="server" />:
                </span>
            </li>
 </HeaderTemplate>
    <ItemTemplate>
        <li class="list-inline-item">
            <YAF:Icon IconName="comments" IconType="text-secondary" runat="server" />
            <%#  this.GetForumLink((ForumRead)Container.DataItem) %>
            <asp:Label runat="server"
                       Visible="<%# ((ForumRead)Container.DataItem).ReadAccess  %>"
                       CssClass="badge bg-light text-dark me-1"
                       ToolTip='<%# this.GetText("TOPICS") %>'
                       data-bs-toggle="tooltip">
                <YAF:Icon runat="server"
                          IconName="comments"
                          IconStyle="far"></YAF:Icon>
                <%# this.Topics((ForumRead)Container.DataItem) %>
            </asp:Label>
            <asp:Label runat="server"
                       Visible="<%# ((ForumRead)Container.DataItem).ReadAccess  %>"
                       CssClass="badge bg-light text-dark"
                       ToolTip='<%# this.GetText("Posts") %>'
                       data-bs-toggle="tooltip">
            <YAF:Icon runat="server"
                      IconName="comment"
                      IconStyle="far"></YAF:Icon>
            <%# this.Posts((ForumRead)Container.DataItem) %>
            </asp:Label>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        <li class="list-inline-item" Visible="false" ID="CutOff" runat="server">
            ...
        </li>
        </ul>
        </div></div>
</FooterTemplate>
</asp:Repeater>
