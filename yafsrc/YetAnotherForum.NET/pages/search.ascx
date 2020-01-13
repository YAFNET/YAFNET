<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.search" CodeBehind="search.ascx.cs" %>

<YAF:PageLinks ID="PageLinks" runat="server" />

<div class="input-group">
    <asp:TextBox runat="server" CssClass="form-control searchInput" ID="searchInput"></asp:TextBox>
    <div class="input-group-append">
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
                <div class="form-group">
                    <label for='<%= this.txtSearchStringFromWho.ClientID %>'>
                        <YAF:LocalizedLabel runat="server" LocalizedTag="postedby" />
                    </label>
                    <asp:TextBox ID="txtSearchStringFromWho" runat="server" CssClass="form-control searchUserInput" />
                </div>
            </div>
            <div class="dropdown-divider"></div>
            <div class="px-3 py-1">
                <div class="form-group">
                    <label for='<%= this.listSearchWhat.ClientID %>'>
                        <YAF:LocalizedLabel runat="server" LocalizedTag="KEYWORDS" />
                    </label>
                    <asp:DropDownList ID="listSearchWhat" runat="server" CssClass="custom-select searchWhat" />
                </div>
                <div class="form-group">
                    <label for='<%= this.listForum.ClientID %>'>
                        <YAF:LocalizedLabel runat="server" LocalizedTag="SEARCH_IN" />
                    </label>
                    <asp:DropDownList ID="listForum" runat="server" CssClass="custom-select searchForum" />
                </div>
                <div class="form-group">
                    <label for='<%= this.TitleOnly.ClientID %>'>
                        <YAF:LocalizedLabel runat="server" LocalizedTag="SEARCH_TITLEORBOTH" />
                    </label>
                    <asp:DropDownList ID="TitleOnly" runat="server" CssClass="custom-select titleOnly" />
                </div>
                <div class="form-group">
                    <label for='<%= this.listResInPage.ClientID %>'>
                        <YAF:LocalizedLabel runat="server" LocalizedTag="SEARCH_RESULTS" />
                    </label>
                    <asp:DropDownList ID="listResInPage" runat="server" CssClass="custom-select resultsPage" />
                </div>
            </div>
            <div class="px-3 text-center pb-2">
                <button type="button" class="btn btn-primary btn-sm w-25"><%= this.Get<ILocalization>().GetText("COMMON", "OK") %></button>
            </div>
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
        data-url='<%=BoardInfo.ForumClientFileRoot %>'
        data-userid='<%= YafContext.Current.PageUserID %>'
        data-notext='<%= this.Get<ILocalization>().GetAttributeText("NO_SEARCH_RESULTS") %>'
        data-posted='<%= this.Get<ILocalization>().GetAttributeText("POSTED") %>'
        data-by='<%= this.Get<ILocalization>().GetAttributeText("By") %>'
        data-lastpost='<%= this.Get<ILocalization>().GetAttributeText("GO_LAST_POST") %>'
        data-topic='<%= this.Get<ILocalization>().GetAttributeText("COMMON","VIEW_TOPIC") %>'
        style="clear: both;">
    </div>
    
    <div id="SearchResultsPagerBottom" class="mt-3"></div>
</div>
