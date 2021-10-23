<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Help" CodeBehind="Help.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:HelpMenu runat="server">
    <h1 class="h2">
        <YAF:LocalizedLabel ID="Title" runat="server" LocalizedTag="title" LocalizedPage="HELP_INDEX" />
    </h1>
    <asp:Repeater runat="server" ID="HelpList">
        <ItemTemplate>
            <div class="card my-3">
                <div class="card-header d-flex"><%# this.Eval("Title") %></div>
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
                <div class="g-3">
                    <asp:Label runat="server" AssociatedControlID="search">
                        <YAF:LocalizedLabel ID="SearchFor" runat="server"
                                            LocalizedTag="searchfor" />
                    </asp:Label>
                    <asp:TextBox runat="server" ID="search" CssClass="form-control" />
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton runat="server" ID="DoSearch"
                                 TextLocalizedTag="BTNSEARCH"
                                 Type="Primary" 
                                 Icon="search" />
            </div>
        </div>
    </asp:PlaceHolder>
</YAF:HelpMenu>

