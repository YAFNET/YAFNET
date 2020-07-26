<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Search" CodeBehind="Search.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />

<div class="input-group dropleft">
    <asp:TextBox runat="server" CssClass="form-control searchInput" ID="searchInput"></asp:TextBox>
    <YAF:ThemeButton runat="server"
            ID="GoSearch"
            Type="Primary"
            Icon="search"
            TextLocalizedTag="btnsearch"
            NavigateUrl="javascript:getSearchResultsData(0);">
        </YAF:ThemeButton>
        <YAF:ThemeButton runat="server"
                         CssClass="dropdown-toggle"
                         DataToggle="dropdown"
                         Type="Secondary"
                         Icon="cog"
                         TextLocalizedTag="OPTIONS"
                         TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>
        <div class="dropdown-menu" aria-labelledby="optionsDropDown">
            <div class="px-3 py-1">
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="txtSearchStringFromWho">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="postedby" />
                    </asp:Label>
                    <asp:TextBox ID="txtSearchStringFromWho" runat="server" CssClass="form-control searchUserInput" />
                </div>
            </div>
            <div class="px-3 py-1">
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="SearchStringTag">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="Tags" />
                    </asp:Label>
                    <asp:TextBox ID="SearchStringTag" runat="server" CssClass="form-control searchTagInput" />
                </div>
            </div>
            <div class="dropdown-divider"></div>
            <div class="px-3 py-1">
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="listSearchWhat">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="KEYWORDS" />
                    </asp:Label>
                    <asp:DropDownList ID="listSearchWhat" runat="server" CssClass="form-select searchWhat" />
                </div>
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="listForum">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="SEARCH_IN" />
                    </asp:Label>
                    <asp:DropDownList ID="listForum" runat="server" CssClass="select2-image-select searchForum" />
                </div>
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="TitleOnly">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="SEARCH_TITLEORBOTH" />
                    </asp:label>
                    <asp:DropDownList ID="TitleOnly" runat="server" CssClass="form-select titleOnly" />
                </div>
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="listResInPage">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="SEARCH_RESULTS" />
                    </asp:Label>
                    <asp:DropDownList ID="listResInPage" runat="server" CssClass="form-select resultsPage" />
                </div>
            </div>
            <div class="px-3">
                <YAF:ThemeButton runat="server"
                                 Type="Primary"
                                 Size="Small"
                                 TextLocalizedTag="OK"
                                 TextLocalizedPage="COMMON"
                                 CssClass="btn-block"></YAF:ThemeButton>
            </div>
    </div>
</div>

<div id="SearchResultsListBox">

    <div id="SearchResultsPagerTop" class="mt-3"></div>
    <div id="SearchResultsLoader">
        <div class="modal fade" id="loadModal" tabindex="-1" role="dialog" aria-labelledby="loadModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-body text-center">
                        <div class="fa-3x">
                            <i class="fas fa-spinner fa-pulse"></i>
                        </div>
                        <h5 class="text-center"><%= this.Get<ILocalization>().GetText("COMMON", "LOADING") %></h5>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <div id="SearchResultsPlaceholder"
         data-url="<%= BoardInfo.ForumClientFileRoot %>"
         data-minimum="<%= this.Get<BoardSettings>().SearchStringMinLength %>"
         data-userid="<%= this.PageContext.PageUserID %>"
         data-notext='<%= this.Get<ILocalization>().GetAttributeText("NO_SEARCH_RESULTS") %>'
         data-posted='<%= this.Get<ILocalization>().GetAttributeText("POSTED") %>'
         data-by='<%= this.Get<ILocalization>().GetAttributeText("By") %>'
         data-lastpost='<%= this.Get<ILocalization>().GetAttributeText("GO_LAST_POST") %>'
         data-topic='<%= this.Get<ILocalization>().GetAttributeText("COMMON","VIEW_TOPIC") %>'
         style="clear: both;">
    </div>
    
    <div id="SearchResultsPagerBottom" class="mt-3"></div>
</div>
