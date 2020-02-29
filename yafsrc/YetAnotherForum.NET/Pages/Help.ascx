<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Help" CodeBehind="Help.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:HelpMenu runat="server">
    <h1 class="h2">
        <YAF:LocalizedLabel ID="Title" runat="server" LocalizedTag="title" LocalizedPage="HELP_INDEX" />
    </h1>
    <asp:Repeater runat="server" ID="HelpList">
        <ItemTemplate>
            <div class="card my-3">
                <div class="card-header form-inline"><%# this.Eval("Title") %></div>
                <div class="card-body">
                    <%# this.Eval("Content") %>
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
                <div class="form-row">
                    <asp:Label runat="server" AssociatedControlID="search"
                        CssClass="col-lg-6">
                        <YAF:LocalizedLabel ID="SearchFor" runat="server"
                            LocalizedTag="searchfor" />
                    </asp:Label>
                    <div class="col-lg-6">
                        <asp:TextBox runat="server" ID="search" CssClass="form-control" />
                    </div>
                </div>
                <div class="form-row float-right">
                    <YAF:ThemeButton runat="server" ID="DoSearch" CssClass="my-2 mr-1"
                        TextLocalizedTag="BTNSEARCH"
                        Type="Primary" Icon="search" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</YAF:HelpMenu>

