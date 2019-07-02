<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopicLine.ascx.cs" Inherits="YAF.Controls.TopicLine" %>

<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Classes" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<div class="row">
    <div class="col-md-6">
        <h5>
            <asp:PlaceHolder ID="SelectionHolder" runat="server" Visible="false">
                <asp:CheckBox ID="chkSelected" runat="server" CssClass="form-check-inline" />
            </asp:PlaceHolder>
            
            <asp:Label runat="server" ID="TopicIcon"></asp:Label>
          
            <asp:Label runat="server" ID="Priority" Visible="False"></asp:Label> 
            <asp:Label runat="server" ID="NewMessage"></asp:Label>
            <asp:HyperLink runat="server" ID="TopicLink"></asp:HyperLink>
            <asp:Label runat="server" ID="FavoriteCount"></asp:Label>
        </h5>
        <asp:Label runat="server" ID="Description" CssClass="font-italic"></asp:Label>
        <p class="card-text">
            <YAF:UserLink runat="server" ID="topicStarterLink">
            </YAF:UserLink>
            <i class="fas fa-calendar-alt fa-fw text-secondary"></i>&nbsp;<YAF:DisplayDateTime runat="server" ID="StartDate">
            </YAF:DisplayDateTime>
            <%
                var actualPostCount = this.TopicRow["Replies"].ToType<int>() + 1;

                if (this.Get<YafBoardSettings>().ShowDeletedMessages)
                {
                    // add deleted posts not included in replies...
                    actualPostCount += this.TopicRow["NumPostsDeleted"].ToType<int>();
                }     

                var tPager = this.CreatePostPager(
                    actualPostCount, this.Get<YafBoardSettings>().PostsPerPage, this.TopicRow["LinkTopicID"].ToType<int>());

                if (tPager != string.Empty)
                {
                    var altMultipages = string.Format(this.GetText("GOTO_POST_PAGER"), string.Empty);
            %>
                <span>- <%--<i class="fas fa-copy fa-fw text-secondary"></i>--%> 
                    <%=tPager%></span>
            <%
           }
            %>
        </p>
    </div>
    <div class="col-md-2">
        <div class="d-flex flex-row flex-md-column justify-content-between justify-content-md-start">
            <div>
                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                    LocalizedTag="REPLIES" LocalizedPage="MODERATE" />:
                <% this.Response.Write(this.FormatReplies()); %>
            </div>
            <div>
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                    LocalizedPage="MODERATE" LocalizedTag="VIEWS" />:
                <% this.Response.Write(FormatViews());%>
            </div>
        </div>
    </div>
    <asp:PlaceHolder runat="server" Visible='<%# !this.TopicRow["LastMessageID"].IsNullOrEmptyDBField() %>'>
    <div class="col-md-4">
        <h6>
            <YAF:ThemeButton runat="server" ID="GoToLastPost" 
                             Size="Small"
                             Icon="share-square"
                             Type="OutlineSecondary"
                             TextLocalizedTag="GO_LAST_POST"
                             CssClass="mt-1 mr-1"></YAF:ThemeButton>
            <YAF:ThemeButton runat="server" ID="GoToLastUnread" 
                             Size="Small"
                             Icon="book-reader"
                             Type="OutlineSecondary"
                             TextLocalizedTag="GO_LASTUNREAD_POST"
                             CssClass="mt-1"></YAF:ThemeButton>
        </h6>
        <hr/>
        <h6><YAF:UserLink runat="server" ID="UserLast"></YAF:UserLink>

            &nbsp;<i class="fas fa-calendar-alt fa-fw text-secondary"></i>&nbsp;
            <YAF:DisplayDateTime runat="server" ID="LastDate"></YAF:DisplayDateTime>
        </h6>
    </div>
    </asp:PlaceHolder>
</div>