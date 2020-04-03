<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.ForumList"
	EnableViewState="false" Codebehind="ForumList.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Register TagPrefix="YAF" TagName="ForumLastPost" Src="ForumLastPost.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumModeratorList" Src="ForumModeratorList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumSubForumList" Src="ForumSubForumList.ascx" %>

<asp:Repeater ID="ForumList1" runat="server" OnItemCreated="ForumList1_ItemCreated">
    <SeparatorTemplate>
        <div class="row">
            <div class="col">
                <hr/>
            </div>
        </div>
    </SeparatorTemplate>
	<ItemTemplate>        
        <div class="row">
            <div class='<%# ((System.Data.DataRow)Container.DataItem)["RemoteURL"].IsNullOrEmptyDBField() ? "col-md-8" : "col" %>'>
                <h5>
                    <asp:PlaceHolder runat="server" ID="ForumIcon"></asp:PlaceHolder>
                    <asp:Image id="ForumImage1" Visible="false" runat="server" />
          
                    <%# this.GetForumLink((System.Data.DataRow)Container.DataItem) %>
            
                    <asp:Label CssClass="badge badge-light" runat="server" 
                               Visible='<%# ((System.Data.DataRow)Container.DataItem)["Viewing"].ToType<int>() > 0 %>'>
                        <%# this.GetViewing((System.Data.DataRow)Container.DataItem) %>
                    </asp:Label>
                    <asp:PlaceHolder runat="server" Visible='<%# ((System.Data.DataRow)Container.DataItem)["RemoteURL"].IsNullOrEmptyDBField() %>'>
                        <asp:Label runat="server" 
                                   CssClass="badge badge-light mr-1"
                                   ToolTip='<%# this.GetText("TOPICS") %>'
                                   data-toggle="tooltip">
                            <YAF:Icon runat="server" 
                                      IconName="comments" 
                                      IconStyle="far"></YAF:Icon>
                            <%# this.Topics((System.Data.DataRow)Container.DataItem) %>
                        </asp:Label>
                        <asp:Label runat="server"
                                   CssClass="badge badge-light" 
                                   ToolTip='<%# this.GetText("Posts") %>'
                                   data-toggle="tooltip">
                            <YAF:Icon runat="server" 
                                      IconName="comment"
                                      IconStyle="far"></YAF:Icon>
                            <%# this.Posts((System.Data.DataRow)Container.DataItem) %>
                        </asp:Label>
                    </asp:PlaceHolder>
                    <YAF:ForumModeratorList ID="ForumModeratorListMob" Visible="false" runat="server"  />
                </h5>
                <h6 class="card-subtitle text-muted">
                    <%# ((System.Data.DataRow)Container.DataItem)["Description"]  %>
                </h6>
                <YAF:ForumSubForumList ID="SubForumList" runat="server"
                                       DataSource="<%# this.GetSubForums((System.Data.DataRow)Container.DataItem ) %>"
                                       Visible="<%# this.HasSubForums((System.Data.DataRow)Container.DataItem) %>" />
            </div>
            <asp:PlaceHolder runat="server" Visible='<%# ((System.Data.DataRow)Container.DataItem)["RemoteURL"].IsNullOrEmptyDBField() %>'>
                <div class="col-md-4 text-secondary">
                <div class="card bg-light card-post-last">
                    <div class="card-body py-2 pl-2">
                        <YAF:ForumLastPost ID="lastPost" runat="server" 
                                           DataRow="<%# (System.Data.DataRow)Container.DataItem %>"/>
                    </div>
                </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </ItemTemplate>
</asp:Repeater>
