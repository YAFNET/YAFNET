<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumSubForumList" Codebehind="ForumSubForumList.ascx.cs" %>


<asp:Repeater ID="SubforumList" runat="server" OnItemCreated="SubforumList_ItemCreated">
    <HeaderTemplate>        
        <ul class="list-inline">
            <li class="list-inline-item">
                <span class="font-weight-bold">
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
</FooterTemplate>
</asp:Repeater>
