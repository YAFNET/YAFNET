<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumSubForumList" Codebehind="ForumSubForumList.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>


<asp:Repeater ID="SubforumList" runat="server" OnItemCreated="SubForumList_ItemCreated">
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
        </li>
    </ItemTemplate>
    <FooterTemplate>
	<li class="list-inline-item">
            <asp:Label Text="..." Visible="false" ID="CutOff" runat="server" />
			</li>
        </ul>
        </div></div>
</FooterTemplate>
</asp:Repeater>
