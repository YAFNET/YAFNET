<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OpenAuthProviders.ascx.cs" Inherits="YAF.Controls.OpenAuthProviders" %>
<%@ Import Namespace="ServiceStack" %>


<asp:Panel ID="SocialLoginList" runat="server">
    <hr />
    <div class="mb-3">
        <asp:ListView runat="server" ID="providerDetails" 
                      ItemType="System.String"
                      SelectMethod="GetProviderNames" 
                      ViewStateMode="Disabled">
            <ItemTemplate>
                <div class="d-grid gap-2">
                    <YAF:ThemeButton runat="server" ID="Login" 
                                     Type="None"
                                     Size="Small"
                                     CssClass='<%#: "btn btn-social btn-{0} me-1 mb-1".Fmt(Item.ToLower()) %>'
                                     Icon="<%#: Item.ToLower() %>"
                                     IconCssClass="fab"
                                     CausesValidation="False" 
                                     CommandName="provider" 
                                     OnCommand="Login_OnCommand"
                                     CommandArgument="<%#: Item %>" 
                                     TextLocalizedTag="AUTH_CONNECT"
                                     TitleLocalizedTag="AUTH_CONNECT"
                                     ParamTitle0="<%#: Item %>"
                                     ParamText0="<%#: Item %>"/>
                </div>
            </ItemTemplate>
        </asp:ListView>
    </div>
</asp:Panel>
