<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Help" Codebehind="Help.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:HelpMenu runat="server">
    <h1 class="h2"><YAF:LocalizedLabel ID="Title" runat="server" LocalizedTag="title" LocalizedPage="HELP_INDEX" /></h1>
    <asp:Repeater runat="server" ID="HelpList">
        <ItemTemplate>
            <div class="card mb-3">
                <div class="card-header form-inline"><%# this.Eval("Title") %></div>
                <div class="card-body">
                    <%# this.Eval("Content") %>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:PlaceHolder ID="SearchHolder" runat="server">
        <div class="card mb-3">
            <div class="card-header"><asp:Label id="SubTitle" runat="server" /></div>
            <div class="card-body">
                <p class="card-text"><asp:Label ID="HelpContent" runat="server" /></p>
                <div class="form-inline">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="search"
                                   CssClass="mr-3">
                            <YAF:LocalizedLabel ID="SearchFor" runat="server" 
                                                LocalizedTag="searchfor" />
                        </asp:Label>
                        <asp:TextBox runat="server" ID="search" CssClass="form-control mr-3" />
                    </div>
                    <YAF:ThemeButton runat="server" ID="DoSearch" CssClass="mr-3" 
                                     TextLocalizedTag="BTNSEARCH"
                                     type="Primary" Icon="search"/>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</YAF:HelpMenu>

