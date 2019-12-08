<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumSubForumList" Codebehind="ForumSubForumList.ascx.cs" %>


<asp:Repeater ID="SubforumList" runat="server" OnItemCreated="SubForumList_ItemCreated">
    <HeaderTemplate>
        <div class="card"><div class="card-body pl-2 py-0">        
        <ul class="list-inline">
            <li class="list-inline-item">
                <span class="font-weight-bold small">
                    <YAF:LocalizedLabel ID="SubForums" LocalizedTag="SUBFORUMS" runat="server" />:
                </span>
            </li>
 </HeaderTemplate>
    <ItemTemplate>
        <li class="list-inline-item">
            <asp:PlaceHolder ID="ForumIcon" runat="server" />&nbsp;<%#  this.GetForumLink((System.Data.DataRow)Container.DataItem) %>
        </li>
    </ItemTemplate>
    <FooterTemplate>
            <asp:Label Text="..." Visible="false" ID="CutOff" runat="server" />
        </ul>
        </div></div>
</FooterTemplate>
</asp:Repeater>
