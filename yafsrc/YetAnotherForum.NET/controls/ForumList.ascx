<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.ForumList"
	EnableViewState="false" Codebehind="ForumList.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
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
    <div class="col-md-6">
        <h5>
            <asp:PlaceHolder runat="server" ID="ForumIcon"></asp:PlaceHolder>
            <img id="ForumImage1" class="" src="" alt="image" visible="false" runat="server" style="border-width:0" />
          
            <%# this.GetForumLink((System.Data.DataRow)Container.DataItem) %>
            
            <asp:Label CssClass="badge badge-light" runat="server" 
                       Visible='<%# ((System.Data.DataRow)Container.DataItem)["Viewing"].ToType<int>() > 0 %>'>
                <%# this.GetViewing(Container.DataItem) %>
            </asp:Label>
        </h5>
        <asp:Label runat="server" ID="Description"  Visible='<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]").ToString().IsSet() %>'>
            <%# this.Page.HtmlEncode(DataBinder.Eval(Container.DataItem, "[\"Description\"]")) %>
        </asp:Label>

        <p class="card-text">
            <span id="ModListMob_Span" Visible="false" runat="server">
                <YAF:ForumModeratorList ID="ForumModeratorListMob" Visible="false" runat="server"  />
            </span>
            <YAF:ForumSubForumList ID="SubForumList" runat="server" 
                                   DataSource='<%# this.GetSubforums( (System.Data.DataRow)Container.DataItem ) %>'
                                   Visible='<%# this.HasSubforums((System.Data.DataRow)Container.DataItem) %>' />
        </p>
    </div>
    <asp:PlaceHolder runat="server" Visible='<%# ((System.Data.DataRow)Container.DataItem)["RemoteURL"] == DBNull.Value %>'>
    <div class="col-md-2">
        <ul>
            <li class="list-unstyled">
                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                    LocalizedTag="TOPICS" />:
                <%# this.Topics(Container.DataItem) %>
            </li>
            <li class="list-unstyled">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                    LocalizedTag="POSTS" />:
                <%# this.Posts(Container.DataItem) %>
            </li>
        </ul>
    </div>
    
    <div class="col-md-4">
        <YAF:ForumLastPost DataRow="<%# Container.DataItem %>" Visible='<%# (((System.Data.DataRow)Container.DataItem)["RemoteURL"] == DBNull.Value) %>'
                           ID="lastPost" runat="server" />
    </div>
    </asp:PlaceHolder>
</div>
		
	</ItemTemplate>
</asp:Repeater>
