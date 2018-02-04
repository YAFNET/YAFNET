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
                         NavigateUrl="javascript:getSeachResultsData(0);">
        </YAF:ThemeButton>
        <button type="button" class="btn btn-secondary dropdown-toggle dropdown-toggle-split" id="optionsDropDown" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
            <YAF:LocalizedLabel runat="server" LocalizedTag="Options"></YAF:LocalizedLabel>
        </button>
        <div class="dropdown-menu" aria-labelledby="optionsDropDown">
            <form class="px-4 py-3">
                <div class="dropdown-item">
                    <label for='<%= this.txtSearchStringFromWho.ClientID %>'> <YAF:LocalizedLabel runat="server" LocalizedTag="postedby" /></label>
                    <asp:TextBox ID="txtSearchStringFromWho" runat="server" CssClass="form-control searchUserInput" Width="100%" />
                </div>
            </form>
            <div class="dropdown-divider"></div>
            <form class="px-4 py-3">
                <div class="form-group">
                    <label for='<%= this.listSearchWhat.ClientID %>'>
                        <YAF:LocalizedLabel runat="server" LocalizedTag="KEYWORDS" />
                    </label>
                    <asp:DropDownList ID="listSearchWhat" runat="server" CssClass="custom-select searchWhat" Width="100%" />
                </div>
                <div class="form-group">
                    <label for='<%= this.listForum.ClientID %>'>
                        <YAF:LocalizedLabel runat="server" LocalizedTag="SEARCH_IN" />
                    </label>
                    <asp:DropDownList ID="listForum" runat="server" CssClass="custom-select searchForum" Width="100%" />
                </div>
                <div class="form-group">
                    <label for='<%= this.TitleOnly.ClientID %>'>
                        <YAF:LocalizedLabel runat="server" LocalizedTag="SEARCH_TITLEORBOTH" />
                    </label>
                    <asp:DropDownList ID="TitleOnly" runat="server" CssClass="custom-select titleOnly" Width="100%" />
                </div>
                <div class="form-group">
                    <label for='<%= this.listResInPage.ClientID %>'>
                        <YAF:LocalizedLabel runat="server" LocalizedTag="SEARCH_RESULTS" />
                    </label>
                    <asp:DropDownList ID="listResInPage" runat="server" CssClass="custom-select resultsPage" Width="100%" />
                </div>
            </form>
        </div>
    </div>
</div>
                            
<div id="SearchResultsListBox">

    <div id="SearchResultsPager"></div>
    <div id="SearchResultsLoader">
        <div class="modal fade" id="loadModal" tabindex="-1" role="dialog" aria-labelledby="loadModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-body">
                        <p class="text-center">
                        <img title='<%= this.Get<ILocalization>().GetText("COMMON", "LOADING") %>' 
                             src='<%=YafForumInfo.GetURLToContent("images/loader.gif") %>' 
                             alt='<%= this.Get<ILocalization>().GetText("COMMON", "LOADING") %>'  />
                        </p>
                        <h5 class="text-center"><%= this.Get<ILocalization>().GetText("COMMON", "LOADING") %></h5>
                    </div>
                </div>
        </div>
        </div>
    </div>
                                
                                    
    <div id="SearchResultsPlaceholder" 
         data-url='<%=YafForumInfo.ForumClientFileRoot %>' 
         data-userid='<%= YafContext.Current.PageUserID %>'
         data-notext='<%= this.Get<ILocalization>().GetText("NO_SEARCH_RESULTS") %>' 
         data-posted='<%= this.Get<ILocalization>().GetText("POSTED") %>' 
         data-by='<%= this.Get<ILocalization>().GetText("By") %>' 
         data-lastpost='<%= this.Get<ILocalization>().GetText("GO_LAST_POST") %>' 
         data-topic='<%= this.Get<ILocalization>().GetText("COMMON","VIEW_TOPIC") %>' 
         style="clear: both;">
    </div>
</div>

<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
