<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.help.index" CodeBehind="index.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:HelpMenu runat="server">
    <h1 class="h2">
        <YAF:LocalizedLabel ID="Title" runat="server" LocalizedTag="title" LocalizedPage="HELP_INDEX" />
    </h1>
    <asp:Repeater runat="server" ID="HelpList">
        <ItemTemplate>
            <div class="card my-3">
                <div class="card-header form-inline"><%# this.Eval("HelpTitle") %></div>
                <div class="card-body">
                    <%# this.Eval("HelpContent") %>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:PlaceHolder ID="SearchHolder" runat="server">
        <div class="card my-3">
            <div class="card-header">
                <asp:Label ID="SubTitle" runat="server" />
            </div>
            <div class="card-body">
                <p class="card-text">
                    <asp:Label ID="HelpContent" runat="server" />
                </p>
                <div class="form-inline">
                    <label for='<%= this.search.ClientID %>' class="mr-3">
                        <YAF:LocalizedLabel ID="SearchFor" runat="server" LocalizedTag="searchfor" />
                    </label>
                    <div class="input-group flex-fill">
                        <asp:TextBox runat="server" ID="search" CssClass="form-control" />
                        <div class="input-group-append">
                            <YAF:ThemeButton runat="server" ID="DoSearch"
                                TextLocalizedTag="BTNSEARCH"
                                Type="Primary" Icon="search" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</YAF:HelpMenu>

