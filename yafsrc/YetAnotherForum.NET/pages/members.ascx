<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.members" CodeBehind="members.ascx.cs" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>


<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-search-plus fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="SearchMembersLocalizedLabel" runat="server" LocalizedTag="Search_Members" />
            </div>
        <div class="card-body">
                <div class="form-row">
                    <div class="form-group col-md-4">
                        <asp:Label runat="server" AssociatedControlID="Group">
                            <YAF:LocalizedLabel ID="SearchRolesLocalizedLabel" runat="server" LocalizedTag="Search_Role" />
                        </asp:Label>
                        <asp:DropDownList ID="Group" runat="server" CssClass="standardSelectMenu" Width="300">
                        </asp:DropDownList>
                    </div>
                    <div class="form-group col-md-4">
                        <asp:Label runat="server" AssociatedControlID="Ranks">
                            <YAF:LocalizedLabel ID="SearchRankLocalizedLabel" runat="server" LocalizedTag="Search_Rank" />
                            <asp:DropDownList ID="Ranks" runat="server" CssClass="standardSelectMenu" Width="300">
                            </asp:DropDownList>
                        </asp:Label>
                    </div>
                    <div class="form-group col-md-4">
                        <asp:Label runat="server" AssociatedControlID="NumPostsTB">
                            <YAF:LocalizedLabel ID="NumPostsLabel" runat="server" LocalizedTag="NUMPOSTS" />
                        </asp:Label>
                        <asp:DropDownList ID="NumPostDDL" runat="server" CssClass="standardSelectMenu">
                        </asp:DropDownList>
                        <asp:TextBox ID="NumPostsTB" runat="server" 
                                     CssClass="Numeric" 
                                     TextMode="Number"></asp:TextBox>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="UserSearchName">
                            <YAF:LocalizedLabel ID="SearchMemberLocalizedLabel" runat="server" LocalizedTag="Search_Member" />
                        </asp:Label>
                        <asp:TextBox ID="UserSearchName" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="SearchByUserName" runat="server" 
                                 OnClick="Search_Click"
                                 TextLocalizedTag="BTNSEARCH" 
                                 Type="Primary"
                                 Icon="search">
                </YAF:ThemeButton>
                &nbsp;
                <YAF:ThemeButton ID="ResetUserSearch" runat="server" 
                                 OnClick="Reset_Click" 
                                 TextLocalizedTag="CLEAR" 
                                 Type="Secondary"
                                 Icon="trash">
                </YAF:ThemeButton>
            </div>
        </div>
    </div>
</div>

<YAF:AlphaSort ID="AlphaSort1" runat="server" />
<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-users fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
            </div>
           <div class="card-body">
               <YAF:Pager runat="server" ID="Pager" OnPageChange="Pager_PageChange" />
               <div class="table-responsive">
               <table class="table mt-3">
                   <thead>
                       <tr>
                          <th scope="col">
                              <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="Avatar" />
                          </th>
                          <th scope="col">
                              <asp:Label runat="server" id="SortUserName" />
                              <asp:LinkButton runat="server" ID="UserName" OnClick="UserName_Click" />
                          </th>
                          <th scope="col">
                              <asp:Label runat="server" id="SortRank" />
                              <asp:LinkButton runat="server" ID="Rank" OnClick="Rank_Click" />
                          </th>
                          <th scope="col">
                              <asp:Label runat="server" id="SortJoined" />
                              <asp:LinkButton runat="server" ID="Joined" OnClick="Joined_Click" />
                          </th>
                           <th scope="col">
                               <asp:Label runat="server" id="SortPosts" />
                               <asp:LinkButton runat="server" ID="Posts"  OnClick="Posts_Click" />
                           </th>
                           <th scope="col">
                               <asp:Label runat="server" id="SortLastVisit" />
                               <asp:LinkButton runat="server" ID="LastVisitLB" OnClick="LastVisitLB_Click" />
                           </th>
                       </tr>
                   </thead>
    <asp:Repeater ID="MemberList" runat="server">
        <ItemTemplate>
            <tr>
                <td>
                   <img src="<%# this.GetAvatarUrlFileName(this.Eval("UserID").ToType<int>(), this.Eval("Avatar").ToString(), this.Eval("AvatarImage").ToString().IsSet(), this.Eval("Email").ToString()) %>" alt="<%# this.HtmlEncode(DataBinder.Eval(Container.DataItem,"Name").ToString()) %>"
                        title="<%# this.HtmlEncode(this.Get<YafBoardSettings>().EnableDisplayName ? this.Eval("DisplayName").ToString() : this.Eval("Name").ToString()) %>" class="avatarimage img-rounded" />
                </td>
                <td>
                    <YAF:UserLink ID="UserProfileLink" runat="server" IsGuest="False" ReplaceName='<%# this.Get<YafBoardSettings>().EnableDisplayName ? this.Eval("DisplayName").ToString() : this.Eval("Name").ToString() %>'  UserID='<%# this.Eval("UserID").ToType<int>() %>'
                        Style='<%# this.Eval("Style") %>' />
                </td>
                <td>
                    <%# this.Eval("RankName") %>
                </td>
                <td>
                    <%# this.Get<IDateTime>().FormatDateLong((DateTime)((System.Data.DataRowView)Container.DataItem)["Joined"]) %>
                </td>
                <td>
                    <%# "{0:N0}".FormatWith(((System.Data.DataRowView)Container.DataItem)["NumPosts"]) %>
                </td>
                <td>
                    <%# this.Get<IDateTime>().FormatDateLong((DateTime)((System.Data.DataRowView)Container.DataItem)["LastVisit"]) %>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
               </table>  
                   </div>
               <YAF:Pager runat="server" LinkedPager="Pager" OnPageChange="Pager_PageChange" />
           </div>
        </div>
    </div>
</div>